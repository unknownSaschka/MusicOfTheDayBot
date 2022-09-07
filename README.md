# MusicOfTheDayBot
 A music bot which randomly choose a music track and posts it in a Discord channel, developed for the German Xenoblade Discord Community.
 
# Tasks
- [x] Liste einlesen mit Songs und den zugehörigen YouTube Links
	- Song Library unterteilt in die einzelnen Games (XC1, XC2, ...). Pro Game eine eigene Text-Datei.
- [x] Commands
	- [x] Command Processing
	- [x] Adding und Removing von Zeitpunkt und Channel
	- [x] Adding und Removing von Tracks
	- [x] Auflisten der gespeicherten Tracks (gesamt, pro Game)
- [x] Discord implementation
	- [x] Rechtesystem
	- [x] Discord Umsetzung und Testing
- [x] Liste die speichert, welche Songs zuvor gepostet wurden und nach einstellbarer Anzahl von Zyklen diese wieder aus Liste entfernt.
	- [x] Zyklisches Posten
	- [x] Speichern und abrufen der Liste

# Planned Commands
- !addgame "[gameName]" - Adds a new game
- !removegame "[gameName]" - removes a game
- !addsong "[game]" "[songname]" "[youtubelink]" - Adds a song and link to a game
- !changelink "[game]" "[songname]" "[newyoutubelink]" - changes the link to a song
- !removesong "[game]" "[songname]" - Removes a song from a game
 
- !post "[game]" "[songname]" [#channel (optional)] - posts a song manually
- !postrandom [#channel (optional)] - posts a randomly selected song

- !list "[game]" - Lists the songlibrary of a game
- !listgames - lists all games
- !listlink "[game]" - Lists the songlibrary and the corresponding link

- !addschedule [#channel] [time (e.g. 18:00)] "[game (optional)]" - adds a schedule for posting a song Sotd at 18:00 every day
- !listschedules - lists all schedules with their ScheduleID
- !removeschedule [id] - removes a schedule
