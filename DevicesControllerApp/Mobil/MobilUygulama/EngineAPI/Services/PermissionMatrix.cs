using System.Collections.Generic;
using System.Security.Claims;

namespace RehabilitationSystem.EngineAPI.Services
{
    /// <summary>
    /// Rol/izin matrisi: hangi komutu hangi rol kullanabilir.
    /// </summary>
    public static class PermissionMatrix
    {
        private static readonly Dictionary<string, string[]> Rules = new(StringComparer.OrdinalIgnoreCase)
        {
            { "start", new[] { "Admin", "Operator" } },
            { "stop", new[] { "Admin", "Operator" } },
            { "pause", new[] { "Admin", "Operator" } },
            { "resume", new[] { "Admin", "Operator" } },
            { "emergencystop", new[] { "Admin", "Servis", "Operator" } },
            { "up", new[] { "Admin", "Operator", "Servis" } },
            { "down", new[] { "Admin", "Operator", "Servis" } },
            { "left", new[] { "Admin", "Operator", "Servis" } },
            { "right", new[] { "Admin", "Operator", "Servis" } },
            { "footincrease", new[] { "Admin", "Operator", "Servis" } },
            { "footdecrease", new[] { "Admin", "Operator", "Servis" } },
            { "barup", new[] { "Admin", "Operator", "Servis" } },
            { "bardown", new[] { "Admin", "Operator", "Servis" } },
            { "weightincrease", new[] { "Admin", "Operator", "Servis" } },
            { "weightdecrease", new[] { "Admin", "Operator", "Servis" } }
        };

        public static bool IsAllowed(ClaimsPrincipal user, string command, out string reason)
        {
            reason = string.Empty;

            if (!Rules.TryGetValue(command, out var roles))
            {
                reason = "Komut tanımlı değil.";
                return false;
            }

            foreach (var role in roles)
            {
                if (user.IsInRole(role))
                {
                    return true;
                }
            }

            reason = "Bu komut için yetkiniz yok.";
            return false;
        }
    }
}
