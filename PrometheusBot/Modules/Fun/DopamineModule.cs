using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using PrometheusBot.Extensions;

namespace PrometheusBot.Modules.Fun
{
    public class DopamineModule : ModuleBase<SocketCommandContext>
    {
        private static string _dopamineFile = Path.Combine(Program.Directory, "assets", "dopamine.txt");
        private static DateTime _lastRead = DateTime.MinValue;
        private static IList<string> _dopamine = Array.Empty<string>();

        //Dopamine command, replies with a random dopamine-inducing image
        [Command("dopamine")]
        [Summary("Replies with a random dopamine-inducing image")]
        public async Task Dopamine()
        {
            ReadDopamine();
            //Return if dopamine list is empty
            if (!_dopamine.Any()) return;

            await ReplyAsync(_dopamine.RandomElement());
        }

        //Store all lines from the dopamine file if it has been modified
        private static void ReadDopamine()
        {
            var lastModified = File.GetLastWriteTime(_dopamineFile);

            if (lastModified < _lastRead) return;

            _lastRead = lastModified;
            var lines = File.ReadAllLines(_dopamineFile)
                .Where(line => !line.StartsWith("#"))
                .ToList();
            _dopamine = lines;
        }
    }
}
