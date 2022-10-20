# MusicOfTheDayBot
 A music bot which randomly choose a music track and posts it in a Discord channel, developed for the German Xenoblade Discord Community.
 
# TODOs
- settings file
- fixing a bug in sceduler tasks

# Usable Commands
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
