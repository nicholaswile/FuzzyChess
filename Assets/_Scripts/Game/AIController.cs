using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private ChessBoard board;
    [SerializeField] private GameController controller;
    [SerializeField] private GameUI gameUI;

    private List<Vector2Int> potentialCaptures = new List<Vector2Int>();

    //capturePairs<coordinates of piece in danger of being captured, list of coordinates of pieces that may capture the key piece> 
    private Dictionary<Vector2Int, List<Vector2Int>> capturePairs = new Dictionary<Vector2Int,List<Vector2Int>>();


    private IEnumerator AI_TakeTurn_Coroutine()
    {
        List<Piece> activeCorpPieces = controller.blackPlayer.KingCorpPieces;

        List<Piece> enemyPieces = controller.whitePlayer.ActivePieces;

        //use method PotentialCaptureFinder to determine which enemy pieces are able to capture friendly pieces
        PotentialCaptureFinder(enemyPieces);

        foreach(Vector2Int potcap in potentialCaptures.ToList())
        {
            //Debug.Log(potcap + " Name of piece " + board.GetPieceOnSquare(potcap).pieceType);
            if(capturePairs.ContainsKey(potcap))
            {
                foreach (Vector2Int contCoords in capturePairs[potcap].ToList())
                {
                    Debug.Log("Friendly " + board.GetPieceOnSquare(potcap).pieceType + " at " + potcap + 
                        " contested by : " + board.GetPieceOnSquare(contCoords).pieceType + " at " + contCoords);
                }
            }
        }
        Debug.Log("There are " + potentialCaptures.Count + " pieces that could be captured this turn.");

        while (controller.activePlayer == controller.blackPlayer)
        {
                foreach (Piece corpPiece in activeCorpPieces.ToList())

            {
                Vector3 piecePosition = board.GetPositionFromCoords(corpPiece.occupiedSquare);

                if (board.isSelectable(corpPiece) && corpPiece.AvailableMoves.Count > 0)
                {

                    //Debug.Log(corpPiece.AvailableMoves.Count);

                    yield return new WaitForSeconds(1);
                    board.OnSquareSelected(piecePosition);

                    foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                    {

                        //see if we can detect that a "take" is available.
                        //Debug.Log(move);
                        //Piece piece = board.GetPieceOnSquare(move);
                        //if (piece != null && !corpPiece.IsFromSameTeam(piece)) Debug.Log("Found cap");
                    }

                        foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                    {
                        //Debug.Log(move);

                        Vector3 movePosition = board.GetPositionFromCoords(move);

                        yield return new WaitForSeconds(1);
                        board.OnSquareSelected(movePosition);

                        break;
                    }
                    break;
                }
            }
            if (activeCorpPieces == controller.blackPlayer.KingCorpPieces) 
                activeCorpPieces = controller.blackPlayer.RightCorpPieces;
            else if (activeCorpPieces == controller.blackPlayer.RightCorpPieces) 
                activeCorpPieces = controller.blackPlayer.LeftCorpPieces;
            else if (activeCorpPieces == controller.blackPlayer.LeftCorpPieces) 
                activeCorpPieces = controller.blackPlayer.KingCorpPieces;
        }
    }


    //Creates a list of type Vector2Int which stores all the locations of pieces which a knight may roll a +1 on capture.
    //This was created because knight movement in AvailableMoves doesn't list places a knight may move to, then capture around.
    //Pieces stored in this list are at a heightened risk of capture, as knights get +1 on their roll for these.
    private List<Vector2Int> KnightHighDangerZone(Piece piece)
    {
        List<Vector2Int> dangerSpots = new List<Vector2Int>();
        foreach (Vector2Int move in piece.AvailableMoves.ToList())
        {
            Debug.Log("Knight first move location: " + move);
            //gather a list of new knight moves at each location that the knight could potentially land.
            Piece contestedPiece = board.GetPieceOnSquare(move);

            //check to make sure we're not looking at already-occupied squares. knight can't move after killing.
            if (contestedPiece != null && !piece.IsFromSameTeam(contestedPiece))
            {
                continue;
            }
            else
            {
                List<Vector2Int> newKnightMoves = piece.GetAdjacentEnemySquares(move);
                foreach (Vector2Int futureSpot in newKnightMoves)
                {
                    //unneeded, since GetAdjacentEnemySquares already checks if there's a piece and if it's an enemy on the square.
                    //Piece possiblePiece = board.GetPieceOnSquare(futureSpot);

                    //go through the usual checks to see if something may be captured, but also check to avoid duplicates. Dupes are unneeded.
                    if (!(dangerSpots.Contains(futureSpot)))
                    {
                        Debug.Log("Added danger spot from knight at " + futureSpot);
                        dangerSpots.Add(futureSpot);
                    }
                }
            }

        }
        return dangerSpots;
    }

    //function to handle both updating the capture list, and updating the dict.
    private void AddToCaptureList(Vector2Int dz, Piece piece)
    {
        if(!(potentialCaptures.Contains(dz)))
        potentialCaptures.Add(dz);

        //check to make sure that the piece isn't in the dict already (i.e. in danger of capture by another piece)
        //and decide on which logic needs to be used to add to the list of vulnerabilities.
        if (capturePairs.ContainsKey(dz))
        {
            capturePairs[dz].Add(piece.occupiedSquare);
        }
        else
        {
            List<Vector2Int> starterList = new List<Vector2Int>
                            {
                                piece.occupiedSquare
                            };
            capturePairs.Add(dz, starterList);
        }
    }

    //written to generate a list of pieces in danger, and a dictionary which links the list of pieces to 
    //each piece that could potentially capture the pieces in danger.
    //designed with defense in mind, but could probably be useful to offense applications as well.

    //INPUT: a list of pieces from the attacking team (example: controller.whitePlayer.ActivePieces)
    //OUTPUT: modifies List<Vector2Int> potentialCaptures and dictionary capturePairs to contain all potential offensive moves for the attacking team.
    private void PotentialCaptureFinder(List<Piece> pieces)
    {
        //ensure these two are emptied before each run of PotentialCaptureFinder.
        potentialCaptures.Clear();
        capturePairs.Clear();
        foreach (Piece corpPiece in pieces.ToList())
        {
            //Debug.Log("Reached inside of enemypieces");
            //Debug.Log("Number of enemy pieces: " + enemyPieces.Count);
            //Debug.Log("Available moves: " + corpPiece.AvailableMoves.Count);
            Vector3 piecePosition = board.GetPositionFromCoords(corpPiece.occupiedSquare);
            Debug.Log("Checking potential for movements from enemy piece " + corpPiece.pieceType + " at coordinate " + corpPiece.occupiedSquare);

            if (corpPiece.AvailableMoves.Count > 0)
            {
                //Handle finding the true area a knight can capture. Knight's AvailableMoves doesn't give an honest representation.
                if (corpPiece.pieceType.ToString().Equals("Knight"))
                {
                    Debug.Log("Found special rule for " + corpPiece.pieceType);
                    List<Vector2Int> knightDangerZone = KnightHighDangerZone(corpPiece);
                    Debug.Log("Now listing places knight can capture");
                    foreach (Vector2Int dz in knightDangerZone.ToList())
                    {
                        Debug.Log(dz);
                        AddToCaptureList(dz, corpPiece);

                    }
                    Debug.Log("End of List.");
                }


                foreach (Vector2Int move in corpPiece.AvailableMoves.ToList())
                {
                    //see if we can detect that a "take" is available.
                    //Debug.Log("Reached inside of availablemoves");
                    Debug.Log(move);
                    Piece piece = board.GetPieceOnSquare(move);
                    if (piece != null && !corpPiece.IsFromSameTeam(piece))
                    {
                        Debug.Log("Found cap");
                        AddToCaptureList(move, corpPiece);
                    }

                }
            }
        }
    }


    public void AI_TakeTurn() 
    {
        StartCoroutine(AI_TakeTurn_Coroutine());
    }
}
