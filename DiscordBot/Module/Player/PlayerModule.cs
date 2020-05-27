using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot.Service;
using System.Threading.Tasks;
using DiscordBot.Models;

namespace DiscordBot.Module
{
    public class PlayerModule:ModuleBase<SocketCommandContext>
    {
        private PlayerService service = new PlayerService();
        private RoleService RoleService = new RoleService();
        [Command("RegisterPlayer")]
        public async Task RegisterPlayer([Remainder]string input)
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            try
            {
                var player = await service.RegisterPlayer(input, Context.Message.Author.ToString(), Context.Message.Author.Id.ToString(), Context.Guild.Id.ToString());
                await channel.SendMessageAsync($"Created {player.IngameName} with {player.Ilvl} ilvl, Cloak Level {player.CloakLevel} & mythic + score of {player.MythicScore} with the role {Helpers.GetRoleFromList(player.Roles)} - Class {player.PClass.ToString().Replace('_', ' ')} for Discord User {player.DiscordName}");

            }
            catch (Exception EE)
            {
                await channel.SendMessageAsync(EE.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(EE.Message, EE.StackTrace);
            }
        }
        [Command("DeleteCharacter")]
        public async Task DeletePlayer(string input)
        {
            try
            {
                service.DeletePlayer(input, Context.Message.Author.Id.ToString(), Context.Guild.Id.ToString());
                await Context.Channel.SendMessageAsync("Done amigo");

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }

        [Command("SetMain")]
        public async Task SetMain(string charName)
        {
            try
            {
                service.SetMain(charName, Context.Message.Author.Id.ToString(), Context.Guild.Id.ToString());
                await Context.Channel.SendMessageAsync("Done amigo");

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("GetMyCharacters")]
        public async Task GetMyCharacters()
        {
            try
            {
                var liste = service.GetMyCharacters(Context.Message.Author.Id.ToString(), Context.Guild.Id.ToString());
                var emb = new EmbedBuilder().WithTitle(liste[0].DiscordName + " Characters ");
                liste.ForEach(o =>
                {
                    var s1 = " - This is your main - ";
                    var s2 = "";
                    emb.AddField(o.IngameName + " - " + o.Ilvl.ToString(), "Id: " + o.Id + (o.Main?s1:s2) +" "+ Helpers.GetRoleFromList(o.Roles) + " - Cloak level:" + o.CloakLevel + " - M+ Score:" + o.MythicScore);
                });
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("GetAllPlayers")]
        [RequireUserPermission(GuildPermission.Administrator)]


        public async Task GetAllPlayers()
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            try
            {
                var list = service.GetAll(Context.Guild.Id.ToString());
                var embedPlayerListe = new List<Player>();
                var embedListe = new List<EmbedBuilder>();
                var count = 0;
                var embedcount = -1;
                list.ForEach(o => {
                    if (count % 25 == 0)
                    {
                        embedListe.Add(new EmbedBuilder().WithTitle("All players - Count:" +list.Count));
                        embedcount++;
                    }
                    embedListe[embedcount].AddField(o.IngameName, "Ilvl: " + o.Ilvl + " - Role(s): " + Helpers.GetRoleFromList(o.Roles) + " - Cloak level: " + o.CloakLevel+" - M+ Score: "+ o.MythicScore);
                    count++;

                });

                embedListe.ForEach(async o =>
                {
                    await channel.SendMessageAsync("", false, o.Build());

                });
            }
            catch (Exception e)
            {
                await channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }

        }
        [Command("Update")]
        public async Task Update(string CharName)
        {
            try
            {
                await service.Update(CharName, Context.Guild.Id.ToString());
                await Context.Channel.SendMessageAsync("Done amigo!");
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("UpdateAll")]
        public async Task UpdateAll()
        {
            try
            {
                service.UpdateAll();

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }

        [Command("UpdatePlayerGuildsAndIds")]
        public async Task UpdatePlayerGuildAndIds()
        {
            var guildid = Context.Guild.Id;
            var userList = Context.Guild.Users;

            try
            {
                await service.UpdatePlayerGuildAndIds(userList);
                await Context.Channel.SendMessageAsync("Done");

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
