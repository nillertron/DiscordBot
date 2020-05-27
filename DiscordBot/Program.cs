using Discord;
using Discord.WebSocket;
using DiscordBot.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using API;
using DiscordBot.Module;
using DiscordBot.Service;
using System.Linq;

namespace DiscordBot
{
    class Program
    {
        public static DiscordSocketClient Client;
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();
            client.Log += Log;
            await client.LoginAsync(TokenType.Bot, "");
            await client.StartAsync();
            new CommandHandler(client, new Discord.Commands.CommandService());
            Client = client;
            var dbhelper = new DBHelper();
            var tm = new TestModule();
            tm.ChangePlaying(client);
            Startup();
            await Task.Delay(-1);
        }

        private Task Log(Discord.LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;

        }

        private async Task Startup()
        {
            DBHelper db = new DBHelper();
            var PM = new PlayerModule();
            var RS = new RaidService();
            PM.UpdateAll();
            var list = db.GetAllSubscribedChannels();
            var ss = new SubscribedChannelService();
            if (list.Count != 0)
            {
                list = list.Where(x => x.Type == SubscribeType.Raid).ToList();
                list.ForEach(async o => await ss.Attach(o));
            }
            ss.ObserveRaids();
            RS.ObserveThread();
            var player = db.GetAll<Player>().Where(o => o.DiscordName == "Nillertron#5220").FirstOrDefault();
            var msgService = new LogMessageService();
            await msgService.Attach(player);
            var guildService = new GuildService();


        }
    }
}
