﻿/*
 * This Discord Bot is programmed for the German Xenoblade Discord.
 * 
 * Implementation and Idea by unknownSaschka
 */

using MusicOfTheDayBot;

Console.WriteLine("Starting Song Bot");
Logic logic = new Logic();
logic.discord.Init().GetAwaiter().GetResult();

/*

//console debug
bool running = true;
while (running)
{
    Console.WriteLine("Type next | n - new song, r - remove song, a - real all new in, l - list all, ng - new game, rg - remove game, q - quit");

    

    string? s = Console.ReadLine();
    if (s == null) continue;

    string info = "";
    //logic.NewCommand(s, out info);
    Console.WriteLine(info);

    
    if (s.Equals("q"))
    {
        running = false;
        continue;
    }

    if (s.Equals("post"))
    {
        logic.NewPost("", "");
    }

    //new song
    if (s.Equals("n"))
    {
        Console.WriteLine("Type game name");
        string? gameName = Console.ReadLine();
        Console.WriteLine("Type new Song name");
        string? songName = Console.ReadLine();
        Console.WriteLine("Type new song link");
        string? songLink = Console.ReadLine();

        logic.AddSong(gameName, songName, songLink);
    }

    //remove song
    if (s.Equals("r"))
    {
        Console.WriteLine("Type game name");
        string? gameName = Console.ReadLine();
        Console.WriteLine("Type new Song name");
        string? songName = Console.ReadLine();

        logic.RemoveSong(gameName, songName);
    }

    //reads and lists all new in
    if (s.Equals("a"))
    {
        logic.ReadAll();
    }

    if (s.Equals("l"))
    {
        logic.ListAllGames();
    }

    if (s.Equals("ng"))
    {
        Console.WriteLine("Type game name");
        string? gameName = Console.ReadLine();
        logic.NewGame(gameName);
    }

    if (s.Equals("rg"))
    {
        Console.WriteLine("Type game name");
        string? gameName = Console.ReadLine();
        logic.RemoveGame(gameName);
    }
    
}
*/