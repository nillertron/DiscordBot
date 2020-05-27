using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Service
{
    class CommandService
    {
        private DBHelper db = new DBHelper();
        public async Task<List<Command>> GetAll()
        {
            return db.GetAllCommands();
        }
        public async Task Create(string input)
        {
            input = input.Trim();

            var splitIndex = Helpers.FindBindingInString(input, '-');
            var cmdName = input.Substring(0, splitIndex).Trim();
            input = input.Substring(splitIndex + 1, input.Length - splitIndex - 1).Trim();

            var cmd = input;

            var command = new Command { Cmd = cmdName, CmdText = cmd };
            db.Create(command);
        }
        public async Task Delete(int id)
        {
            var cmd = new Command { Id = id };
            db.Delete(cmd);
        }
        public async Task Update(string input)
        {
            input = input.Trim();

            var splitIndex = Helpers.FindBindingInString(input, '-');
            var id = Convert.ToInt32(input.Substring(0, splitIndex).Trim());
            input = await Helpers.SplitString(input, splitIndex);

            splitIndex = Helpers.FindBindingInString(input, '-');
            var cmdName = await Helpers.FindValueInString(input, splitIndex);
            input = await Helpers.SplitString(input,splitIndex);

            var cmdText = input;

            var cmd = new Command { Id = id, CmdText = cmdText, Cmd = cmdName };
            db.Update(cmd);
        }
        
        public async Task CreateExistingCommands()
        {
            var cmdList = new List<Command> {

            new Command{ Cmd = "!RegisterPlayer CharacterName - Role1,Role2,Role3 - Realm", CmdText="You can register all your characters, just make sure spelling is correct, it searches raider.io for info"},
            new Command{ Cmd = "!SetMain Character", CmdText="Set your main character, makes it easier for you to sign for raids"},
            new Command{ Cmd = "!Update CharacterName", CmdText="Updates itemlevel and cloak level from raider.io"},
            new Command{ Cmd = "!GetMyCharacters", CmdText="Get a list of your registered characters."},
            new Command{ Cmd = "!GetAllPlayers", CmdText="Retreive a list of all registred players"},
            new Command{ Cmd = "!DeleteCharacter CharacterName", CmdText="Delete character"},
            new Command{ Cmd = "!Raids", CmdText="Retrieves a list of all active raids."},
            new Command{ Cmd = "!CreateRaid Raid_Title - MM/DD/YYYY HH:MM", CmdText="Creates a new raid with the title you specify and on the date."},
            new Command{ Cmd = "!EditRaid Raid_Title - Raid_Id - MM/DD/YYYY HH:MM", CmdText="Edits a raid"},
            new Command{ Cmd = "!DeleteRaid Raid_Id", CmdText="Sets a raid inactive"},
            new Command{ Cmd = "!Signup !!!optional character name, if its NOT your character SET as main!!! - RaidId", CmdText="Signs to the specified raid."},
            new Command{ Cmd = "!GetSignUpsFor RaidId", CmdText="Gets all the signups for a specific raid"},
            new Command{ Cmd = "!SplitGroups RaidId", CmdText="Splits the groups evenly and as close to the same average ilvl as possible for a given raid."},
            new Command{ Cmd = "!DeleteSignUp - RaidId", CmdText="Deletes signup for a given raid"},
            new Command{ Cmd = "!AddRole !!!optional character name, if its NOT your character SET as main!!! - Role1,role2,role3", CmdText="Adds roles to your player"},
            new Command{ Cmd = "!RemoveRole !!!optional character name, if its NOT your character SET as main!!! - Role1,role2,role3", CmdText="Removes roles from your player"},
            new Command{ Cmd="!Keys", CmdText="Will retreive a list of keystones registered by users."},
            new Command{ Cmd = "!AddKey Key_name Key_level", CmdText="Will add a key to the list, or refresh your key if it changed and you already registered this week."}
            };
            cmdList.ForEach(o => db.Create(o));
        }
    }
}
