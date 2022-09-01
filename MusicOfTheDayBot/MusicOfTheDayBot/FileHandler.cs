using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOfTheDayBot
{
    public class FileHandler
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
            public string GameName;
            public List<Song> Songs;

            public GameSongLibrary(string gameName, List<Song> songs)
            {
                GameName = gameName;
                Songs = songs;
            }
        }

        private static string _songFolder = "./songlists/";

        public static List<GameSongLibrary> GetAllSongs()
        {
            List<GameSongLibrary> songs = new List<GameSongLibrary>();

            foreach(string fileName in GetAllSongListFiles())
            {
                string gameName = GetGameName(fileName);
                List<Song> gameSongs = GetSongList(fileName);

                songs.Add(new GameSongLibrary(gameName, gameSongs));
            }

            return songs;
        }

        public static List<Song> GetSongList(string fileName)
        {
            List<Song> list = new List<Song>();

            foreach (var line in File.ReadLines(fileName))
            {
                //if line has a #, its the game name
                if (line.StartsWith("#")) continue;

                //line is build up: "[song name]|[youtube link]"
                if (line.Length == 0) continue;
                string[] splitted = line.Split('|');
                if (splitted.Length != 2) continue;
                list.Add(new Song(splitted[0], splitted[1]));
            }

            return list;
        }

        private static List<string> GetAllSongListFiles()
        {
            var fileNames = Directory.EnumerateFiles(_songFolder, "*.txt");
            return fileNames.ToList();
        }

        private static string GetGameName(string fileName)
        {
            try
            {
                return File.ReadLines(fileName).First();
            }
            catch (Exception ex)
            {

            }

            return "";
        }

    }
}
