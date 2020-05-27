using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Service;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class CommandHandler
    {
        private DiscordSocketClient Client;
        private Discord.Commands.CommandService Commands;

        public CommandHandler(DiscordSocketClient client, Discord.Commands.CommandService cmd)
        {
            Client = client;
            Commands = cmd;
             InstallCommandsAsync();
        }

        public async Task InstallCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;
            Client.UserJoined += UserJoined;
            await Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        public async Task UserJoined(SocketGuildUser user)
        {
            try
            {
                var channel = await user.GetOrCreateDMChannelAsync();
                var emb = new EmbedBuilder();
                emb.WithTitle("Welcome amigo!").AddField(user.Username, "Welcome to the guild amigo. I am in charge of making heroics raids. Go to the bot channel to get started or DM me !CMD for help. Have a nice day");
                await channel.SendMessageAsync("", false, emb.Build());
            }
            catch(Exception e)
            {
                var ms = new LogMessageService();
                await ms.CreateLogEntry(e.Message, e.StackTrace);
            }

        }
        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null)
                return;
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos)) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) || message.Author.IsBot)
                return;

            var context = new SocketCommandContext(Client, message);

            await Commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}
