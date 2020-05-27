using Discord;
using Discord.Commands;
using DiscordBot.Models;
using DiscordBot.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Service
{
    class RaidService
    {
        private DBHelper db = new DBHelper();
        private SignupService signupService = new SignupService();
        public Raid Create(string input, string discordId, string guildId)
        {
            var splitIndex = Helpers.FindBindingInString(input, '-');
            var raidtitle = input.Substring(0, splitIndex).Trim();
            input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).TrimStart();
            var raidday = Convert.ToDateTime(input);
            var raid = new Raid { CreatedByUser = discordId, GuildId=guildId, RaidActive = true, RaidDay = raidday, RaidTitle = raidtitle };
            db.Create(raid);
            return raid;
        }

        public Raid Edit(string input, string guildId)
        {
            var splitIndex = Helpers.FindBindingInString(input, '-');
            var raidtitle = input.Substring(0, splitIndex).Trim();
            input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).TrimStart();
            splitIndex = Helpers.FindBindingInString(input, '-');
            var id = Convert.ToInt32(input.Substring(0, splitIndex).Trim());
            input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).TrimStart();
            var storedRaid = db.GetSpecificRaid(id, guildId);
            if (input.Length>0)
            {
                var raidday = Convert.ToDateTime(input);
                storedRaid.RaidDay = raidday;
            }
            storedRaid.RaidTitle = raidtitle;
            
            db.Update(storedRaid);
            return storedRaid;
        }
        public List<Raid> GetAll(string guildId)
        {
            var list = db.GetAllRaids(guildId);
            list = list.Where(o => o.RaidActive == true).ToList();
            for(int i = 0; i<list.Count; i++)
            {
                if(list[i].RaidDay.AddHours(6) <= DateTime.Now)
                {
                    list[i].RaidActive = false;
                    db.Update(list[i]);
                    list.RemoveAt(i);
                }
            }
            return list;
        }
        public List<Raid> GetAll()
        {
            var list = db.GetAllRaids();
            list = list.Where(o => o.RaidActive == true).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].RaidDay.AddHours(1) <= DateTime.Now)
                {
                    list[i].RaidActive = false;
                    db.Update(list[i]);
                    list.RemoveAt(i);
                }
            }
            return list;
        }

        public Raid DeleteRaid(int id, string guildId)
        {
            var raid = db.GetSpecificRaid(id,guildId);
            raid.RaidActive = false;
            db.Update(raid);
            return raid;
        }

        public int GetRaidSignupCount(int raidId)
        {
            return db.GetRaidSignupCount(raidId);
        }
        public List<Raid> GetAllInactive(string guildId)
        {
            var list = db.GetAllRaids(guildId);
            list = list.Where(o => o.RaidActive == false).ToList();
            return list;
        }
        public Raid GetSpecificRaid(int raidId, string guildId)
        {
            return db.GetSpecificRaid(raidId,guildId);
        }

        public async Task ObserveThread()
        {
#pragma warning disable CS4014 
            Task.Run(async() =>
            {
                while(true)
                {
                    var raids = GetAll();
                    raids.ForEach(x =>
                    {
                        if (!x.GroupsSorted && (x.RaidDay - DateTime.Now).TotalMinutes <= 30 && (x.RaidDay - DateTime.Now).TotalMinutes > -1)
                        {
                            var avgIlvlList = new List<int>();
                            var list = signupService.SplitGroups(x.Id, ref avgIlvlList, x.GuildId);
                            var SM = new SignupModule();
                            SM.SplitFromThreadGroups(list, x.CreatedByUser, avgIlvlList);
                            x.GroupsSorted = true;
                            db.Update(x);

                        }
                    });
                    await Task.Delay(TimeSpan.FromMinutes(15));
                }

            });
#pragma warning restore CS4014 


        }


    }
}
