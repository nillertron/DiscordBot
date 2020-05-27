using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    class Command
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Cmd { get; set; }
        public string CmdText { get; set; }

    }
}
