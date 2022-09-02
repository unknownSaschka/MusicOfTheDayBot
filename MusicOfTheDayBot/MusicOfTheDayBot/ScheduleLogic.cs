using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOfTheDayBot
{
    public class ScheduleLogic
    {
        private List<Schedule> _schedules;

        public struct Schedule
        {
            public string Time;
            public string ChannelID;
            public string Game;
        }

        public ScheduleLogic()
        {
            _schedules = FileHandlerSchedules.LoadSchedules();
        }

        public string GetAllSchedules()
        {
            string toReturn = "";

            for (int i = 0; i < _schedules.Count; i++)
            {
                Schedule schedule = _schedules[i];
                string game = "All";

                if(schedule.Game.Length > 0)
                {
                    game = schedule.Game;
                }

                toReturn += $"Id: {i}, Uhrzeit: {schedule.Time}, Games: {game} \r\n";
            }

            return toReturn;
        }

        public void AddSchedule(string time, string channelID, string game)
        {
            _schedules.Add(new Schedule { Time = time, ChannelID = channelID, Game = game });
        }

        public bool RemoveSchedule(int id)
        {
            try
            {
                _schedules.RemoveAt(id);
                FileHandlerSchedules.SaveSchedules(_schedules);
                return true;
            }
            catch(Exception e) 
            {
                
            }

            return false;
        }

        
    }
}
