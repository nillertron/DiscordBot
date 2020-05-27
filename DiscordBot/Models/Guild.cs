using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    class Guild
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public int MembersCount { get; set; }
    }
}
