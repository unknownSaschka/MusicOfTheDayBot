using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MusicOfTheDayBot.Logic;

namespace MusicOfTheDayBot
{
    public class ScheduleLogic
    {
        private List<Schedule> _schedules;
        private Logic _logic;

        public ScheduleLogic(Logic logic)
        {
            _schedules = FileHandlerSchedules.LoadSchedules();
            Console.WriteLine("Schedules loaded!");

            for (int i = 0; i < _schedules.Count; i++)
            {
                var schedule = _schedules[i];
                if (NewSchedule(schedule.Time, i, out Timer? timer))
                {
                    if(timer == null)
                    {
                        Console.WriteLine("Fehler beim starten des Timer");
                        continue;
                    }

                    _schedules[i].Timer = timer;
                }
                else
                {
                    Console.WriteLine("Fehler beim starten des Timer");
                }
            }

            Console.WriteLine("Timers loaded");

            _logic = logic;
        }

        private bool NewSchedule(string time, int listID, out Timer? timer)
        {
            string format = "HH:mm";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime? postingTime = null;

            try
            {
                postingTime = DateTime.ParseExact(time, format, provider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                timer = null;
                return false;
            }

            //Falls die gesetzte Uhrzeit bereits verstrichen ist, nächstes Posting auf morgigen Tag setzen
            if(postingTime.Value.TimeOfDay < DateTime.Now.TimeOfDay)
            {
                postingTime = postingTime.Value.AddDays(1);
            }

            timer = new Timer((_) =>
            {
                NewPost(listID);
            }, null, postingTime.Value.Subtract(DateTime.Now), TimeSpan.FromHours(24));

            return true;
        }

        public string GetAllSchedules(out string info, DiscordHandler discord)
        {
            if(_schedules.Count == 0)
            {
                info = "Keine Schedules gesetzt.";
                return info;
            }

            info = "";

            for (int i = 0; i < _schedules.Count; i++)
            {
                Schedule schedule = _schedules[i];
                string game = "All";

                if(schedule.Game?.Length > 0)
                {
                    game = schedule.Game;
                }

                info += $"Id: {i}, Uhrzeit: {schedule.Time}, Channel: {discord.GetChannelName(schedule.ChannelInfo.GuildID, schedule.ChannelInfo.ChannelID)}, Games: {game} \r\n";
            }

            return info;
        }

        public bool AddSchedule(string time, DiscordChannelInfo channelInfo, out string info)
        {
            _schedules.Add(new Schedule(time, channelInfo, "", null));
            int listID = _schedules.Count - 1;

            if (NewSchedule(time, listID, out Timer? timer))
            {
                if(timer == null)
                {
                    info = $"Fehler beim setzen des Timers!";
                    return false;
                }

                _schedules[listID].Timer = timer;
                FileHandlerSchedules.SaveSchedules(_schedules);
                info = $"Schedule um {time} hinzugefügt";
                return true;
            }
            else
            {
                _schedules.RemoveAt(listID);
                info = "Zeitformat inkorrekt! Es muss bspw. aussehen: 18:00 | 14:23 | 04:05";
                return false;
            }
        }

        public bool RemoveSchedule(string id, out string info)
        {
            try
            {
                int parsedID = int.Parse(id);
                _schedules[parsedID].Timer?.Dispose();
                _schedules.RemoveAt(parsedID);
                FileHandlerSchedules.SaveSchedules(_schedules);
                info = "Schedule entfernt!";
                return true;
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
            }

            info = "Schedule konnte nicht entfernt werden!";
            return false;
        }

        private void NewPost(int listID)
        {
            try
            {
                Schedule schedule = _schedules[listID];
                _logic.NewRandomPost(schedule.ChannelInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Schedule
    {
        public string Time;
        public DiscordChannelInfo ChannelInfo;
        public string Game;
        public Timer? Timer;     //wont be saved but created every time the program starts

        public Schedule(string time, DiscordChannelInfo channelInfo, string game, Timer? timer)
        {
            Time = time;
            ChannelInfo = channelInfo;
            Game = game;
            Timer = timer;
        }
    }
}
