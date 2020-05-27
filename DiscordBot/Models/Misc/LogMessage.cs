using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    class LogMessage
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string StackTrace { get; set; }
        public string MsgText { get; set; }
        public DateTime EventTime { get; set; }
    }
}
