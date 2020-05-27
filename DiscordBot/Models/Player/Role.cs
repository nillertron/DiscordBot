using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public enum Role
    {
        DPS,
        Tank,
        Healer,
        RaidLeader
    }
    class role
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Role Role { get; set; }
    }
}
