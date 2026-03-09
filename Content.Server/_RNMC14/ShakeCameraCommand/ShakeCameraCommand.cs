
using Content.Shared._RMC14.CameraShake;
using Content.Shared.Administration;
using Content.Shared.Mind.Components;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.Admin)]
    public sealed class ShakeCameraCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly RMCCameraShakeSystem _rmcCameraShake = default!;

        public override string Command => "shakecamera";

        public override string Description => Loc.GetString("cmd-shakecamera-desc");

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {

            if (args.Length < 2)
            {
                shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }

            if (!int.TryParse(args[0], out var entInt))
            {
                shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }

            if (!int.TryParse(args[1], out var shakes))
            {
                shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }

            if (!int.TryParse(args[2], out var strength))
            {
                shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }

            if (!float.TryParse(args[3], out var spacing))
            {
                shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }

            var nent = new NetEntity(entInt);

            if (!EntityManager.TryGetEntity(nent, out var eUid))
            {
                shell.WriteLine(Loc.GetString("shell-invalid-entity-id"));
                return;
            }

            EntityUid entId = (EntityUid)eUid;

            TimeSpan spacingTime = TimeSpan.FromSeconds(spacing);

            _rmcCameraShake.ShakeCamera(entId, shakes, strength, spacingTime);

        }
    }
}
