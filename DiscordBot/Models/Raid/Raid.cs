using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    class Raid
    {      
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string RaidTitle { get; set; }

        public DateTime RaidDay { get; set; }

        public bool RaidActive { get; set; }

        public string CreatedByUser { get; set; }
        public string GuildId { get; set; }

        public DateTime LastNotified { get; set; } = DateTime.Now.AddDays(7);

        public bool GroupsSorted { get; set; }

        public Raid()
        {

            
        }
    }
}
