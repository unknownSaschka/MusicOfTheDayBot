using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicOfTheDayBot.Logic;

namespace MusicOfTheDayBot
{
    public class DiscordHandler
    {
        private Logic _logic;
        private DiscordSocketClient? _client;

        private string token;

        public struct DiscordMessageInfo
        {
            public string Message;
            public ulong GuildID;
            public ulong ChannelID;
            public ulong UserID;

            public ulong MentionedChannel;
        }

        public DiscordHandler(Logic logic)
        {
            _logic = logic;

            try
            {
                token = File.ReadAllText("./token.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                token = "";
            }
            //Init().GetAwaiter().GetResult();
        }

        public async Task Init()
        {
            
            _client = new DiscordSocketClient();
            _client.MessageReceived += ReceiveMessage;
            _client.Connected += () => { 
                Console.WriteLine("Bot successfully connected!");
                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        ~DiscordHandler()
        {
            if(_client != null)
            {
                if(_client.ConnectionState == ConnectionState.Connected)
                {
                    _client.LogoutAsync();
                }

                _client.Dispose();
            }
        }

        public bool SendMessage(string message, DiscordChannelInfo channelInfo)
        {
            //Debug
            Console.WriteLine(message);

            if(_client == null)
            {
                Console.WriteLine("Discord Client is null");
                return false;
            }

            _client.GetGuild(channelInfo.GuildID).GetTextChannel(channelInfo.ChannelID).SendMessageAsync(message);
            return true;
        }

        public bool SendMessage(List<string> messages, DiscordChannelInfo channelInfo)
        {
            foreach (var message in messages)
            {
                SendMessage(message, channelInfo);
            }

            return true;
        }

        [RequireUserPermission(ChannelPermission.ManageChannels)]
        private Task ReceiveMessage(SocketMessage arg)
        {
            var channelGuild = arg.Channel as SocketGuildChannel;
            if(channelGuild == null)
            {
                return Task.CompletedTask;
            }

            var discordMessage = new DiscordMessageInfo()
            {
                Message = arg.Content,
                GuildID = channelGuild.Guild.Id,
                ChannelID = channelGuild.Id,
                UserID = arg.Author.Id,
                MentionedChannel = arg.MentionedChannels.FirstOrDefault(channelGuild).Id
            };

            if (_logic.NewCommand(discordMessage, out List<string> response))
            {
                foreach(var message in response)
                {
                    if(message.Length == 0) continue;
                    arg.Channel.SendMessageAsync(message);
                }
            }

            return Task.CompletedTask;
        }

        public string GetChannelName(ulong guildID, ulong channelID)
        {
            if (_client == null)
            {
                Console.WriteLine("Discord Client is null");
                return "";
            }

            var guild = _client.GetGuild(guildID);
            var channel = guild.GetChannel(channelID);
            if (channel == null) return "Don't exist anymore!";
            return channel.Name;
        }
    }
}
