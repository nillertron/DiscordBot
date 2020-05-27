using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models.Observer
{
    interface IPlayerObserver
    {
        Task Update(LogMessage msg);
    }
}
