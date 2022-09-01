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

        public Logic()
        {
            _library = FileHandler.GetAllSongs();
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

    }
}
