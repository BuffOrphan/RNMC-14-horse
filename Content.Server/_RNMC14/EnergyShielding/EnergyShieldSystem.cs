using Content.Shared._RNMC14.EnergyShield;
using Content.Shared.Damage;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared._RMC14.Aura;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

// !!! SHITCODE ALERT !!!
//   THIS CODE HAS BEEN
//   WRITTEN BY A PERSON
//  THAT HAS NEVER TOUCHED
//       C# BEFORE  
namespace Content.Server._RNMC14.EnergyShield
{
    public sealed class EnergyShieldSystem : EntitySystem
    {

        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
        [Dependency] private readonly SharedAuraSystem _rmcAura = null!;
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<EnergyShieldComponent, InventoryRelayedEvent<DamageModifyEvent>>(OnDamaged);
            SubscribeLocalEvent<EnergyShieldComponent, GotEquippedEvent>(OnEquipped);
            SubscribeLocalEvent<EnergyShieldComponent, GotUnequippedEvent>(OnUnequipped);
        }

        public EntityUid _equipee;

        /// <summary>
        /// The energy shield item is equipped, we have to track the equipee.
        /// </summary>
        private void OnEquipped(Entity<EnergyShieldComponent> ent, ref GotEquippedEvent args)
        {
            _equipee = args.Equipee;
            ent.Comp.IsEquipped = true;
            if (ent.Comp.TotalDamage < ent.Comp.DamageThreshold)
                _rmcAura.GiveAura(_equipee, ent.Comp.ShieldColor, null);
            else
                _rmcAura.GiveAura(_equipee, ent.Comp.ShieldColorLow, null);
        }
        // This is probably going to be weird if the unequip attempt isn't catched. Is it gonna happen? Probably not. Am I gonna make a failsafe? Fuck No.
        /// <summary>
        /// The energy shield item is unequipped, we no longer need to track the equipee.
        /// </summary>
        private void OnUnequipped(Entity<EnergyShieldComponent> ent, ref GotUnequippedEvent args)
        {
            RemComp<AuraComponent>(_equipee);
            _equipee = default!;
            ent.Comp.IsEquipped = false;
        }

        TimeSpan _rechargeTiming;
        bool _rechargeSoundPlayed;

        private void OnDamaged(EntityUid uid, EnergyShieldComponent comp, ref InventoryRelayedEvent<DamageModifyEvent> args)
        {
            uid = _equipee;
            var _damage = args.Args.Damage.GetTotal();

            if (_damage > 0 && comp.IsEquipped && !comp.IsBroken && comp.IsEnabled)
            {
                comp.TotalDamage += (float)_damage;
                args.Args.Damage = new DamageSpecifier();
                _rechargeTiming = _timing.CurTime + comp.RechargeCooldown;
                _rechargeSoundPlayed = false;

                if (comp.TotalDamage >= comp.DamageThreshold)
                {
                    comp.TotalDamage = comp.DamageThreshold;
                    comp.IsBroken = true;
                    _audioSystem.PlayPvs(comp.ShieldBustedSound, uid);
                    RemComp<AuraComponent>(_equipee);
                    _rmcAura.GiveAura(_equipee, comp.ShieldColorLow, null);
                }

                if (!comp.IsBroken)
                    _audioSystem.PlayPvs(comp.DamagedSound, uid);
            }
        }

        public override void Update(float frameTime)
        {
            var query = EntityQueryEnumerator<EnergyShieldComponent>();
            var time = _timing.CurTime;

            while (query.MoveNext(out var uid, out var comp))
            {
                if (time >= _rechargeTiming && comp.TotalDamage > 0)
                {
                    if (!_rechargeSoundPlayed)
                    {
                        _audioSystem.PlayPvs(comp.ShieldRechargeSound, uid);
                        _rechargeSoundPlayed = true;
                        if (_equipee != null)
                        {
                            RemComp<AuraComponent>(_equipee);
                            _rmcAura.GiveAura(_equipee, comp.ShieldColor, null);
                        }
                    }
                    comp.TotalDamage -= comp.RechargeRateAmount;
                    comp.IsBroken = false;

                    if (comp.TotalDamage <= 0)
                    {
                        comp.TotalDamage = 0;
                    }
                }
            }
        }

    }
}
