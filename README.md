# FuzzyChess
 Senior Project 4A

A chess playing application, with a GUI that displays the board and allows the human player to move according to the rules of the game.

**Distributed AI**
The "armies" of chess pieces for the AI is divided into three "corp":

The "left" side bishop commands the three left pawns and the left knight.
The "right" side bishop commands the three right pawns and the right bishop
The king commands the queen, two rooks, and the remaining two center pawns (the king may delegate any of its pieces to be commanded by either bishop, at any time, based on your AI decision process.

Pieces scan their local areas for opportunities or threats, and provide the results of their observations to their commander (bishop or king).   The commander (bishop or king) makes decisions based on the input from the the pieces it commands, and may also do its own scan of the board, and makes decision on the actions of its pieces (move or attack, the knight may both move and attack).  The bishops may move themselves and engage in combat.
