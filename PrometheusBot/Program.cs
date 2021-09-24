using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace PrometheusBot
{
    class Program
    {
        public static string Directory { get; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static async Task Main(string[] args)
        {
            ManualResetEvent resetEvent = new(false);
            PrometheusBot prometheusBot = new();
            prometheusBot.Client.Ready += async () => resetEvent.Set();
            InteractiveConsole.InteractiveConsole console = new(prometheusBot);
            await prometheusBot.StartAsync();
            try
            {
                resetEvent.WaitOne();
                await Task.Delay(1000);
                console.Run();
            }
            finally
            {
                await prometheusBot.StopAsync();
            }
        }
    }
}
