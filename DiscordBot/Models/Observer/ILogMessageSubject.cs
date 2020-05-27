using DiscordBot.Models.Observer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{
    interface ILogMessageSubject
    {
        Task Attach(IPlayerObserver obs);
        Task Detach(IPlayerObserver obs);
        Task Notify(LogMessage msg);
    }
}
