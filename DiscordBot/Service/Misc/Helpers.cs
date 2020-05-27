using Discord;
using Discord.Rest;
using Discord.Commands;
using DiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public enum DefinedEmojis
    {
        Stars,

    }
    public static class Helpers
    {
        public static DateTime FindNextWednesday()
        {
            var date = DateTime.Now.AddDays(1);


            for (int i = 0; i < 7; i++)
            {
                if (date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    break;
                }
                date = date.AddDays(1);

            }
            return date;
        }
        public static int FindBindingInString(string input, char CharToSearchFor)
        {
            int returnInt = -1;

            for(int i = 0; i < input.Length; i++)
            {
                if (input[i].ToString() == CharToSearchFor.ToString())
                {
                    returnInt = i;
                    break;
                }
            }

            return returnInt;
        }

        public static string RegisterPlayerInputFormatString { get => "Input format should be: Playername - Role - Realm"; }

        public static EmbedBuilder GetEmbed(string title, string subTitle, string content)
        {
            var embed = new EmbedBuilder().WithTitle(title).AddField(subTitle, content);
            return embed;
            
        }
        public static string GetRoleFromList(List<Role> input)
        {
            string v = "";
            input.ForEach(o => v += o.ToString() + " ");
            return v;
        }

        public static async Task ReactToMessage(RestUserMessage msg, DefinedEmojis ChosenEmoji)
        {
            string emojiString = "";
            switch(ChosenEmoji)
            {
                case DefinedEmojis.Stars:
                    emojiString = "\U0001F929";
                    break;

            }
            await msg.AddReactionAsync(new Emoji(emojiString));
        }
        public static async Task<string> SplitString(string input, int splitIndex)
        {
            input = input.Substring(splitIndex + 1, input.Length - 1 - splitIndex).Trim();
            return input;
        }

        public static async Task<string> FindValueInString(string input, int splitIndex)
        {
            input = input.Substring(0, splitIndex).Trim();
            return input;
        }

    }
}
