using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrometheusBot.Modules.Utility.History
{
    class MessageHistoryList : LinkedList<IMessage>
    {
        public bool IsDeleted { get { return DeletedTimestamp != null; } }
        public DateTimeOffset? DeletedTimestamp { get; set; } = null;
    }
}
