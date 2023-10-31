using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class Program
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("report")]
        public async Task ReportUser(IGuildUser user, [Remainder] string reason)
        {
            var guild = Context.Guild;
            var channel = guild.GetTextChannel(123456789012345678); // Replace with your channel ID

            await channel.SendMessageAsync($"User {Context.User.Mention} is reporting {user.Mention} for: {reason}");
            await ReplyAsync("Report sent.");
        }
    }

    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public Bot()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;

            _commands.AddModuleAsync<Commands>(null);
        }

        public async Task StartAsync()
        {
            string token = "YOUR_BOT_TOKEN"; // Replace with your bot token
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine("Bot is connected.");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (!(message is IUserMessage userMessage) || message.Author.IsBot)
                return;

            int argPos = 0;
            if (!userMessage.HasStringPrefix("!", ref argPos) || userMessage.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, userMessage);
            await _commands.ExecuteAsync(context, argPos, null);
        }
    }

    public static void Main(string[] args)
    {
        new Bot().StartAsync().GetAwaiter().GetResult();
    }
}
