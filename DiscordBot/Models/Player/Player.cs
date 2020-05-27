using Discord;
using DiscordBot.Models.Observer;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Models
{

    public enum Class
    {
        Warrior,
        Paladin,
        Hunter,
        Rogue,
        Priest,
        Shaman,
        Mage,
        Warlock,
        Monk, 
        Druid,
        Demon_Hunter,
        Death_Knight
    }
    class Player:IPlayerObserver
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double MythicScore { get; set; }
        public string IngameName { get; set; }
        public string DiscordName { get; set; }
        public bool Main { get; set; }
        public string Realm { get; set; }
        public string GuildId { get; set; }
        public string UserId { get; set; }
        private int _ILvl;
        public int Ilvl { get => _ILvl; set {
                if (value > 0)
                    _ILvl = value;
                else
                    throw new FormatException("Failed to convert ilvl. ");

            }
        }
        private int _CloakLevel;
        public int CloakLevel { get => _CloakLevel; set
            {
                if (value < 0 || value > 15)
                    throw new FormatException("Failed to convert cloak level, make sure to only write whole integers between 1 and 15");
                else _CloakLevel = value;
            } }
        public Class PClass { get; set; }
        [Ignore]
        public List<Role> Roles { get; set; } = new List<Role>();

        public void FillRoleList()
        {
            var db = new DBHelper();
            var list = db.GetAllRoleLines(Id);
            list.ForEach(o => Roles.Add((Role)o.RoleId));
        }

        public async Task Update(LogMessage msg)
        {
            var userChannel = await Program.Client.GetUser(Convert.ToUInt64(UserId)).GetOrCreateDMChannelAsync();
            var embBuilder = new EmbedBuilder();
            embBuilder.WithTitle(msg.MsgText).AddField(msg.Id + " - " + msg.EventTime.ToString(), msg.StackTrace);
            await userChannel.SendMessageAsync("",false,embBuilder.Build());
            
        }
    }
}
