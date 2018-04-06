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

* [ ] Core
	* [x] Movement
	* [x] Command parsing
	* [ ] Audio
		* `Console.Beep()` Or maybe something else?
* [ ] Commands
	* [x] Movement
	* [x] Pick up
	* [x] Drop
	* [x] Eat/Drink
		* Maybe make them two different commands?
	* [x] Look
	* [x] Equip
	* [ ] Unequip
	* [ ] Examine/Inspect
	* [ ] Use
	* [ ] Insert
	* [ ] Give
* [ ] Actors
	* [x] Base class
	* [ ] NPC's
		* [ ] Generic AI
			* Data-driven behaviour?
			* Should be easily extensible for unique NPC's
		* [ ] Dialouge
			* [KISS](https://en.wikipedia.org/wiki/KISS_principle)
			* Probably some menu-based system, no user-written responses.
	* [ ] Equipment
	* Other, static interractable entities
		* [ ] Containers
		* [ ] Buttons & Levers
		* [ ] Traps
			* Could be activated by interracting with other entities, could be 
			activated by moving between map nodes.
* [ ] Items
	* [x] Base class
	* [ ] An easy way to create new items
		* Data-driven?
		* Generic base class and polymorphism?
	* [ ] Consumables
* [ ] Map
	* [x] Base node class
	* [ ] Better way to build a map
		* Data-driven?
		* Procedually-generated map?
			* Would be pretty darn cool, as always.