using Discord;
using Discord.Commands;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Module
{
    public class SubscribedChannelModule:ModuleBase<SocketCommandContext>
    {
        private SubscribedChannelService service = new SubscribedChannelService();
        public async Task SendMessage(ulong channelId, string message, string raidTitle)
        {
            var client = Program.Client;
            var channel = client.GetChannel(Convert.ToUInt64(channelId)) as IMessageChannel;
            var emb = new EmbedBuilder();
            emb.WithTitle("Raid Signup Reminder").AddField(raidTitle, message).WithFooter("Use !CMD for help");
            await channel.SendMessageAsync("", false, emb.Build());
        }
        [Command("SubscribeRaidChannel")]
        public async Task SubscribeRaidChannel()
        {
            var user = Context.Message.Author.ToString();
            if (user != "Nillertron#5220")
                return;
            try
            {
                var ch = Context.Channel;
                await service.SubscribeChannel(ch.Id, Context.Guild.Id.ToString());
                await Context.Channel.SendMessageAsync("Done");

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("UnSubscribeRaidChannel")]
        public async Task UnSubscribeRaidChannel()
        {
            var user = Context.Message.Author.ToString();
            if (user != "Nillertron#5220")
                return;
            try
            {
                var ch = Context.Channel;
                await service.UnSubscribeChannel(ch.Id);
                await Context.Channel.SendMessageAsync("Done");

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }

        [Command("NotifyChannels")]
        public async Task NotifyChannels()
        {
            try
            {
                var dif = DateTime.Now.Day - DateTime.Now.AddDays(2).Day;
                await service.Notify($"@here Everyone who is interested in the friday raid remember to sign up.\nIf you want to raid on your main and it is !setmain, you can signup by using\n!signup RaidId\n otherwise you'll have to use the format:\n!Signup - Character name - RaidId. \nYou can register multiple characters!", "Friday Raid", Context.Channel.Id.ToString());

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
    }
}
