using DiscordBot.Models;
using DiscordBot.Models.Observer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Service
{
    class LogMessageService:ILogMessageSubject
    {
        private static List<IPlayerObserver> ObserverListe = new List<IPlayerObserver>();
        public async Task CreateLogEntry(string message, string stackTrace)
        {
            var msg = new LogMessage { EventTime = DateTime.Now, MsgText = message,StackTrace = stackTrace };
            var db = new DBHelper();
            db.Create(msg);
            await Notify(msg);
        }

        public async Task Attach(IPlayerObserver obs)
        {
            ObserverListe.Add(obs);
        }

        public async Task Detach(IPlayerObserver obs)
        {
            ObserverListe.Remove(obs);
        }

        public async Task Notify(LogMessage msg)
        {
            ObserverListe.ForEach(o => o.Update(msg));
        }
    }
}
