// See https://aka.ms/new-console-template for more information
using MusicOfTheDayBot;

Console.WriteLine("Starting Song Bot");
Logic logic = new Logic();

//console debug
bool running = true;
while (running)
{
    Console.WriteLine("Type next");

    string? s = Console.ReadLine();
    if (s == null) continue;

    if (s.Equals("q"))
    {
        running = false;
        continue;
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