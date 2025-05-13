After years of rolling the idea for this game in my head, I've decided to give it a shot at life. My focus with this project is developing each design as close to what I have in my head as possible, instead of committing to a timeline.
Currently working on developing the main systems of the game based on design documents I write.

Important class breakdown:
- Action: the abstract parent of each operation a character, player and enemy, does. Run through InitiativeManager and created by anyone, Action ensures a baseline of information collected and a unified action method, ExecuteAction.
- GridManager: creates and holds the grid of Tiles. Responsible for populating the Tiles with anything necessary, usually on start up.
- InitiativeManager: the central clock of the game. Every object should be designed to listen to this class for instructions on when to do what. 
- ManaContainer: the main component of the mana system, holds and distributes mana through a set of probabilistic relations. ManaContainer holds ManaParticles and operates the logic for when and what mana moves each phase.
- Tile: the main unit of measure of the game, serves as the centerpoint for most systems. Tiles are created through GridManager initializing Tile prefabs with all scripts already attached and linked. As Tile is the centerpiece, the class contains a lot of logic for UX and game script interactions.

Major systems roadmap:
- Grid and tiles
- Characters
- Initiative clock
- Item / Skill attack system
- Character stats
- Magic
- Rituals (here)
- Runes
- Progression
- Truths
- Menus
- Style refactor
- Rooms and camera
- Crafting
- Enemy variety
- Player variety
- AI
