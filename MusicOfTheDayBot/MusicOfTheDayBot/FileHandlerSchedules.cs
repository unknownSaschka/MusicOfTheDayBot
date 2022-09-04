using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicOfTheDayBot.ScheduleLogic;

namespace MusicOfTheDayBot
{
    public class FileHandlerSchedules
    {
        private static string FOLDER = "./";
        private static string FILENAME = "schedules";

        public static void SaveSchedules(List<Schedule> schedules)
        {
            try
            {
                var sw = File.CreateText(FOLDER + FILENAME);

                foreach (Schedule schedule in schedules)
                {
                    string gameName = schedule.Game;
                    if (gameName?.Length == 0 || gameName == null) gameName = "null";

                    sw.WriteLine($"{gameName}|{schedule.Time}|{schedule.ChannelInfo.GuildID}|{schedule.ChannelInfo.ChannelID}");
                }

                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public static List<Schedule> LoadSchedules()
        {
            List<Schedule> schedules = new List<Schedule>();

            try
            {
                foreach(var line in File.ReadAllLines(FOLDER + FILENAME))
                {
                    string[] parts = line.Split('|');
                    string gameName = parts[0];
                    if (parts[0].Equals("null")) gameName = "";
                    schedules.Add(new Schedule { Game = gameName, Time = parts[1], ChannelInfo = new Logic.DiscordChannelInfo(ulong.Parse(parts[2]), ulong.Parse(parts[3])) });
                }

            }
            catch(Exception e)
            {
                return new List<Schedule>();
            }

            return schedules;
        }
    }
}
