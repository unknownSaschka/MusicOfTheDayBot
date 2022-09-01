using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicOfTheDayBot.Logic;

namespace MusicOfTheDayBot
{
    public class FileHandler
    {
        

        private static string _songFolder = "./songlists/";

        public static List<GameSongLibrary> GetAllSongs()
        {
            List<GameSongLibrary> songs = new List<GameSongLibrary>();

            foreach(string fileName in GetAllSongListFiles())
            {
                string gameName = GetGameName(fileName);

                Console.WriteLine($"Game: {gameName}");

                List<Song> gameSongs = GetSongList(fileName);

                songs.Add(new GameSongLibrary(fileName, gameName, gameSongs));

                Console.WriteLine("");
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
                Console.WriteLine($"Song: {splitted[0]}, Link: {splitted[1]}");
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
                return File.ReadLines(fileName).First().Substring(1);
            }
            catch (Exception ex)
            {

            }

            return "";
        }

        public static void SaveNewList(GameSongLibrary library)
        {
            try
            {
                //File.Delete(library.FileName);
                //File.CreateText(library.FileName);
                StreamWriter sw = File.CreateText(library.FileName);

                sw.WriteLine($"#{library.GameName}");
                foreach(var song in library.Songs)
                {
                    sw.WriteLine($"{song.Name}|{song.YouTubeLink}");
                }
                sw.Flush();
                sw.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string GetFilePath()
        {
            return Path.Combine(_songFolder, $"{Path.GetRandomFileName()}.txt");
        }

        public static void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }
    }
}
