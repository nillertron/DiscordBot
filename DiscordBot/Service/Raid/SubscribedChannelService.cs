using DiscordBot.Models;
using DiscordBot.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Service
{
    class SubscribedChannelService:ISubject
    {
        private DBHelper db = new DBHelper();
        private RaidService raidService = new RaidService();
        private static List<IObserver> SubscribeList = new List<IObserver>();
        public async Task SubscribeChannel(ulong channel, string guildId)
        {
            var ch = new SubscribedChannel {GuildId=guildId, channelId = channel.ToString(), Type = SubscribeType.Raid };
            db.Create(ch);
            await Attach(ch);
        }
        public async Task UnSubscribeChannel(ulong channel)
        {
            var ch = SubscribeList.Where(o =>
            {
                var ob = o as SubscribedChannel;
                return ob.channelId == channel.ToString();
            }).FirstOrDefault();
            if (ch != null)
            {
                db.Delete(ch);
                await Detach(ch);
            }
            else
                throw new FormatException("Subscriber not found");

        }

        public async Task ObserveRaids()
        {
#pragma warning disable CS4014 
            Task.Run(async() =>
            {
                await Task.Delay(5000);
                while(true)
                {
                    var list = raidService.GetAll();
                    list.ForEach(x =>
                    {
                        var difference = (x.LastNotified.Date == new DateTime(0001,01,01)) ? TimeSpan.FromDays(5) :  DateTime.Now.Date - x.LastNotified.Date;
                        if(difference >= TimeSpan.FromDays(2))
                        {
                            var dif = x.RaidDay.Date - DateTime.Today.Date;
                            if (dif <= TimeSpan.FromDays(7))
                            {
                                x.LastNotified = DateTime.Now;
                                db.Update(x);
                                var sb = new SubscribedChannelModule();
                                var s1 = "Raid is tonight!";
                                var s2 = $"There is only {dif.Days} days left.";
                                Notify($"@here Everyone who is interested in the {x.RaidTitle} remember to sign up. {((dif.Days == 0) ? s1 : s2)} \nIf you want to raid on your main and it is !setmain, you can signup by using\n!signup {x.Id}\n otherwise you'll have to use the format:\n!Signup Character name - {x.Id}. \nYou can register multiple characters!", x.RaidTitle + " - " + x.Id, x.GuildId);
                            }
                        }
                    });
                    await Task.Delay(TimeSpan.FromHours(1));
                }


            });
#pragma warning restore CS4014 
        }

        public async Task Attach(IObserver obs)
        {
            SubscribeList.Add(obs);
        }

        public async Task Detach(IObserver obs)
        {
            SubscribeList.Remove(obs);
        }

        public async Task Notify(string msg, string raidTitle, string guildId)
        {
            SubscribeList.ForEach(async o => await o.Update(msg, raidTitle,guildId));
        }
    }
}
