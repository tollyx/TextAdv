# TextAdv

(Temporary name, I promise.)

![Screenshot](https://i.imgur.com/gRSHWHt.png)

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
		* [ ] Figure out an easier way to add sub-commands 
		(eg. take all, drop all)
	* [ ] Audio
		* `Console.Beep()` Or maybe something else?
* [ ] Commands
	* [x] Movement
	* [x] Take
		* [x] Take all
		* [x] Item disambiguation (Make the player specify which item in case 
		of multiple matches)
	* [x] Drop
	* [x] Eat/Drink
		* Maybe make them two different commands?
	* [x] Look
	* [x] Equip
	* [x] Clear
	* [x] Unequip
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
		* [x] Equipping & Unequipping items
		* [ ] Equipment effects (stats, other fun stuff)
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
		* [x] Drinking & Eating
		* [ ] Effects
* [ ] Map
	* [x] Base node class
	* [ ] Better way to build a map
		* Data-driven?
		* Procedually-generated map?
			* Would be pretty darn cool, as always.