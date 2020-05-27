using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    class RoleLine
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int RoleId { get; set; }
    }
}
