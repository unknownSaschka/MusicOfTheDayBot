// See https://aka.ms/new-console-template for more information
using MusicOfTheDayBot;
using static MusicOfTheDayBot.FileHandler;

Console.WriteLine("Starting Song Bot");

var gameSongLibs = FileHandler.GetAllSongs();
Console.WriteLine(gameSongLibs.Count);
Console.WriteLine(gameSongLibs[0].GameName);

GameSongLibrary libs = gameSongLibs[0];
Console.WriteLine(libs.Songs[0].Name);
Console.WriteLine(libs.Songs[0].YouTubeLink);

void SelectSong()
{

}