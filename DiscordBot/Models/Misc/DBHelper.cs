using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using SQLite;


namespace DiscordBot.Models
{
    class DBHelper
    {
        private static SQLiteConnection Conn;

        public DBHelper()
        {
            string dbPath = Path.Combine(Environment.CurrentDirectory, "Data.db3");
            if (!File.Exists(dbPath)) File.Create(dbPath);

            if(Conn == null)
            Conn = new SQLiteConnection(dbPath);
            //clearAlltables();
            CreateTables();
            
        }
        public void AddDummyPlayers(string guildId)
        {
            Conn.DeleteAll<Player>();
            var rnd = new Random();
            for(int i = 0; i<40; i++)
            {
                Conn.Insert(new Player { GuildId=guildId, Realm="Kazzak", CloakLevel = 15, DiscordName = "JohnDoe" + i, Ilvl = rnd.Next(455, 476), IngameName = "Smil", PClass = (Class)rnd.Next(0, 12)});
            }

        }

        public void AddDummySignups(string guildId)
        {
            Conn.DeleteAll<SignUp>();
            Conn.DeleteAll<RoleLine>();
            var rolelist = new Queue<int>();
            for (int i = 0; i < 33; i++)
                rolelist.Enqueue(0);
            for (int i = 0; i < 3; i++)
                rolelist.Enqueue(1);
            for (int i = 0; i < 4; i++)
                rolelist.Enqueue(2);
            int count = 1;
            var list = GetAllPlayers();
            list.ForEach(o =>
            {
                var signup = new SignUp { GuildId=guildId, PlayerId = o.Id, RaidId = 10 };
                if(count % 15 == 0)
                {
                    var rlRole = new RoleLine { PlayerId = o.Id, RoleId = 3 };
                    Create(rlRole);
                }
                var rol = new RoleLine { PlayerId = o.Id, RoleId = rolelist.Dequeue() };
                Create(rol);


                Create(signup);
                count++;
            });
        }

        public int GetRaidSignupCount(int raidId)
        {
            var sql = "Select * from SignUp where RaidId = ?";
            var execute = Conn.Query<SignUp>(sql, raidId);
            return execute.Count;
        }

        public void AddRoles()
        {
            var r1 = new role { Role = Role.RaidLeader };

            Create(r1);
        }
        public void DeleteSpecificSignup(int id)
        {
            
            var sig = new SignUp { Id = id };
            Delete(sig);
        }

        public List<T> GetAll<T>() where T : class, new()
        {        
            return Conn.Table<T>().ToList();
        }
        public List<KeyStone> GetAllKeyStones()
        {
            return Conn.Table<KeyStone>().ToList();
        }
        public List<SubscribedChannel> GetAllSubscribedChannels()
        {
            return Conn.Table<SubscribedChannel>().ToList();
        }
        public List<role> GetAllRoles()
        {
            return Conn.Table<role>().ToList();
        }
        public List<Command> GetAllCommands()
        {
            return Conn.Table<Command>().ToList();
        }
        public List<RoleLine> GetAllRoleLines()
        {
            return Conn.Table<RoleLine>().ToList();
        }
        public List<Player> GetAllPlayers()
        {
            return Conn.Table<Player>().ToList();
        }
        public List<Raid> GetAllRaids()
        {
            return Conn.Table<Raid>().ToList();
        }
        public List<Raid> GetAllRaids(string guildId)
        {
            return Conn.Query<Raid>("Select * from Raid where GuildId = ?", guildId);
        }
        public List<SignUp> GetAllSignUps()
        {
            return Conn.Table<SignUp>().ToList();
        }
        public List<SignUp> GetAllSignUps(int raidId,string guildId)
        {
            return Conn.Query<SignUp>("Select * from SignUp where GuildId = ? AND RaidId = ?", guildId, raidId).ToList();
        }
        public List<SignUp> GetAllSignUps(int playerId, bool isPlayer)
        {
            return Conn.Query<SignUp>("Select * from SignUp where PlayerId = ?", playerId).ToList();
        }
        public List<RoleLine> GetAllRoleLines(int playerId)
        {
            return Conn.Query<RoleLine>("Select * from RoleLine where PlayerId = ?", playerId).ToList();
        }
        public List<Player> GetAllPlayers(string discUserId, string guildId)
        {
            return Conn.Query<Player>("Select * from Player where UserId = ? AND GuildId = ?", discUserId, guildId).ToList();
        }
        public List<Player> GetAllPlayers(string guildId)
        {
            return Conn.Query<Player>("Select * from Player where GuildId = ?", guildId).ToList();
        }
        public void Create<T>(T Obj)
        {
            Conn.Insert(Obj);
        }

        public void Delete<T>(T obj)
        {
            Conn.Delete(obj);
        }
        public void Update<T>(T obj)
        {
            Conn.Update(obj);
        }
        public Raid GetSpecificRaid(int id, string guildId)
        {

            var obj = Conn.Query<Raid>("Select * from Raid where Id = ? AND GuildId = ?", id, guildId).FirstOrDefault();
            if (obj == null)
                throw new FormatException("Raid not found");
            return obj;
        }
        public role GetSpecificRole(int roleId)
        {

            var obj = Conn.Get<role>(roleId);
            if (obj == null)
                throw new FormatException("Role not found");
            return obj;
        }
        public Player GetSpecicPlayer(int id)
        {
            var obj = new Player();
            try
            {
                 obj = Conn.Get<Player>(id);
            }
            catch
            {

            }
            if (obj == null)
                throw new FormatException("Player not found");
            return obj;
        }
        /// <summary>
        /// Gets main characters from the discord tag.
        /// </summary>
        /// <param name="discordName">Discord tag</param>
        /// <returns></returns>
        public Player GetSpecicPlayer(string discordName, string guildId)
        {
            var sql = "Select * from Player where DiscordName = ? AND Main = true AND GuildId = ?";
            var obj = Conn.Query<Player>(sql,discordName,guildId).FirstOrDefault();
            if (obj == null)
                throw new FormatException("Player not found, did you make sure to !SetMain on your main character?");
            return obj;
        }
        public Player GetPlayerFromCharName(string charName, string guildId)
        {
            charName = charName.ToLower();
            var sql = "Select * from Player where LOWER(IngameName) = ? AND GuildId = ?";
            var obj = Conn.Query<Player>(sql, charName, guildId).FirstOrDefault();
            if (obj == null)
                throw new FormatException("Player not found, double check your spelling.");

            return obj;
        }
        public Player GetPlayerFromCharAndRealmName(string charName, string realmName)
        {
            charName = charName.ToLower();
            var sql = "Select * from Player where LOWER(IngameName) = ? AND LOWER(Realm) = ?";
            var obj = Conn.Query<Player>(sql, charName.ToLower(), realmName.ToLower()).FirstOrDefault();
            return obj;
        }
        /// <summary>
        /// Gets a list of players for discordname and loops through it to find out if any of the discordnames characters is signed up for the given raid id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="discordName"></param>
        /// <returns></returns>
        public SignUp GetSpecificSignUp(int id, string discordName)
        {
            var player = new Player();
                var sql = "Select * from Player where DiscordName = ?";
                var playerList = Conn.Query<Player>(sql, discordName);

            

            if (player == null)
                throw new FormatException("No player found. Register as player");
            var obj = new SignUp();
            for(int i = 0; i<playerList.Count; i++)
            {
              obj = Conn.Query<SignUp>("Select * from SignUp where PlayerId = ? AND RaidId = ?", playerList[i].Id, id).FirstOrDefault();
                if (obj != null)
                break;
            }
            return obj;
        }
        public SignUp GetSpecificSignupForDeletion(int id, string discordName)
        {
            var sql = "Select * from Player where DiscordName = ?";
            var playerList = Conn.Query<Player>(sql, discordName).ToList();
            if (playerList.Count == 0)
                throw new FormatException("No player found. Register as player");
            SignUp obj = null;
            playerList.ForEach(o =>
            {
                try
                {
                    if (obj == null)
                        obj = Conn.Query<SignUp>("Select * from SignUp where PlayerId = ? AND RaidId = ?", o.Id, id).FirstOrDefault();
                    
                }
                catch (Exception e) { }
            });
            return obj;
        }

        public void CreateTables()
        {
            Conn.CreateTable<KeyStone>();
            Conn.CreateTable<Player>();
            Conn.CreateTable<Raid>();
            Conn.CreateTable<SignUp>();
            Conn.CreateTable<role>();
            Conn.CreateTable<RoleLine>();
            Conn.CreateTable<Command>();
            Conn.CreateTable<SubscribedChannel>();
            Conn.CreateTable<Guild>();
            Conn.CreateTable<LogMessage>();



        }
        public void clearAlltables()
        {
            Conn.DeleteAll<KeyStone>();
            Conn.DeleteAll<Player>();
            Conn.DeleteAll<Raid>();
            Conn.DeleteAll<SignUp>();
            Conn.DeleteAll<role>();
            Conn.DeleteAll<RoleLine>();
            Conn.DeleteAll<Command>();

        }
    }
}
