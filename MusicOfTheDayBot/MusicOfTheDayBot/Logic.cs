using MusicOfTheDayBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicOfTheDayBot.DiscordHandler;

namespace MusicOfTheDayBot
{
    public class Logic
    {
        public struct Song
        {
            public string Name;
            public string YouTubeLink;

            public Song(string name, string youTubeLink)
            {
                Name = name;
                YouTubeLink = youTubeLink;
            }
        };
        public struct GameSongLibrary
        {
            public string FileName;
            public string GameName;
            public List<Song> Songs;

            public GameSongLibrary(string fileName, string gameName, List<Song> songs)
            {
                FileName = fileName;
                GameName = gameName;
                Songs = songs;
            }
        }
        public struct DiscordChannelInfo
        {
            public ulong ChannelID;
            public ulong GuildID;

            public DiscordChannelInfo(ulong channelID, ulong guildID)
            {
                ChannelID = channelID;
                GuildID = guildID;
            }
        }
        public struct LastPosted
        {
            public string GameName;
            public string SongName;
        }

        List<GameSongLibrary> _library;
        public DiscordHandler discord;
        Random rng;
        CommandInterpreter commandInterpreter;
        ScheduleLogic scheduler;

        //TODO: settings file
        private static int _lastPostedAmount = 30;      //later changeable in settings
        private static int _lastPostedTires = 20;       //later changeable in settings
        private static int _discordCharacterLimit = 2000;

        public Logic()
        {
            _library = FileHandler.GetAllSongs();
            discord = new DiscordHandler(this);
            rng = new Random();
            commandInterpreter = new CommandInterpreter();
            scheduler = new ScheduleLogic(this);
        }

        public void NewRandomPost(DiscordChannelInfo channelInfo)
        {
            //TODO: Implement only game selected

            if(FirstAprilFool(out string aprilGame, out string aprilSong, out string aprilLink))
            {
                string aprilMessage = "";
                aprilMessage += $"Der neue Song of the Day ist {aprilSong} aus {aprilGame}! \r\n";
                aprilMessage += $"{aprilLink}";

                discord.SendMessage(aprilMessage, channelInfo);
                return;
            }

            if(_library.Count == 0)
            {
                return;
            }

            List<LastPosted> lastPosteds = FileHandler.LoadLastPosted();

            int tries = 0;

            GameSongLibrary? selectedLibrary;
            Song? selectedSong;

            (selectedLibrary, selectedSong) = GetRandomSong();

            while (tries < _lastPostedTires)
            {
                (selectedLibrary, selectedSong) = GetRandomSong();

                if (selectedLibrary == null || selectedSong == null) return;

                if(!WasPosted(selectedLibrary.Value.GameName, selectedSong.Value, lastPosteds))
                {
                    break;
                }

                tries++;
            }

            if(tries >= _lastPostedTires)
            {
                (selectedLibrary, selectedSong) = GetRandomSong();
            }

            if (selectedLibrary == null || selectedSong == null) return;

            AddToLastPosted(selectedLibrary.Value, selectedSong.Value, lastPosteds);
            FileHandler.SaveLastPosted(lastPosteds);

            string message = "";
            message += $"Der neue Song of the Day ist {selectedSong.Value.Name} aus {selectedLibrary.Value.GameName}! \r\n";
            message += $"{selectedSong.Value.YouTubeLink}";

            discord.SendMessage(message, channelInfo);
        }

        public bool NewPost(string game, string songname, DiscordChannelInfo channelInfo, out string info)
        {
            GameSongLibrary? gsl = null;

            if(_library.Count == 0)
            {
                info = $"Song Library ist leer! Füge zuerst Spiele und Songs hinzu.";
                return false;
            }

            foreach(var lib in _library)
            {
                if (lib.GameName.ToLower().Equals(game.ToLower()))
                {
                    gsl = lib;
                    break;
                }
            }

            if(gsl.HasValue || gsl == null)
            {
                info = $"Kein Spiel mit dem Namen {game} gefunden!";
                return false;
            }

            foreach(var song in gsl.Value.Songs)
            {
                if (song.Name.ToLower().Equals(songname.ToLower()))
                {
                    string message = "";
                    message += $"Der neue Song of the Day ist {song.Name} aus {gsl.Value.GameName}! \r\n";
                    message += $"{song.YouTubeLink}";

                    discord.SendMessage(message, channelInfo);
                    info = "";
                    return true;
                }
            }

            info = $"Kein Song mit dem Namen {songname} in {game} gefunden!";
            return false;
        }

        public void ListAllGames(out string info)
        {
            info = "";
            info += "Vorhandene Gamelibraries: \r\n";
            foreach (var gameLib in _library)
            {
               info += gameLib.GameName + ", ";
            }
        }

        public void ListSongs(string game, bool withLink, out List<string> messages)
        {
            GameSongLibrary? gsl = null;
            messages = new List<string>();
            string info;

            foreach (var lib in _library)
            {
                if (lib.GameName.ToLower().Equals(game.ToLower()))
                {
                    gsl = lib;
                    break;
                }
            }

            if (!gsl.HasValue)
            {
                info = $"Kein Spiel mit dem Namen {game} gefunden!";
                messages.Add(info);
                return;
            }

            info = $"{gsl.Value.GameName}: \r\n";


            info += "```";

            foreach(Song song in gsl.Value.Songs)
            {
                string toAdd = $"{song.Name}\r\n";
                if (withLink)
                {
                    toAdd = $"{song.Name}, {song.YouTubeLink}\r\n";
                }

                info += toAdd;

                if(info.Length > _discordCharacterLimit - 20)
                {
                    info = info.Remove(info.Length - toAdd.Length);     //removes the last string to be sure the ``` fits in the char limit
                    info += "```";
                    messages.Add(info);

                    info = "```";       //start the new string
                    info += toAdd;      //add the song to the new string
                }
            }

            info += "```";
            messages.Add(info);
        }

        public void ReadAll()
        {
            _library = FileHandler.GetAllSongs();
        }

        public bool AddSong(string gameName, string songName, string youtubeLink, out string info)
        {
            bool gameFound = false;

            foreach(var gameLib in _library)
            {
                if (gameLib.GameName.ToLower().Equals(gameName.ToLower()))
                {
                    gameFound = true;

                    foreach(var song in gameLib.Songs)
                    {
                        //falls Song Name oder YouTube Link schon vohanden, nicht nochmal hinzufügen
                        if (song.Name.ToLower().Equals(songName.ToLower()) || song.YouTubeLink.Equals(youtubeLink))
                        {
                            info = "Song bereits vorhanden";
                            return false;
                        }
                    }

                    gameLib.Songs.Add(new Song(songName, youtubeLink));
                    FileHandler.SaveNewList(gameLib);
                    //Console.WriteLine("Song successfully added!");
                    break;
                }
            }

            if (!gameFound)
            {
                info = $"Kein Game gefunden mit dem Namen: {gameName}";
                return false;
            }

            info = "Song erfolgreich hinzugefügt!";
            return true;
        }

        public bool RemoveSong(string gameName, string songName, out string info)
        {
            bool gameFound = false;

            foreach (var gameLib in _library)
            {
                if (gameLib.GameName.ToLower().Equals(gameName.ToLower()))
                {
                    gameFound = true;
                    Song? toRemove = null;

                    foreach (var song in gameLib.Songs)
                    {
                        //falls Song Name oder YouTube Link schon vohanden, nicht nochmal hinzufügen
                        if (song.Name.ToLower().Equals(songName.ToLower()))
                        {
                            toRemove = song;
                            break;
                        }
                    }

                    if (toRemove == null)
                    {
                        info = $"Song nicht gefunden mit dem Namen: {songName}";
                        return false;
                    }

                    gameLib.Songs.Remove(toRemove.Value);
                    FileHandler.SaveNewList(gameLib);
                    
                    break;
                }
            }

            if (!gameFound)
            {
                info = $"Kein Game gefunden mit dem Namen: {gameName}";
                return false;
            }

            info = "Song successfully removed!";
            return true;
        }

        public bool NewGame(string gameName, out string info)
        {
            if (ContainsGame(gameName))
            {
                info = "Game ist bereits hinterlegt!";
                return false;
            }

            GameSongLibrary gsl = new GameSongLibrary(FileHandler.GetFilePath(), gameName, new List<Song>());
            if (FileHandler.SaveNewList(gsl))
            {
                ReadAll();
                info = "Game sucessfully added!";
                return true;
            }
            else
            {
                info = "Error creating file for the game!";
                return false;
            }
        }

        public bool RemoveGame(string gameName, out string info)
        {
            string? fileName = null;

            foreach(var gameLib in _library)
            {
                if (gameLib.GameName.ToLower().Equals(gameName.ToLower()))
                {
                    fileName = gameLib.FileName;
                }
            }

            if(fileName == null)
            {
                info = "Game wurde nicht gefunden";
                return false;
            }

            FileHandler.DeleteFile(fileName);
            
            ReadAll();

            info = "Game erfolgreich entfernt!";
            return true;
        }

        private bool ContainsGame(string gameName)
        {
            foreach(var gsl in _library)
            {
                if (gsl.GameName.ToLower().Equals(gameName.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private void GetCommands(out string info)
        {
            info = "Commands: \r\n";
            info += "!addgame \"[gameName]\" - Adds a new game \r\n";
            info += "!removegame \"[gameName]\" - removes a game \r\n";
            info += "!addsong \"[game]\" \"[songname]\" \"[youtubelink]\" - Adds a song and link to a game \r\n";
            info += "!changelink \"[game]\" \"[songname]\" \"[newyoutubelink]\" - changes the link to a song \r\n";
            info += "!removesong \"[game]\" \"[songname]\" - Removes a song from a game \r\n";


            info += "!post \"[game]\" \"[songname]\" [#channel (optional)] - posts a song manually \r\n";
            info += "!postrandom [#channel (optional)] - posts a randomly selected song \r\n";

            info += "!list \"[game]\" - Lists the songlibrary of a game \r\n";
            info += "!listgames - lists all games \r\n";
            info += "!listlink \"[game]\" - Lists the songlibrary and the corresponding link";

            info += "!addschedule [#channel] [time (e.g. 18:00)] \"[game(optional)]\" - adds a schedule for posting a song Sotd at 18:00 every day \r\n";
            info += "!listschedules - lists all schedules with their ScheduleID \r\n";
            info += "!removeschedule [id] - removes a schedule \r\n";
        }

        //Debug
        public bool NewCommand(DiscordMessageInfo message, out List<string> messages)
        {
            //string command;
            messages = new List<string>();
            List<string> args;
            commandInterpreter.ProcessCommand(message.Message, out string command, out args);
            string info = "";

            switch (command.ToLower())
            {
                case "addgame":
                    if(args.Count != 1)
                    {
                        info = "Command usage: !addgame \"[Game]\"";
                    }
                    NewGame(args[0], out info);
                    break;
                case "removegame":
                    if(args.Count != 1)
                    {
                        info = "Command usage: !removegame \"[Game]\"";
                    }
                    RemoveGame(args[0], out info);
                    break;
                case "addsong":
                    if(args.Count != 3)
                    {
                        info = "Command usage: !addsong \"[game]\" \"[songname]\" \"[youtubelink]\"";
                    }
                    AddSong(args[0], args[1], args[2], out info);
                    break;
                case "changelink":
                    break;
                case "removesong":
                    if(args.Count != 2)
                    {
                        info = "Command usage: !removesong \"[game]\" \"[songname]\"";
                    }
                    RemoveSong(args[0], args[1], out info);
                    break;
                case "post":
                    if(args.Count == 2)
                    {
                        NewPost(args[0], args[1], new DiscordChannelInfo() { GuildID = message.GuildID, ChannelID = message.ChannelID }, out info);
                    }
                    else if(args.Count == 3)
                    {
                        NewPost(args[0], args[1], new DiscordChannelInfo() { GuildID = message.GuildID, ChannelID = message.MentionedChannel }, out info);
                    }
                    else
                    {
                        info = "Command usage: !post \"[game]\" \"[songname]\" [#channel (optional)]";
                    }
                    break;
                case "postrandom":
                    if(args.Count > 1)
                    {
                        info = "Command usage: !postrandom [#channel (optional)]";
                    }
                    NewRandomPost(new DiscordChannelInfo() { GuildID = message.GuildID, ChannelID = message.MentionedChannel });
                    break;
                case "list":
                    if(args.Count != 1)
                    {
                        info = "Command usage: !list \"[game]\"";
                    }
                    ListSongs(args[0], false, out messages);
                    return true;
                //break;
                case "listlink":
                    if (args.Count != 1)
                    {
                        info = "Command usage: !listlink \"[game]\"";
                    }
                    ListSongs(args[0], true, out messages);
                    return true;
                case "listgames":
                    if(args.Count != 0)
                    {
                        info = "Command usage: !listgames";
                    }
                    ListAllGames(out info);
                    break;
                case "addschedule":
                    if(args.Count != 2)
                    {
                        info = "Command usage: !addschedule [#channel] [time (e.g. 18:00)]";
                    }
                    scheduler.AddSchedule(args[1], new DiscordChannelInfo() { GuildID = message.GuildID, ChannelID = message.MentionedChannel }, out info);
                    break;
                case "listschedules":
                    scheduler.GetAllSchedules(out info, discord);
                    break;
                case "removeschedule":
                    if(args.Count != 1)
                    {
                        info = "Command usage: !removeschedule [id]";
                    }
                    scheduler.RemoveSchedule(args[0], out info);
                    break;
                case "help":
                    GetCommands(out info);
                    break;
                default:
                    info = "";
                    messages.Add(info);
                    return false;
            }

            messages.Add(info);
            return true;
        }

        private bool WasPosted(string gameName, Song song, List<LastPosted> lastPosted)
        {
            foreach(var posted in lastPosted)
            {
                if (posted.GameName.Equals(gameName))
                {
                    if (posted.SongName.Equals(song.Name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private (GameSongLibrary?, Song?) GetRandomSong()
        {
            GameSongLibrary? selectedLibrary;
            Song selectedSong;

            if (_library.Count == 1)
            {
                selectedLibrary = _library[0];
            }
            else
            {
                selectedLibrary = _library[rng.Next(0, _library.Count)];
            }

            if (selectedLibrary.Value.Songs.Count == 0)
            {
                return (null, null);
            }

            if (selectedLibrary.Value.Songs.Count == 1)
            {
                selectedSong = selectedLibrary.Value.Songs[0];
            }
            else
            {
                selectedSong = selectedLibrary.Value.Songs[rng.Next(0, selectedLibrary.Value.Songs.Count)];
            }

            return (selectedLibrary, selectedSong);
        }

        private void AddToLastPosted(GameSongLibrary library, Song song, List<LastPosted> lastPosteds)
        {
            lastPosteds.Add(new LastPosted() { GameName = library.GameName, SongName = song.Name });
            if(lastPosteds.Count > _lastPostedAmount)
            {
                lastPosteds.RemoveAt(0);
            }
        }

        private bool FirstAprilFool(out string game, out string song, out string youtubelink)
        {
            game = "Xenoblade 1";
            song = "You will know our names - Kazoo Edition";
            youtubelink = "https://www.youtube.com/watch?v=4Y0Z1GxtfkU";

            //Check if it's 1st April
            if (DateTime.Now.Day == 1 && DateTime.Now.Month == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
