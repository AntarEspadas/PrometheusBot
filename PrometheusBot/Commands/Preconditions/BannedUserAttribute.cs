using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrometheusBot.Commands.Preconditions
{
    class BannedUserAttribute : PreconditionAttribute
    {
        private ulong _userId;

        public string Message { get; set; }

        public BannedUserAttribute(ulong userId)
        {
            _userId = userId;
        }
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User.Id != _userId)
                return PreconditionResult.FromSuccess();
            string message = Message ?? $"User {context.User.Username} is not allowed to use this command";
            return PreconditionResult.FromError(message);
        }
    }
}
