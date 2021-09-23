using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PrometheusBot.Commands;
using PrometheusBot.Services;
using PrometheusBot.Services.Audio;
using PrometheusBot.Services.MessageHistory;
using PrometheusBot.Services.Music;
using PrometheusBot.Services.NonStandardCommands;
using PrometheusBot.Services.Settings;
using Victoria;

namespace PrometheusBot
{
    class Program
    {
        public static string Directory { get; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static async Task Main(string[] args)
        {
            PrometheusBot prometheusBot = new();
            await prometheusBot.StartAsync();
            try
            {
                await Task.Delay(-1);
            }
            finally
            {
                await prometheusBot.StopAsync();
            }
        }
    }
}
