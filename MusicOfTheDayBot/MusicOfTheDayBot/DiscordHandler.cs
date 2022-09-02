using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOfTheDayBot
{
    public class DiscordHandler
    {

        private DiscordSocketClient _client;

        public bool SendMessage(string message, string channelID)
        {

            //Debug
            Console.WriteLine(message);

            return true;
        }
    }
}
