using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API;
using Discord.WebSocket;

namespace DiscordBot.Service
{
    class PlayerService
    {
        private DBHelper db = new DBHelper();
        private BlizzardService bService = new BlizzardService();
        private List<string> RoleList = new List<string>();
        private List<string> Classes = new List<string>();
        public PlayerService()
        {
            RoleList.Add("Tank");
            RoleList.Add("Dps");
            RoleList.Add("Damage");
            RoleList.Add("Healer");
            RoleList.Add("Heal");

            Classes.Add("Warr");
            Classes.Add("Warrior");
            Classes.Add("War");

            Classes.Add("Pala");
            Classes.Add("Paladin");

            Classes.Add("Hunter");

            Classes.Add("Monk");


            Classes.Add("Rogue");

            Classes.Add("Priest");

            Classes.Add("Sham");
            Classes.Add("Shaman");
            Classes.Add("Shammy");

            Classes.Add("Mage");

            Classes.Add("Druid");
            Classes.Add("Dudu");


            Classes.Add("Lock");
            Classes.Add("Warlock");

            Classes.Add("Dh");
            Classes.Add("Demon Hunter");

            Classes.Add("Dk");
            Classes.Add("Death Knight");

        }
        public async Task<Player> RegisterPlayer(string input, string discordName, string discordId, string guildId)
        {
            input = input.Trim();
            var splitindex = Helpers.FindBindingInString(input, '-');
            var name = input.Substring(0, splitindex).Trim();

            input = input.Substring(splitindex+1, input.Length-splitindex-1).TrimStart();
            var role = new List<Role>();
            splitindex = Helpers.FindBindingInString(input, '-');
            var stringRole = input.Substring(0, splitindex).Trim();
            if(Helpers.FindBindingInString(stringRole,',') != -1)
            {
                do
                {
                    splitindex = Helpers.FindBindingInString(input, ',');
                    if (splitindex == -1)
                        break;
                    stringRole = input.Substring(0, splitindex).Trim();
                    role.Add(GetPlayerRoleFromString(stringRole));
                    input = input.Substring(splitindex + 1, input.Length - splitindex - 1).TrimStart();
                } while (splitindex != -1);
            }

                splitindex = Helpers.FindBindingInString(input, '-');
                stringRole = input.Substring(0, splitindex).Trim();
                role.Add(GetPlayerRoleFromString(stringRole));
                input = input.Substring(splitindex + 1, input.Length - splitindex - 1).TrimStart();
                var realm = input.Trim();
            var pcheck = db.GetPlayerFromCharAndRealmName(name,realm);
            if (pcheck != null)
                throw new FormatException("Player already exists");

            var p = await bService.GetCharacter(realm.ToLower(), name.ToLower());
            if (p.gear == null || p.Class == null)
                throw new FormatException("Character not found on Raider.io");
            var player = new Player { UserId=discordId, GuildId=guildId, MythicScore=p.mythic_plus_scores.All, Realm=realm, CloakLevel = p.gear.corruption.cloakRank, PClass=GetPlayerClassFromString(p.Class), DiscordName = discordName, Ilvl = p.gear.item_level_equipped, IngameName = name, Roles = role, Main = false };

                db.Create(player);
                player.Roles.ForEach(o =>
                {
                    var roleLine = new RoleLine { PlayerId = player.Id, RoleId = (int)o };
                    db.Create(roleLine);
                });
            
            return player;
        }

        internal void SetMain(string charName, string discId, string guildId)
        {
            charName = charName.Trim();
            var liste = db.GetAllPlayers(discId,guildId);
            var mainliste = liste.Where(o => o.Main == true).ToList();
            var toon = liste.Where(o => o.IngameName.ToLower() == charName.ToLower()).FirstOrDefault();
            if (toon == null)
                throw new FormatException("Character not found");
            mainliste.ForEach(o =>
            {
                o.Main = false;
                db.Update(o);
            });
            toon.Main = true;
            db.Update(toon);

        }

        public List<Player> GetMyCharacters(string discId, string guildId)
        {
            var list = db.GetAllPlayers(discId, guildId);
            list.ForEach(o => o.FillRoleList());
            if (list.Count == 0)
                throw new FormatException("You've got no characters registered in your discord tag");
            return list;
        }
        public List<Player> GetAll(string guildId)
        {
            var list = db.GetAllPlayers(guildId);
            list.ForEach(o => o.FillRoleList());
            return list;
        }
        public void DeletePlayer(string playerName, string discId, string guildId)
        {
            playerName = playerName.Trim();
            var player = db.GetPlayerFromCharName(playerName,guildId);
            if (player.IngameName.ToLower() == playerName.ToLower())
            {
                if(player.UserId == discId && player.GuildId == guildId)
                {
                    var rolelist = db.GetAllRoleLines(player.Id);
                    var signupList = db.GetAllSignUps(player.Id, true);
                    if (rolelist.Count > 0)
                        rolelist.ForEach(o => db.Delete(o));
                    if (signupList.Count > 0)
                        signupList.ForEach(o => db.Delete(o));
                    db.Delete(player);
                }
                else
                    throw new Exception("No authorization :(");

            }
            else
                throw new Exception("Wrong character name");

        }
        public Player GetSpecificPlayer(int playerId)
        {
            var player = db.GetSpecicPlayer(playerId);
            player.FillRoleList();
            return player;
        }
        public async Task Update(string playerName, string guildId)
        {
            var player = db.GetPlayerFromCharName(playerName, guildId);
            string realm = "kazzak";
            if (player != null)
            {
                if(player.Realm != null)
                {
                    realm = player.Realm.ToLower();
                }
                try
                {
                    var values = await bService.GetCharacter(realm, player.IngameName.ToLower());
                    player.Ilvl = values.gear.item_level_equipped;
                    player.CloakLevel = values.gear.corruption.cloakRank;
                    player.MythicScore = values.mythic_plus_scores.All;
                    db.Update(player);

                    
                }
                catch(Exception e)
                {
                    var lm = new LogMessageService();
                    await lm.CreateLogEntry(e.Message, e.StackTrace);
                    throw new FormatException("Char not found");
                }


            }
        }
        public async Task UpdateAll()
        {
#pragma warning disable CS4014 
            Task.Run(async() =>
            {
                while(true)
                {
                    var player = db.GetAllPlayers();
                    string realm = "kazzak";
                    player.ForEach(async o =>
                    {
                        if (o.Realm != null)
                        {
                            realm = o.Realm.ToLower();
                        }
                        try
                        {
                            var values = await bService.GetCharacter(realm, o.IngameName.ToLower());
                            o.PClass = GetPlayerClassFromString(values.Class);
                            o.Ilvl = values.gear.item_level_equipped;
                            o.CloakLevel = values.gear.corruption.cloakRank;
                            o.MythicScore = values.mythic_plus_scores.All;
                            db.Update(o);

                        }
                        catch (Exception e)
                        {

                        }
                        realm = "kazzak";
                    });
                    await Task.Delay(TimeSpan.FromDays(1));
                }

            });
#pragma warning restore CS4014 

        }

        public async Task UpdatePlayerGuildAndIds(IReadOnlyCollection<SocketGuildUser> userList)
        {
            var list = db.GetAllPlayers();
            var guildId = "";
            list.ForEach(o =>
            {
                var ulist = userList.GetEnumerator();
                while (ulist.MoveNext())
                {
                    var user = ulist.Current;
                    if (user.Username + "#" + user.Discriminator == o.DiscordName)
                    {
                        o.UserId = user.Id.ToString();
                       guildId = o.GuildId = user.Guild.Id.ToString();
                        db.Update(o);
                        break;
                    }
                }
            });
            var raids = db.GetAllRaids();
            raids.ForEach(o =>
            {
                o.GuildId = guildId;
                db.Update(o);
            });
            var subs = db.GetAllSubscribedChannels();
            subs.ForEach(o =>
            {
                o.GuildId = guildId;
                db.Update(o);
            });
            var signUps = db.GetAllSignUps();
            signUps.ForEach(o =>
            {
                o.GuildId = guildId;
                db.Update(o);
            });
        }
        private Role GetPlayerRoleFromString(string input)
        {
            input = RoleList.Where(o => o.ToLower().Contains(input.ToLower()) || o.ToLower().StartsWith(input.ToLower()) || o.ToLower().EndsWith(input.ToLower())).FirstOrDefault();
            if (input == null)
                throw new FormatException("Role not found!");
            else if (input == "Tank")
                return Role.Tank;
            else if (input == "Dps" || input == "Damage")
                return Role.DPS;
            else
                return Role.Healer;
        }

        private Class GetPlayerClassFromString(string input)
        {
            input = input.Trim();
            input = Classes.Where(o => o.ToLower().Contains(input.ToLower()) || o.ToLower().StartsWith(input.ToLower()) || o.ToLower().EndsWith(input.ToLower())).FirstOrDefault();
            if (input == null)
                throw new FormatException("Class not found!");
            else if (input == "Warr" || input == "War" || input == "Warrior")
                return Class.Warrior;
            else if (input == "Pala" || input == "Paladin")
                return Class.Paladin;
            else if (input == "Hunter")
                return Class.Hunter;
            else if (input == "Rogue")
                return Class.Rogue;
            else if (input == "Priest")
                return Class.Priest;
            else if (input == "Sham" || input == "Shaman" || input == "Shammy")
                return Class.Shaman;
            else if (input == "Mage")
                return Class.Mage;
            else if (input == "Lock" || input == "Warlock")
                return Class.Warlock;
            else if (input == "Dh" || input == "Demon Hunter")
                return Class.Demon_Hunter;
            else if (input == "Monk")
                return Class.Monk;
            else if (input == "Druid" || input == "Dudu")
                return Class.Druid;
            else
                return Class.Death_Knight;
        }
        
    }
}
