using Discord.WebSocket;
using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Service
{
    class GuildService
    {
        private DBHelper db = new DBHelper();
        public GuildService()
        {
            Program.Client.Ready += async() => 
            {
                await RegisterGuild(Program.Client.Guilds);
            };
        }
        public async Task RegisterGuild(IReadOnlyCollection<SocketGuild> guilds)
        {
            var list = guilds.GetEnumerator();
            while (list.MoveNext())
            {
                try
                {
                    var obj = new Guild { Id = list.Current.Id.ToString(), MembersCount = list.Current.Users.Count, Name = list.Current.Name };
                    var liste = db.GetAll<Guild>();
                    if (liste.Any(x => x.Id == obj.Id))
                        continue;
                    db.Create(obj);
                }
                catch (Exception e)
                {
                    var ms = new LogMessageService();
                    await ms.CreateLogEntry(e.Message, e.StackTrace);
                }

            }

        }
    }
}
