using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PrometheusBot.Forms
{
    public class FormTestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test form")]
        public async Task TestForm()
        {
            FormTest form = new(Context, 60);
            var result = await form.ShowDialogAsync();
            await ReplyAsync(result.ToString());
        }
    }

    class FormTest : Form
    {
        private Button leftArrow;
        private Button rightArrow;
        private Button cancelButton;
        private string PageText => $"--------------------\n\n\n\n\n\nThe page number is: {page}\n\n\n\n\n\n--------------------";
        private int page;
        public FormTest(SocketCommandContext context, int timeoutSeconds) : base(context, timeoutSeconds)
        {
            leftArrow = new(new Emoji("\u25C0"));
            rightArrow = new(new Emoji("\u25B6"));
            cancelButton = new(new Emoji("\u274C"));

            AddButtonAsync(leftArrow).Wait();
            AddButtonAsync(cancelButton).Wait();
            AddButtonAsync(rightArrow).Wait();

            leftArrow.Clicked += LeftArrowClicked;
            rightArrow.Clicked += RightArrowClicked;
            cancelButton.Clicked += CancelButton_Clicked;

            Text = PageText;
        }

        private void CancelButton_Clicked(object sender, ButtonEventArgs e)
        {
            Result = DialogResult.Cancel;
            CloseAsync();
        }

        private void RightArrowClicked(object sender, ButtonEventArgs e)
        {
            page++;
            Text = PageText;
        }

        private void LeftArrowClicked(object sender, ButtonEventArgs e)
        {
            page--;
            Text = PageText;
        }
    }
}
