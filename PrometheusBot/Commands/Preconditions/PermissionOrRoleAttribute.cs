using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PrometheusBot.Extensions;
using PrometheusBot.Model;
using PrometheusBot.Model.Settings;

namespace PrometheusBot.Commands.Preconditions
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PermissionOrRoleAttribute : RequireUserPermissionAttribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        public PermissionOrRoleAttribute(GuildPermission permission) : base(permission)
        {
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (CheckRole(context))
                return PreconditionResult.FromSuccess();
            return await base.CheckPermissionsAsync(context, command, services);
        }
        private bool CheckRole(ICommandContext context)
        {
            if (context.User is IGuildUser user)
            {
                var model = PrometheusModel.Instance;

                SettingLookupInfo info = new("admin:bot-role-name") { GId = context.Guild.Id, CId = context.Channel.Id };
                model.GetSetting(info, out string roleName, true);

                return user.HasRoleName(roleName);
            }
            return false;
        }
    }
}
