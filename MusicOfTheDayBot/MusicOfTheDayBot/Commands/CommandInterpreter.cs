using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOfTheDayBot.Commands
{
    public class CommandInterpreter
    {
        /*
         * Commands:
         * !addgame "[gameName]" - Adds a new game
         * !removegame "[gameName]" - removes a game
         * !addsong "[game]" "[songname]" "[youtubelink]" - Adds a song and link to a game
         * !changelink "[game]" "[songname]" "[newyoutubelink]" - changes the link to a song
         * !removesong "[game]" "[songname]" - Removes a song from a game
         * 
         * !post "[game]" "[songname]" [#channel (optional)] - posts a song manually
         * !postrandom [#channel (optional)] - posts a randomly selected song
         * 
         * !list "[game]" - Lists the songlibrary of a game
         * !listlink "[game]" - Lists the songlibrary and the corresponding link
         * !listgames - lists all games
         * 
         * !addschedule [#channel] [time (e.g. 18:00)] "[game (optional)]" - adds a schedule for posting a song Sotd at 18:00 every day
         * !listschedules - lists all schedules with their ScheduleID
         * !removeschedule [id] - removes a schedule
         * 
         */

        public bool ProcessCommand(string commandLine, out string command, out List<string> args)
        {
            command = "";
            args = new List<string>();

            if (commandLine.Length == 0) return false;   //Falls gar nichts eingegeben wurde

            if (commandLine.StartsWith('!'))
            {
                if (commandLine.Length == 1) return false;  //Falls nur ein '!' eingegeben wurde
                
                string currentString = "";

                var inputs = commandLine.Split(' ');
                command = inputs[0].Substring(1);

                //argument processing
                if(inputs.Length > 1)
                {
                    bool inCap = false;

                    for (int i = 1; i < inputs.Length; i++)
                    {
                        if(inputs[i].StartsWith('"') && inputs[i].EndsWith('"'))
                        {
                            if (inputs[i].Length == 1)  // only one single "
                            {
                                if (inCap)
                                {
                                    if (currentString.Length > 0)
                                    {
                                        string overlap = currentString.TrimStart();
                                        overlap = overlap.TrimEnd();

                                        args.Add(overlap);
                                        currentString = "";
                                    }
                                }
                                else
                                {
                                    inCap = true;
                                }

                                continue;
                            }

                            args.Add(RemoveChars(inputs[i], '"'));
                            continue;
                        }

                        if (inputs[i].StartsWith('"'))
                        {
                            inCap = true;
                            //currentString += inputs[i].Substring(1);
                            currentString += RemoveChars(inputs[i], '"') + " ";
                            continue;

                            //Neues Wird mit " gestartet aber vorherige ist noch in current
                            
                        }

                        if (inputs[i].EndsWith('"'))
                        {
                            inCap = false;
                            //currentString += inputs[i].Substring(0, inputs[i].Length - 1);
                            currentString += RemoveChars(inputs[i], '"');
                            args.Add(currentString);
                            currentString = "";
                            continue;
                        }

                        if (inCap)
                        {
                            currentString += inputs[i] + " ";
                        }
                        else
                        {
                            args.Add(inputs[i]);
                        }
                    }
                }

                Console.WriteLine($"Command: {command}");
                foreach(string input in args)
                {
                    Console.WriteLine($"Arg: |{input}|");
                }
            }

            return true;
        }

        private string RemoveChars(string str, char c)
        {
            string toReturn = "";

            foreach(char ch in str)
            {
                if (ch != c) toReturn += ch;
            }

            return toReturn;
        }
    }
}
