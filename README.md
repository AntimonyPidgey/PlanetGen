# PlanetGen 


A sector map generator for the tabletop role-playing game Mongoose Traveller, translating the planet generation rules from the book to an automated program. I found it an interesting challenge, as the data formatting required to generate a map of sufficient size using a map generator at https://travellermap.com/make/poster was very picky. Written in C# using Visual Studio Express.

1. Select mode by typing in the corresponding number.

MODE 1 - sector generator

1. Enter sector density from 0 to 100. This integer represents the percentage of hexes filled by the generator.
2. The output will be saved to "Sector.txt".
3. Paste this output exactly as it is into http://travellermap.com/make/poster.

MODE 2 - verbose colony generator

1. Enter Subsector
2. Enter System Name
3. Keep pressing enter until you find a spread you like.
4. Some of the text is a framework to build on and not suitable for players. 
You can retype it, adding suitable clarifications and details, 
or just delete the Deviances and Factions text to give the players raw data.

MODE 3 - secor generator to verbose colony

1. Paste a desired line from the list of colonies made by the sector generator.
WARNING: Explicit details are randomly generated each time.

------

Last Modification: Feb 21, 2016

v0.21 - Implemented saving generated reports to file, sector naming, more intuitive menu loop.
v0.2 - Implemented 2 more modes to work with http://travellermap.com/make/poster.
v0.1 - Initial testing release
