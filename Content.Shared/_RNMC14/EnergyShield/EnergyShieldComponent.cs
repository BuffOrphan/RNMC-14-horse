
using Robust.Shared.Audio;

namespace Content.Shared._RNMC14.EnergyShield;

[RegisterComponent]
public sealed partial class EnergyShieldComponent : Component
{

    [DataField("isEnabled")]
    public bool IsEnabled;

    [DataField("isBroken")]
    public bool IsBroken;

    [DataField("totalDamage")]
    public float TotalDamage;

    [DataField("damageThreshold")]
    public float DamageThreshold = 200;

    [DataField("rechargeCooldown")]
    public TimeSpan RechargeCooldown = TimeSpan.FromSeconds(5);

    [DataField("rechargeRate")]
    public TimeSpan RechargeRate = TimeSpan.FromMilliseconds(25);

    [DataField("rechargeRateAmount")]
    public float RechargeRateAmount = 10;

    [DataField("isEquipped")]
    public bool IsEquipped;

    [DataField("damagedSound")]
    public SoundSpecifier? DamagedSound = new SoundCollectionSpecifier("shieldhit");

    [DataField("shieldBustedSound")]
    public SoundSpecifier? ShieldBustedSound = new SoundCollectionSpecifier("shieldbusted");

    [DataField("shieldRechargeSound")]
    public SoundSpecifier? ShieldRechargeSound = new SoundPathSpecifier("/Audio/_RNMC14/Effects/shield_charge.ogg");

    [DataField("shieldColor")]
    public Color ShieldColor = Color.LightBlue;

    [DataField("shieldColorLow")]
    public Color ShieldColorLow = Color.Red;

}
