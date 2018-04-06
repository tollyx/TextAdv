# TextAdv

(Temporary name, I promise.)

A text adventure game/engine written in C#, using .NET (not core, yet).

It runs in your terminal.

Currently the "game" part has no content whatsoever.

## Building

C# together with Visual Studio is so simple so I don't think you need any
 instructions. Oh, it's only tested on VS2017, probably won't work on anything
 earlier.
 
## To-do

Non-complete list of ideas and things to do

* Base
	* [x] Movement
	* [x] Command framework
	* [ ] Audio
		* `Console.Beep()` Or maybe something else?

* Actors
	* [ ] NPC's
		* [ ] Generic AI
			* Data-driven behaviour?
			* Should be easily extensible for unique NPC's
	* [ ] Equipment
	* Other, static interractable entities
		* [ ] Containers
		* [ ] Buttons & Levers
* Items
	* [ ] An easy way to create new items
		* Data-driven?
		* Generic base class and polymorphism?
	* [ ] Consumables
* Map
	* [ ] Better way to build a map
		* Data-driven?
		* Procedually-generated map?
			* Would be pretty darn cool, as always.