using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;

namespace DiscordBot
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        public async Task ChangePlaying(DiscordSocketClient client)
        {
            var list = new List<string> { "!CMD for commands", "!Signup for friday raid", "Create multiple characters and set one as main!", "Report bugs to Nillertron#5220" };
              Task.Run(async() =>
            {
                while (true)
                {
                for(int i = 0; i<list.Count;i++)
                    {
                        SetStatus(list[i],client);
                        await Task.Delay(10000);
                    }

                }
            });

        }

        private async Task SetStatus(string status, DiscordSocketClient client)
        {
            await client.SetGameAsync(status);

        }



    }
}
