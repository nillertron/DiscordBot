using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Service
{
    class RoleService
    {
        private DBHelper db = new DBHelper();
        private List<string> RoleList = new List<string>();
        public RoleService()
        {
            RoleList.Add("Tank");
            RoleList.Add("Dps");
            RoleList.Add("Damage");
            RoleList.Add("Healer");
            RoleList.Add("Heal");
            RoleList.Add("Rl");
            RoleList.Add("Raid Leader");
            RoleList.Add("Raid Lead");
            RoleList.Add("RaidLeader");


        }

        public void AddRoles()
        {
            db.AddRoles();
        }

        public void AddRole(string input, string discUser, string guildId)
        {
            var player = new Player();
            input = input.Trim();
            if(Helpers.FindBindingInString(input,'-') != -1)
            {
                var index = Helpers.FindBindingInString(input, '-');
                var playerName = input.Substring(0, index).Trim();
                player = db.GetPlayerFromCharName(playerName,guildId);
                input = input.Substring(index + 1, input.Length - index - 1).Trim();
            }
            else
            {
                player = db.GetSpecicPlayer(discUser,guildId);
            }

            var splitIndex = Helpers.FindBindingInString(input, ',');
            var streng = "";
            if(splitIndex == -1)
            {
                player.Roles.Add(GetPlayerRoleFromString(input));
            }
            else
            {
                do {
                    streng = input.Substring(0, splitIndex).Trim();
                    player.Roles.Add(GetPlayerRoleFromString(streng));
                    input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).TrimStart();
                    splitIndex = Helpers.FindBindingInString(input, ',');

                } while (splitIndex != -1);

                player.Roles.Add(GetPlayerRoleFromString(input));

            }
            if (player.Roles.Count > 0)
            {
                var roleLines = db.GetAllRoleLines(player.Id);

                player.Roles.ForEach(o =>
                {
                    if (o == Role.RaidLeader)
                    {
                        var playerList = db.GetAllPlayers(player.UserId.ToString(), player.GuildId.ToString());
                        playerList.ForEach(o =>
                        {
                            var roleLines = db.GetAllRoleLines(o.Id);
                            if (!roleLines.Any(l => l.RoleId == (int)Role.RaidLeader))
                            {
                                db.Create(new RoleLine { PlayerId = o.Id, RoleId = (int)Role.RaidLeader });

                            }
                        });
                    }
                    else
                    {
                        var line = roleLines.Where(x => x.RoleId == (int)o).FirstOrDefault();
                        if (line == null)
                        {
                            var linje = new RoleLine { PlayerId = player.Id, RoleId = (int)o };
                            db.Create(linje);
                        }
                    }
                });
            }
            else
                throw new FormatException("Empty list");
        }

        public void RemoveRole(string input, string discUser,string guildId)
        {
            var player = new Player();
            input = input.Trim();
            if (Helpers.FindBindingInString(input, '-') != -1)
            {
                var index = Helpers.FindBindingInString(input, '-');
                var playerName = input.Substring(0, index).Trim();
                player = db.GetPlayerFromCharName(playerName,guildId);
                input = input.Substring(index + 1, input.Length - index - 1).Trim();
            }
            else
            {
                player = db.GetSpecicPlayer(discUser,guildId);
            }
            var splitIndex = Helpers.FindBindingInString(input, ',');
            var streng = "";
            if (splitIndex == -1)
            {
                player.Roles.Add(GetPlayerRoleFromString(input));
            }
            else
            {
                do
                {

                    streng = input.Substring(0, splitIndex).Trim();
                    player.Roles.Add(GetPlayerRoleFromString(streng));
                    input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).TrimStart();
                    splitIndex = Helpers.FindBindingInString(input, ',');

                } while (splitIndex != -1);
                player.Roles.Add(GetPlayerRoleFromString(input));

            }
            if (player.Roles.Count > 0)
            {
                var roleList = db.GetAllRoleLines(player.Id);

                roleList.ForEach(o =>
                {
                    if(o.RoleId == (int)Role.RaidLeader)
                    {
                        var player = db.GetSpecicPlayer(o.PlayerId);
                        var players = db.GetAllPlayers(player.UserId, player.GuildId);
                        players.ForEach(x =>
                        {
                            var roles = db.GetAllRoleLines(x.Id);
                            var role = roles.Where(s => s.RoleId == (int)Role.RaidLeader).FirstOrDefault();
                            if (role != null)
                                db.Delete(role);
                        });
                    }
                    else if (player.Roles.Contains((Role)o.RoleId))
                        db.Delete(o);
                });
            }
            else
                throw new FormatException("Empty list");
        }

        private Role GetPlayerRoleFromString(string input)
        {
            input = input.Trim();
            input = RoleList.Where(o => o.ToLower().Contains(input.ToLower()) || o.ToLower().StartsWith(input.ToLower()) || o.ToLower().EndsWith(input.ToLower())).FirstOrDefault();
            if (input == null)
                throw new FormatException("Role not found!");
            else if (input == "Tank")
                return Role.Tank;
            else if (input == "Dps" || input == "Damage")
                return Role.DPS;
            else if (input == "Rl" || input == "Raid Leader" || input == "Raid Lead" || input == "RaidLeader")
                return Role.RaidLeader;
            else
                return Role.Healer;
        }

    }
}
