using Discord;
using Discord.Commands;
using DiscordBot.Models;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class KeyModule:ModuleBase<SocketCommandContext>
    {
        private KeyService ks = new KeyService();

        [Command("keys")]
        public async Task GetKeys()
        {
            try
            {
                var channel = await Context.User.GetOrCreateDMChannelAsync();
                var KeyList = ks.GetAllKeys();


                var eb = new EmbedBuilder().WithTitle("Keys:");
                KeyList.ForEach(o =>
                {
                    eb.AddField(o.UserName, o.Key + " " + o.KeyLevel).WithCurrentTimestamp();

                });
                await channel.SendMessageAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }


        }

        [Command("AddKey")]
        public async Task AddKey([Remainder]string input)
        {
            try
            {
                ks.CreateKeyFromStringAndSave(input, Context.Message.Author.ToString());
                await Context.Channel.SendMessageAsync("Done");

            }
            catch (FormatException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("DeleteKey")]
        public async Task RemoveKey()
        {
            var removed = ks.CheckIfKeyAuthorExists(new KeyStone { UserName = Context.Message.Author.ToString() });
            if(removed)
            {
                await Context.Channel.SendMessageAsync("Done amigo");
            }
            else
            {
                await Context.Channel.SendMessageAsync("You sure you had a key?");

            }
        }
    }
}
