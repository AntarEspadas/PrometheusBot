using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace PrometheusBot.Modules.Fun
{
    public class AModule : ModuleBase<SocketCommandContext>
    {
        private static readonly string _aResponse = Path.Combine(Program.Directory, "assets", "a", "aResponse.txt");

        [ACommand]
        public async Task A()
        {
            string response = null;
            try { response = File.ReadAllText(_aResponse); } catch { }
            if (string.IsNullOrWhiteSpace(response)) return;
            await ReplyAsync(response);
        }
    }

    class ACommandAttribute : NonStandardCommandAttribute
    {
        static readonly HashSet<string> _a;
        static ACommandAttribute()
        {
            string aPath = Path.Combine(Program.Directory, "assets", "a", "a.txt");
            try
            {
                _a = new HashSet<string>(File.ReadAllLines(aPath));
            }
            catch
            {
                _a = new HashSet<string>(new[] { "a", "A" });
            }
        }
        public override bool Validate(ICommandContext context)
        {
            string strippedContent = StripNonSpacingMarks(context.Message.Content);
            strippedContent = strippedContent.Trim();
            strippedContent = strippedContent.Trim(new[] { '\u200B', '\uFEFF' });
            return _a.Contains(strippedContent);
        }
        private static string StripNonSpacingMarks(string str)
        {
            var stringBuilder = new StringBuilder(str.Length);
            foreach (char chr in str)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(chr);
                if (category != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(chr);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
