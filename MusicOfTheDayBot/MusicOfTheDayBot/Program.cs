/*
 * This Discord Bot is programmed for the German Xenoblade Discord.
 * 
 * Implementation and Idea by unknownSaschka
 */

using MusicOfTheDayBot;

Console.WriteLine("Starting Song Bot");
Logic logic = new Logic();
logic.discord.Init().GetAwaiter().GetResult();
