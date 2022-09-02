using MusicOfTheDayBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        List<GameSongLibrary> _library;
        DiscordHandler discord;
        Random rng;
        CommandInterpreter commandInterpreter;

        public Logic()
        {
            _library = FileHandler.GetAllSongs();
            discord = new DiscordHandler();
            rng = new Random();
            commandInterpreter = new CommandInterpreter();
        }

        public void NewPost(string channelID, string game)
        {
            //TODO: Implement only game selected

            if(_library.Count == 0)
            {
                return;
            }

            GameSongLibrary? selectedLibrary;

            if(_library.Count == 1)
            {
                selectedLibrary = _library[0];
            }
            else
            {
                selectedLibrary = _library[rng.Next(0, _library.Count)];
            }

            Song selectedSong;

            if(selectedLibrary.Value.Songs.Count == 0)
            {

            }

            if(selectedLibrary.Value.Songs.Count == 1)
            {
                selectedSong = selectedLibrary.Value.Songs[0];
            }
            else
            {
                selectedSong = selectedLibrary.Value.Songs[rng.Next(0, selectedLibrary.Value.Songs.Count)];
            }

            string message = "";
            message += $"Der neue Song of the Day ist {selectedSong.Name} aus {selectedLibrary.Value.GameName}! \r\n";
            message += $"{selectedSong.YouTubeLink}";

            discord.SendMessage(message, channelID);
        }

        public void ListAllGames()
        {
            Console.Write("Vorhandene Games: ");
            foreach (var gameLib in _library)
            {
                Console.Write(gameLib.GameName + ", ");
            }
            Console.Write("\r\n");
        }

        public void ReadAll()
        {
            _library = FileHandler.GetAllSongs();
        }

        public void AddSong(string gameName, string songName, string youtubeLink)
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
                            Console.WriteLine("Song bereits vorhanden");
                            return;
                        }
                    }

                    gameLib.Songs.Add(new Song(songName, youtubeLink));
                    FileHandler.SaveNewList(gameLib);
                    Console.WriteLine("Song successfully added!");
                    break;
                }
            }

            if (!gameFound) Console.WriteLine($"No Game found with name: {gameName}");
        }

        public void RemoveSong(string gameName, string songName)
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
                        Console.WriteLine($"Song not found with name: {songName}");
                        return;
                    }

                    gameLib.Songs.Remove(toRemove.Value);
                    FileHandler.SaveNewList(gameLib);
                    Console.WriteLine("Song successfully removed!");
                    break;
                }
            }

            if (!gameFound) Console.WriteLine($"No Game found with name: {gameName}");
        }

        public void NewGame(string gameName)
        {
            GameSongLibrary gsl = new GameSongLibrary(FileHandler.GetFilePath(), gameName, new List<Song>());
            FileHandler.SaveNewList(gsl);
            Console.WriteLine("Game sucessfully added!");
            ReadAll();
        }

        public void RemoveGame(string gameName)
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
                Console.WriteLine("No Game found");
                return;
            }

            FileHandler.DeleteFile(fileName);
            Console.WriteLine("Game sucessfully removed");
            ReadAll();
        }

        //Debug
        public void NewCommand(string command)
        {
            //string command;
            List<string> args;
            commandInterpreter.ProcessCommand(command, out command, out args);

            switch (command.ToLower())
            {
                case "addgame":
                    break;
                case "removegame":
                    break;
                case "addsong":
                    break;
                case "changelink":
                    break;
                case "removesong":
                    break;
                case "post":
                    break;
                case "postrandom":
                    break;
                case "list":
                    break;
                case "listgames":
                    break;
                case "addschedule":
                    break;
                case "listschedules":
                    break;
                case "removeschedules":
                    break;
            }
        }

    }
}
