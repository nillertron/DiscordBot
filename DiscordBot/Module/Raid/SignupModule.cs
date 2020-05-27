using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Module
{
    public class SignupModule : ModuleBase<SocketCommandContext>
    {
        private SignupService service = new SignupService();
        private RaidService raidService = new RaidService();
        private PlayerService playerService = new PlayerService();
        [Command("CheckRoles")]
        public async Task CheckRoles()
        {
            var guildid = Context.Guild.Id;
            var userList = Program.Client.GetGuild(guildid).Users;

            try
            {
                var list = service.FindPlayersWithNoRoles();
                list.ForEach(o =>
                {
                    var ulist = userList.GetEnumerator();
                    while (ulist.MoveNext())
                    {
                        var user = ulist.Current;
                        if (user.Username + "#" + user.Discriminator == o.DiscordName)
                        {
                            UserExtensions.SendMessageAsync(user, $"Beep Boop you have not selected any roles for {o.IngameName}, it is easily done with !AddRole {o.IngameName} - Role, thanks in advance");
                            break;
                        }
                    }






                });
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        [Command("SignUp")]
        public async Task SignUp([Remainder] string input)
        {
            try
            {

                var signup = service.SignUp(input, Context.Message.Author.ToString(), Context.Guild.Id.ToString());
                var emb = Helpers.GetEmbed("Signup registred", raidService.GetSpecificRaid(signup.RaidId, Context.Guild.Id.ToString()).RaidTitle, playerService.GetSpecificPlayer(signup.PlayerId).IngameName);
                var msg = (RestUserMessage)await Context.Channel.GetMessageAsync(Context.Message.Id);
                await Helpers.ReactToMessage(msg, DefinedEmojis.Stars);
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }

        [Command("DeleteSignUp")]
        public async Task DeleteSignUp(int raidId)
        {
            try
            {
                service.DeleteSignup(raidId, Context.Message.Author.ToString());
                var msg = (RestUserMessage)await Context.Channel.GetMessageAsync(Context.Message.Id);
                await Helpers.ReactToMessage(msg, DefinedEmojis.Stars);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        //[Command("GetAllSignUps")]
        //[RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        //public async Task GetAllSignUps()
        //{
        //    var emb = new EmbedBuilder();
        //    var emb2 = new EmbedBuilder();
        //    int count = 0;
        //    //service.SplitGroups(3);
        //    try
        //    {
        //        var signUpList = service.GetAll();
        //        signUpList.ForEach(async o =>
        //        {
        //            if (count <= 24)
        //                emb.AddField(o.Id.ToString(), o.RaidId.ToString() + " "+ o.PlayerId.ToString());
        //            else
        //                emb2.AddField(o.Id.ToString(), o.RaidId.ToString()+ " " + o.PlayerId.ToString());
        //            count++;

        //        });
        //        await Context.Channel.SendMessageAsync("", false, emb.Build());
        //        await Context.Channel.SendMessageAsync("", false, emb2.Build());

        //    }
        //    catch (Exception e)
        //    {
        //        await Context.Channel.SendMessageAsync(e.Message);
        //    }

        //}

        [Command("GetSignUpsFor")]
        public async Task GetSignupsForSpecificRaid(int id)
        {
            try
            {
                var signUpList = service.GetAllForSpecificRaid(id, Context.Guild.Id.ToString());
                var embedList = new List<EmbedBuilder>();
                int count = 1;
                int embedCount = 0;
                var tankCount = 0;
                var healerCount = 0;
                var dpsCount = 0;
                embedList.Add(new EmbedBuilder());
                signUpList.ForEach(async o =>
                {
                    if (o != null)
                    {
                        var player = playerService.GetSpecificPlayer(o.PlayerId);
                        if (player.Roles.Contains(Role.Tank))
                            tankCount++;
                        else if (player.Roles.Contains(Role.Healer))
                            healerCount++;
                        else
                            dpsCount++;

                        embedList[embedCount].AddField(player.IngameName, player.PClass + " - " + player.Ilvl + " - " + Helpers.GetRoleFromList(player.Roles) + " - CloakLevel:" + player.CloakLevel);
                        if (count % 24 == 0)
                        {
                            embedList.Add(new EmbedBuilder());
                            embedCount++;
                        }
                        count++;
                    }

                });
                embedList[0].WithTitle("Total signups: " + signUpList.Count + " Tanks: " + tankCount + " Healers: " + healerCount + " Dps: " + dpsCount);
                embedList.ForEach(async o => await Context.Channel.SendMessageAsync("", false, o.Build()));
            }
            catch (ArgumentException e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }

        }

        [Command("SplitGroups")]
        [RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task SplitGroups(int raidId)
        {

            try
            {
                var channel = await Context.User.GetOrCreateDMChannelAsync();
                var embedList = new List<EmbedBuilder>();
                int count = 1;
                int embedCount = -1;
                int playerCount = 1;
                var avgList = new List<int>();
                var list = new List<List<SignUp>>();
                await Task.Run(() =>
                {

                    list = service.SplitGroups(raidId, ref avgList, Context.Guild.Id.ToString());
                });
                list.ForEach(o =>
                {
                    embedList.Add(new EmbedBuilder().WithTitle("Raid group " + count + " Avg ilvl: " + avgList[count - 1] + " Player count: " + list[count - 1].Count));
                    embedCount++;
                    playerCount = 1;
                    o.ForEach(x =>
                    {
                        var player = playerService.GetSpecificPlayer(x.PlayerId);
                        if (playerCount % 25 == 0)
                        {
                            embedList.Add(new EmbedBuilder());
                            embedCount++;
                        }
                        embedList[embedCount].AddField(player.IngameName, player.PClass + " - " + Helpers.GetRoleFromList(player.Roles));
                        playerCount++;
                    });
                    count++;
                });
                embedList.ForEach(async o => await channel.SendMessageAsync("", false, o.Build()));
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);
            }
        }
        public async Task SplitFromThreadGroups(List<List<SignUp>> list, string RaidOwnerDiscordId, List<int> avgList)
        {

            try
            {
                var channel = await Program.Client.GetUser(Convert.ToUInt64(RaidOwnerDiscordId)).GetOrCreateDMChannelAsync();
                var embedList = new List<EmbedBuilder>();
                int count = 1;
                int embedCount = -1;
                int playerCount = 1;
                var RaidLeaderDic = new Dictionary<int, Player>();
                list.ForEach(o =>
                {
                    embedList.Add(new EmbedBuilder().WithTitle("Raid group " + count + " Avg ilvl: " + avgList[count - 1] + " Player count: " + list[count - 1].Count));
                    embedCount++;
                    playerCount = 1;
                    o.ForEach(x =>
                    {
                        var player = playerService.GetSpecificPlayer(x.PlayerId);
                        if (player.Roles.Contains(Role.RaidLeader))
                            RaidLeaderDic.Add(count, player);
                        if (playerCount % 25 == 0)
                        {
                            embedList.Add(new EmbedBuilder());
                            embedCount++;
                        }
                        embedList[embedCount].AddField(player.IngameName, player.PClass + " - " + Helpers.GetRoleFromList(player.Roles));
                        playerCount++;
                    });
                    count++;
                });
                int lastKey = 0;
                var RLString = "";
                foreach (var player in RaidLeaderDic)
                {
                    if (player.Key == lastKey)
                        continue;
                    else
                    {
                        var ch = await Program.Client.GetUser(Convert.ToUInt64(player.Value.UserId)).GetOrCreateDMChannelAsync();
                        await ch.SendMessageAsync("Beep boop, you are assigned raid leader for group " + player.Key + " here is a list of players for you to invite");
                        await ch.SendMessageAsync("", false, embedList[player.Key - 1].Build());
                        lastKey = player.Key;
                        RLString += "Group: " + player.Key + " - " + player.Value.IngameName + "\n";
                    }
                }
                await channel.SendMessageAsync("Your raid leaders today are:\n" + RLString);
                embedList.ForEach(async o => await channel.SendMessageAsync("", false, o.Build()));


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
