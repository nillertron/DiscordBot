using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Service
{
    class SignupService
    {
        private DBHelper db = new DBHelper();
        public SignUp SignUp(string input, string discordUser, string guildId)
        {
            input = input.Trim();
            var splitIndex = Helpers.FindBindingInString(input, '-');
            var raidId = 0;
            var player = new Player();

            if(splitIndex != -1)
            {
                var name = input.Substring(0, splitIndex).Trim();
                player = db.GetPlayerFromCharName(name,guildId);
                input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).Trim();
                raidId = Convert.ToInt32(input);
                 
            }
            else
            {
                raidId = Convert.ToInt32(input);
                player = db.GetSpecicPlayer(discordUser,guildId);

            }
            var dbSignUp = db.GetSpecificSignUp(raidId, discordUser);
            var raid = db.GetSpecificRaid(raidId,guildId);
            if (raid == null)
                throw new FormatException("Raid ID doesn't exist.");
            else if (!db.GetSpecificRaid(raidId,guildId).RaidActive)
                throw new FormatException("Raid is not active");
            else if (raid.RaidDay.Subtract(TimeSpan.FromMinutes(30)) <= DateTime.Now)
                throw new FormatException("Signup for this raid is closed");
            else if (dbSignUp != null)
                throw new FormatException("You have already signed up for this raid.");




            var signup = new SignUp { GuildId=guildId, PlayerId = player.Id, RaidId = raidId };
            db.Create(signup);

            return signup;
        }

        public List<List<SignUp>> SplitGroups(int raidId, ref List<int> avg, string guildId)
        {


            var UberListe = new List<List<SignUp>>();
            var signupList = db.GetAllSignUps(raidId,guildId);
            if (signupList.Count == 0)
                throw new FormatException("No signups for the given raid id");
            int antalLister = 1;
            antalLister += signupList.Count / 30;

            for (int i = 0; i < antalLister; i++)
                UberListe.Add(new List<SignUp>());
            var avgListe = new List<int>();
            var averageIlvl = -1;
            bool tanks = false;
            bool healers = false;
            bool rl = false;
            int attempts = 0;
            bool memberDifference = false;
            var rnd = new Random();
            var rlList = GetRaidLeaders(signupList);
            rlList.ForEach(o => signupList.Remove(o));
            do
            {
                if (attempts >= 10000)
                {
                    throw new FormatException("Couldn't make any valid combination");
                }
                //reset stuff
                for (int f = 0; f < UberListe.Count; f++)
                {
                    UberListe[f].Clear();
                }
                avgListe.Clear();
                averageIlvl = -1;
                tanks = false;
                healers = false;
                rl = false;
                memberDifference = false;
                int count = 0;
                //Assign raidleaders
                rlList.ForEach(o =>
                {
                    if (count > UberListe.Count)
                        count = 0;
                    UberListe[count].Add(o);
                    count++;
                });
                //assign randomly
                signupList.ForEach(o =>
                {
                    UberListe[rnd.Next(0, UberListe.Count)].Add(o);
                });
                //Add all ilvls to the list
                UberListe.ForEach(o =>
                {

                    avgListe.Add(GetAvgIlvl(o));
                });
                //Skal lave en liste af differencer for at kunne sammenligne hvis det skal være over 2 grupper
                if (UberListe.Count == 2)
                {
                    averageIlvl = avgListe[0] - avgListe[1];
                    //if (GetAmountOfTanks(UberListe[0]) >= 1 && GetAmountOfTanks(UberListe[1]) >= 1)
                        if (GetAmountOfTanks(UberListe[0]) - GetAmountOfTanks(UberListe[1]) >= -1 && GetAmountOfTanks(UberListe[0]) - GetAmountOfTanks(UberListe[1]) <= 1)
                            tanks = true;
                    //if (GetAmountOfHealers(UberListe[0]) >= 2 && GetAmountOfHealers(UberListe[1]) >= 2)
                        if (GetAmountOfHealers(UberListe[0]) - GetAmountOfHealers(UberListe[1]) >= -1 && GetAmountOfHealers(UberListe[0]) - GetAmountOfHealers(UberListe[1]) <= 1)
                            healers = true;
                    if (UberListe[0].Count - UberListe[1].Count < 3 && UberListe[0].Count - UberListe[1].Count >= 0)
                        memberDifference = true;
                    
                }
                else if (UberListe.Count == 3)
                {
                    averageIlvl = avgListe[0] - avgListe[1] - avgListe[2];
                    if (GetAmountOfTanks(UberListe[0]) >= 2 && GetAmountOfTanks(UberListe[1]) >= 2 && GetAmountOfTanks(UberListe[2]) >= 2)
                        tanks = true;
                    if (GetAmountOfHealers(UberListe[0]) >= 2 && GetAmountOfHealers(UberListe[1]) >= 2 && GetAmountOfHealers(UberListe[2]) >= 2)
                        healers = true;
                }
                else
                {
                    break;
                }
                attempts++;

            } while (averageIlvl > 5 || averageIlvl < 0 || !tanks || !healers || !memberDifference);
            foreach (var l in avgListe)
                avg.Add(l);
            return UberListe;
        }
        public void DeleteSignup(int raidId, string discName)
        {
            var signup = db.GetSpecificSignupForDeletion(raidId, discName);
            db.Delete(signup);
        }
        private int GetAvgIlvl(List<SignUp> playerListe)
        {
            var avgIlvl = 0;
            playerListe.ForEach(o =>
            {
                var p = db.GetSpecicPlayer(o.PlayerId);
                
                avgIlvl += p.Ilvl;
            });
            avgIlvl = avgIlvl / playerListe.Count;
            return avgIlvl;
        }
        private int GetAmountOfTanks(List<SignUp> playerListe)
        {
            var tanks = 0;
            playerListe.ForEach(o =>
            {
                var p = db.GetSpecicPlayer(o.PlayerId);
                if(p.Roles.Count == 0)
                p.FillRoleList();
                if (p.Roles.Contains(Role.Tank))
                    tanks++;
            });
            return tanks;
        }
        private List<SignUp> GetRaidLeaders(List<SignUp> playerListe)
        {
            var returnList = new List<SignUp>();
            playerListe.ForEach(o =>
            {

                    var p = db.GetSpecicPlayer(o.PlayerId);
                    if (p.Roles.Count == 0)
                        p.FillRoleList();
                    if (p.Roles.Contains(Role.RaidLeader))
                        returnList.Add(o);
               
            });
            return returnList;
        }
        private int GetAmountOfHealers(List<SignUp> playerListe)
        {
            var healers = 0;
            playerListe.ForEach(o =>
            {
                var p = db.GetSpecicPlayer(o.PlayerId);
                if (p.Roles.Count == 0)
                    p.FillRoleList();
                if (p.Roles.Contains(Role.Healer))
                    healers++;
            });
            return healers;
        }
        public List<SignUp> GetAll()
        {
            return db.GetAllSignUps();
        }

        public List<SignUp> GetAllForSpecificRaid(int id, string guildId)
        {
            return db.GetAllSignUps(id,guildId);
        }

        public List<Player> FindPlayersWithNoRoles()
        {
            var list = db.GetAllPlayers();
            list.ForEach(o =>
            {
                var roles = db.GetAllRoleLines(o.Id);
                roles.ForEach(x => o.Roles.Add((Role)x.RoleId));
            });
            return list.Where(x => x.Roles.Count == 0).ToList();

        }
    }
}
