using Discord.Commands;
using DiscordBot.Models.Observer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
    interface IObserver
    {
        public Task Update(string msg, string raidTitle, string guildId);
    }
}
