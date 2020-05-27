using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBot.Service
{
    class KeyService
    {
        private DBHelper Db = new DBHelper();
        private int[] array;
        private List<string> DungeonNames = new List<string>();
        public KeyService()
        {
            DungeonNames.Add("Atal'Dazar");
            DungeonNames.Add("Freehold");
            DungeonNames.Add("King's Rest");
            DungeonNames.Add("Shrine of the Storm");
            DungeonNames.Add("Siege of Boralus");
            DungeonNames.Add("Temple of Sethraliss");
            DungeonNames.Add("The Motherlode");
            DungeonNames.Add("The Underrot");
            DungeonNames.Add("Tol Dagor");
            DungeonNames.Add("Waycrest Manor");
            DungeonNames.Add("The MOTHERLODE!!!	");
            DungeonNames.Add("Operation: Mechagon Junkyard");
            DungeonNames.Add("Operation: Mechagon Workshop");


        }
        public void CreateKeyFromStringAndSave(string input, string userName)
        {
            array = new int[2] { 100, 100 };
            var keyLevel = 0;
            var splitIndex = FindIndexOfFirstNumber(input);
            if (array[0] == 100)
                throw new FormatException("You need to specify keylevel");
            if (splitIndex != 0)
            {
                input = input.Substring(0, splitIndex);
                input = input.Trim();
                input = CheckIfDungeonNameIsInList(input);
            }
            string SamletKeyLevel = string.Empty;
            if (array[1] != 100)
                SamletKeyLevel = array[0].ToString() + array[1].ToString();
            else
            {
                 SamletKeyLevel = array[0].ToString();
            }

            var key = new KeyStone { Key = input, KeyLevel = Convert.ToInt32(SamletKeyLevel), UserName = userName, ExpireDate = Helpers.FindNextWednesday() };
            CheckIfKeyAuthorExists(key);
            Db.Create(key);
        }
        public List<KeyStone> GetAllKeys()
        {
            var list = Db.GetAll<KeyStone>();
            var returnList = new List<KeyStone>();
            list.ForEach(o => { if (o.ExpireDate.Date <= DateTime.Now.Date) Db.Delete(o); else returnList.Add(o); });
            return returnList;

        }

        public bool CheckIfKeyAuthorExists(KeyStone key)
        {
            var list = Db.GetAll<KeyStone>();
            var keyCheck = list.Where(o => o.UserName == key.UserName).FirstOrDefault();
            if (keyCheck != null)
            {
                Db.Delete(keyCheck);
                return true;
            }
            return false;
        }

        private int FindIndexOfFirstNumber(string input)
        {
            var returnTal = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (Int32.TryParse(input[i].ToString(), out int s))
                {
                    returnTal = i;
                    array[0] = s;
                    if (input.Length > i+1)
                    {
                        if (int.TryParse(input[i + 1].ToString(), out s))
                        {
                            array[1] = s;
                        }
                    }
                    break;
                }
            }

            return returnTal;
        }



        private string CheckIfDungeonNameIsInList(string input)
        {
            var returnString = DungeonNames.Where(o => o.ToLower().Contains(input.ToLower()) || o.ToLower().StartsWith(input.ToLower()) || o.ToLower().EndsWith(input.ToLower())).FirstOrDefault();
            if (returnString != null)
                return returnString;
            else
                return input;
        }
    }
}
