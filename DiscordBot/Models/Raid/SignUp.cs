using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public class SignUp
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PlayerId { get; set; }

        public string GuildId { get; set; }
       
        public int RaidId { get; set; }
        public DateTime ExpireDate { get; set; }

    }
}
