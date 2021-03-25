using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace PrometheusBot.Extensions
{
    public static class Exts
    {
        public static bool IsSubclassOfRawGeneric(this Type type, Type generic)
        {
            while (type != null && type != typeof(object))
            {
                var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (generic == cur)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
        public static async Task<IMessage> GetPreviousMessageAsync(this IMessage message)
        {
            var asyncMessages = message.Channel.GetMessagesAsync(message, Direction.Before, 1);
            var messages = await asyncMessages.FlattenAsync();
            return messages.LastOrDefault();
        }

        public static async Task<IMessage> GetReferencedMessageAsync(this IMessage message)
        {
            var reference = message.Reference;
            if (reference is null) return null;

            return await message.Channel.GetMessageAsync(reference.MessageId.Value);
        }

        public static Task<IMessage> GetReplyingMessageAsync(this IMessage message)
        {
            if (message.Reference is null)
                return message.GetPreviousMessageAsync();
            else
                return message.GetReferencedMessageAsync();
        }
    }
}
