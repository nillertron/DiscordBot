using Discord;
using Discord.Commands;
using DiscordBot.Module;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
    public enum SubscribeType
    {
        Raid
    }
    public class SubscribedChannel : IObserver
    {
        [PrimaryKey]
        public string channelId { get; set; }
        public string GuildId { get; set; }
        public SubscribeType Type { get; set; }
        
        public async Task Update(string message, string raidTitle, string guildId)
        {
            if(this.GuildId == guildId)
            {
                var mm = new SubscribedChannelModule();
                await mm.SendMessage(Convert.ToUInt64(channelId), message, raidTitle);
            }

        

        }
    }
}
