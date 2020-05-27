using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
    interface ISubject
    {
        public Task Attach(IObserver obs);
        public Task Detach(IObserver obs);
        public Task Notify(string msg, string raidTitle,string guildId);
    }
}
