using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using SQLite;
namespace DiscordBot
{
    class KeyStone
    {
       [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Key { get; set; }
        public int KeyLevel { get; set; }
        public string UserName { get; set; }
        [Ignore]
        public IUser User { get; set; }

        public DateTime ExpireDate { get; set; }

        public KeyStone()
        {

        }


    }
}
