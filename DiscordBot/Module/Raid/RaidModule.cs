using Discord.Commands;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Module
{
    public class RaidModule:ModuleBase<SocketCommandContext>
    {
        private RaidService service = new RaidService();
        [Command("CreateRaid")]
        //[RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task CreateRaid([Remainder] string input)
        {
            try
            {
                var raid = service.Create(input, Context.Message.Author.Id.ToString(), Context.Guild.Id.ToString() );
                var embed = Helpers.GetEmbed(raid.RaidTitle, "ID:", raid.Id.ToString()).AddField("Date: ", raid.RaidDay.ToString("dd-MM HH:mm"));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                
            }
            catch(Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message + "Use format: !createraid RAID TITLE - MM/DD/YYYY HH:MM");
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);

            }
        }
        [Command("EditRaid")]
        [RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task EditRaid([Remainder] string input)
        {
            try
            {
                var raid = service.Edit(input, Context.Guild.Id.ToString());
                var embed = Helpers.GetEmbed(raid.RaidTitle, "ID:", raid.Id.ToString()).AddField("Date: ", raid.RaidDay.ToString("dd-MM HH:mm"));
                await Context.Channel.SendMessageAsync("", false, embed.Build());

            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message + "Use format: !EditRaid RAID TITLE - RaidId - (DD/MM/YYYY HH:MM) date can be left out if no changes are to be made.");
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);

            }
        }
        
        [Command("DeleteRaid")]
        [RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task CreateRaid(int id)
        {
            try
            {
                var raid = service.DeleteRaid(id, Context.Guild.Id.ToString());
                var embed = Helpers.GetEmbed(raid.RaidTitle, "ID:", raid.Id.ToString()).AddField("Date: ", raid.RaidDay.ToString("dd-MM HH:mm"));
                await Context.Channel.SendMessageAsync("", false, embed.Build());
                await Context.Channel.SendMessageAsync("Raid set inactive");


            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message + "Use format: !deleteraid RaidId");
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);

            }
        }
        [Command("Raids")]
        public async Task GetAllRaids()
        {
            try
            {
                var raidList = service.GetAll(Context.Guild.Id.ToString());
                raidList.ForEach(async o =>
                {
                    var embed = Helpers.GetEmbed(o.RaidTitle, "ID:", o.Id.ToString()).AddField("Date: ", o.RaidDay.ToString("dd-MM HH:mm")).AddField("Sign Ups: ", service.GetRaidSignupCount(o.Id).ToString());
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                });


            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(e.Message);
                var lm = new LogMessageService();
                await lm.CreateLogEntry(e.Message, e.StackTrace);

            }
        }
        [Command("InactiveRaids")]
        [RequireUserPermission(Discord.GuildPermission.ManageChannels)]
        public async Task GetAllInactiveRaids()
        {
            try
            {
                var raidList = service.GetAllInactive(Context.Guild.Id.ToString());
                raidList.ForEach(async o =>
                {
                    var embed = Helpers.GetEmbed(o.RaidTitle, "ID:", o.Id.ToString()).AddField("Date: ", o.RaidDay.ToString("dd-MM HH:mm")).AddField("Sign Ups: ", service.GetRaidSignupCount(o.Id).ToString());
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                });


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
