using Discord.Commands;
using Discord;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Module
{
    public class RoleModule:ModuleBase<SocketCommandContext>
    {
        private RoleService service = new RoleService();
        [Command("AddStaticRoles")]
        public async Task AddRoles()
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            try
            {
                service.AddRoles();
                await channel.SendMessageAsync("Done!");
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }

        }
        [Command("AddRole")]
        public async Task AddRole([Remainder]string input)
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            try
            {
                service.AddRole(input, Context.Message.Author.ToString(), Context.Guild.Id.ToString());
                await Context.Channel.SendMessageAsync("Added role(s) for you!");
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("RemoveRole")]
        public async Task RemoveRole([Remainder]string input)
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            try
            {
                service.RemoveRole(input, Context.Message.Author.ToString(), Context.Guild.Id.ToString());
                await Context.Channel.SendMessageAsync("Removed role(s) for you!");

            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
    }
}
