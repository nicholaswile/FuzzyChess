# Fuzzy Chess
"Fuzzy Chess" is a chess-themed strategy game for PC. In "Fuzzy Chess", unlike classic chess, the success of piece captures is determined using fuzzy logic. A die is rolled when an attacking piece attempts to capture a defending piece, and the die returns a value that determines whether the capture is executed. The die value needed for a successful capture depends on the type of piece attacking vs the type of piece defending. For example, a pawn needs to roll a 6 to capture a king, while a king needs to roll a 1 to capture a pawn (this is an automatic capture). The game also features a corps system (allowing for multiple moves each turn), a variant chess ruleset (pieces have different movement rules), a robust AI, and level selection.   

## Background
"Fuzzy Chess" was developed from January to May 2022 by a team of 7: Nicholas Wile (me), Parker Smith (team lead), Adam Logan, Tyler Williams, Matthew Graham, Devin Pacl, and Cameron Jones. It was our Computer Science Senior / Capstone Project at Kennesaw State University for the Spring 2022 semester with Dr. Ken Hoganson. The project was completed in four sprints that each ended in meeting with the professor to discuss and demonstrate the progression of the project.

## My Contributions
- Lead game and UI designer: I designed the look of the game, fonts, color palettes, and user interface elements including the menu system, HUD, and other 2D graphics. I also programmed the UI, and developed the visual system to support UI scaling, multiple resolutions, refresh rates, and screen sizes. I also implemented the chess board texture / level selection for players.
- Gameplay and system programming: I implemented the game state manager for defining player, enemy, win, and lose states. I also implemented the fuzzy logic system for determining success of piece captures in the game.
- Unity subject matter expert: I was the only member on the team with prior significant Unity engine experience, so I guided team members on using the technology and helped troubleshoot Unity issues.
- I prepared the sprint documentations and demo presentations. 

## Tools
- Languages: C#
- Technology: Unity 3D, GitHub, Windows

## Demo Video
- Live Demo (starts at 17:00) [https://youtu.be/oOie1JRec9M](https://youtu.be/oOie1JRec9M?t=1020)

## Features
### Corps
The "armies" of pieces are divided into three corps, each of which may move once per turn:

The left bishop commands the three left pawns and the left knight.

The right bishop commands the three right pawns and the right knight.

The king commands the queen, the two rooks, and the remaining two center pawns.

The king may delegate any of its pieces to be commanded by either bishop, at any time. It may also revoke the delegation.

### Fuzzy Logic
Capturing pieces is not automatic. Capturing of pieces is rolled based on the attack strength and defense strength of certain pieces. A pawn is very easy to attack and cannot defend well. A rook has high defense and a moderate attack roll.

### Variant Ruleset
All pieces may move in non-linear directions, but pieces may not jump over other pieces (excluding the rook). Additionally, knights have a special sneak attack move where they can attack any square next to them after they have already made their move, adding +1 to the roll value for the sneak attack.

### AI
Pieces scan their local areas for opportunities or threats, and take action based on the best possible move. This is repeated until each corp has no moves left.
