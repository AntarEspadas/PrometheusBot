using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusBot.Services.MessageHistory
{
    public class MessageHistoryList : LinkedList<IMessage>
    {
        public bool IsDeleted { get { return DeletedTimestamp != null; } }
        public DateTimeOffset? DeletedTimestamp { get; set; } = null;
    }
}
