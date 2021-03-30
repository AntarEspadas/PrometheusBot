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
        public static T RandomElement<T>(this IList<T> list)
        {
            Random random = new();
            int index = random.Next(0, list.Count);
            return list[index];
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
        public static EmbedBuilder Quote(this IUser author, string text, DateTimeOffset? timestamp = null)
        {
            var eab = new EmbedAuthorBuilder()
            {
                IconUrl = author.GetAvatarUrl(),
                Name = author.Username
            };
            var eb = new EmbedBuilder()
            {
                Author = eab,
                Timestamp = timestamp,
                Description = text
            };
            return eb;
        }
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            double itemWeightIndex = new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default;

        }

        public static bool HasRoleName(this IGuildUser user, string roleName)
        {
            var guild = user.Guild;
            var matchingRoles = guild
                .Roles
                .Where(role => role.Name == roleName)
                .Select(role => role.Id)
                .ToHashSet();

            matchingRoles.IntersectWith(user.RoleIds);

            return matchingRoles.Any();
        }
    }
}
