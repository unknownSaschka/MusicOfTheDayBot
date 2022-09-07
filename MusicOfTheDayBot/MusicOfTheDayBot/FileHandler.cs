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
        

        private static string SONGFOLDER = "./songlists/";
        private static string LASTPOSTEDFILENAME = "./lastposted";

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
            try
            {
                var fileNames = Directory.EnumerateFiles(SONGFOLDER, "*.txt");
                return fileNames.ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<string>();
            }
        }

        private static string GetGameName(string fileName)
        {
            try
            {
                return File.ReadLines(fileName).First().Substring(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "";
        }

        public static bool SaveNewList(GameSongLibrary library)
        {
            try
            {
                Directory.CreateDirectory(SONGFOLDER);
                StreamWriter sw = File.CreateText(library.FileName);

                sw.WriteLine($"#{library.GameName}");
                foreach(var song in library.Songs)
                {
                    sw.WriteLine($"{song.Name}|{song.YouTubeLink}");
                }
                sw.Flush();
                sw.Close();

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static string GetFilePath()
        {
            return Path.Combine(SONGFOLDER, $"{Path.GetRandomFileName()}.txt");
        }

        public static void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public static void SaveLastPosted(List<LastPosted> lastPostedList)
        {
            try
            {
                var fs = File.CreateText(LASTPOSTEDFILENAME);

                foreach(var lastPosted in lastPostedList)
                {
                    fs.WriteLine($"{lastPosted.GameName}|{lastPosted.SongName}");
                }
                fs.Flush();
                fs.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not save LastPosted file!");
                Console.WriteLine(e.Message);
            }
        }

        public static List<LastPosted> LoadLastPosted()
        {
            List<LastPosted> lastPostedList = new List<LastPosted>();

            try
            {
                foreach(string lastPosted in File.ReadAllLines(LASTPOSTEDFILENAME))
                {
                    if (lastPosted.Length == 0) continue;
                    var lp = lastPosted.Split('|');
                    lastPostedList.Add(new LastPosted() { GameName = lp[0], SongName = lp[1] });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load LastPosted file!");
                Console.WriteLine(e.Message);
            }

            return lastPostedList;
        }
    }
}
