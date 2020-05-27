using Discord;
using Discord.Commands;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Module
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private Service.CommandService service = new Service.CommandService();
        [Command("Cmd")]
        public async Task GetAllCommands()
        {
            try
            {
                var list = await service.GetAll();
                var embedListe = new List<EmbedBuilder>();
                var embedcount = -1;
                var count = 0;
                list.ForEach(o =>
                {
                    if (count % 25 == 0)
                    {
                        embedListe.Add(new EmbedBuilder().WithTitle("Commands"));
                        embedcount++;
                    }
                    embedListe[embedcount].AddField(o.Id + " - " + o.Cmd, o.CmdText);
                    count++;
                });

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                embedListe.ForEach(async o => await channel.SendMessageAsync("", false, o.Build()));
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }

        }
        [Command("CreateCmd")]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task CreateCommand([Remainder] string input)
        {
            try
            {
                await service.Create(input);
                await Context.Channel.SendMessageAsync("Succes");
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }

        }

        [Command("UpdateCmd")]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task UpdateCmd([Remainder]string input)
        {
            try
            {
                await service.Update(input);
                await Context.Channel.SendMessageAsync("Succes");


            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("DeleteCmd")]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task DeleteCmd(int id)
        {
            try
            {
                await service.Delete(id);
                await Context.Channel.SendMessageAsync("Succes");


            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }


        [Command("CreatePredefinedCmd")]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task CreatePredefinedCmd()
        {
            try
            {
                await service.CreateExistingCommands();
                await Context.Channel.SendMessageAsync("Succes");

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
