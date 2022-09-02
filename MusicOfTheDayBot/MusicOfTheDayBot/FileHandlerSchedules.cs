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
                    sw.WriteLine($"{schedule.Game}|{schedule.Time}|{schedule.ChannelID}");
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
                    schedules.Add(new Schedule { Game = parts[0], Time = parts[1], ChannelID = parts[2] });
                }
            }
            catch(Exception e)
            {

            }

            return new List<Schedule>();
        }
    }
}
