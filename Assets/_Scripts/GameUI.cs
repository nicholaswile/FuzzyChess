using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject mainGameUI, captureTable, cam2d, cam3d, cam360, winScreen, loseScreen, rollScreen, diceObj, movesList, mainSample, ListParent, chessBoard;
    [SerializeField] private Button exitButton, skipButton, undoButton, moveButton, camButton, rollButton, recallButton, delegateLeftButton, delegateRightButton;
    //[SerializeField] private Sprite ReplaceSprite;
    private Dictionary<string, string> MakeChessNotation = new Dictionary<string, string>();
    [SerializeField] private ChessBoard board;
    [SerializeField] private AIController AIController;
    public const int NUMBER_OF_ACTIONS = 6;
    private int turnCount = 1;
    private int turnIterator = 0;
    private List<int> skippedTurns = new List<int>();
    private bool skippedTurn = false;
    private string pieceColor = "White";
    private bool pieceTaken = false;
    private bool pieceTakeFail = false;
    private List<GameObject> moveListObjects = new List<GameObject> ();

    private Vector3[,] camSwitch = new Vector3[2, 2];


    private void Awake()
    {
        mainGameUI.SetActive(true);
        captureTable.SetActive(false);
        movesList.SetActive(false);

        MakeChessNotation.Add("0", "a");
        MakeChessNotation.Add("1", "b");
        MakeChessNotation.Add("2", "c");
        MakeChessNotation.Add("3", "d");
        MakeChessNotation.Add("4", "e");
        MakeChessNotation.Add("5", "f");
        MakeChessNotation.Add("6", "g");
        MakeChessNotation.Add("7", "h");

        GameManager.StateChanged += GameManager_StateChanged;
        GameManager.RollStateChanged += GameManager_RollStateChanged;
    }

    private void Update()
    {
        if (board.selectedPiece != null)
        {
            delegateLeftButton.interactable = board.CanDelegate(CorpType.Left);
            delegateRightButton.interactable = board.CanDelegate(CorpType.Right);
            recallButton.interactable = board.CanRecall();
        }
        else if (board.selectedPiece == null) 
        {
            delegateLeftButton.interactable = false;
            delegateRightButton.interactable = false;
            recallButton.interactable = false;
        }
    }

    private void GameManager_RollStateChanged(RollState state)
    {
        rollScreen.SetActive(state == RollState.TrueRoll);
    }

    private void OnDestroy()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
        GameManager.RollStateChanged -= GameManager_RollStateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        // Makes sure to not go into a roll state
        if (state == GameState.Win || state == GameState.Lose)
        {
            rollScreen.SetActive(false);
        }
        // Can only skip on player turn
        //exitButton.interactable = (state == GameState.PlayerTurn);
        skipButton.interactable = (state == GameState.PlayerTurn);
        undoButton.interactable = (state == GameState.PlayerTurn);
        //moveButton.interactable = (state == GameState.PlayerTurn);
        //camButton.interactable = (state == GameState.PlayerTurn);
        rollButton.interactable = (state == GameState.PlayerTurn);


        winScreen.SetActive(state == GameState.Win);
        loseScreen.SetActive(state == GameState.Lose);
       //rollScreen.SetActive(state == GameState.Rolling);

        mainGameUI.SetActive(!(state == GameState.Win || state == GameState.Lose));
    }

    public void UI_CaptureTable()
    {
        bool open = captureTable.activeSelf;
        captureTable.SetActive(!open);
        exitButton.interactable = open;
        skipButton.interactable = open;
        undoButton.interactable = open;
        moveButton.interactable = open;
        camButton.interactable = open;

        //closes the moveslist in case its open, as it covers the rolls menu
        movesList.SetActive(false);
    }

    public void UI_Exit()
    {
        Debug.Log("Exit");

        // Code to handle what happens before the player can exit

        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        SceneManager.LoadScene(0);
        // Quits to soft start / main menu
    }

    //TW - 4/7/22
    //add undo button, which will store the moves from the current player turn, allowing move-by-move undoing.
    //Complete!
    public void UI_Undo()
    {
        Debug.Log("Undo");

        if(board.GetNumberOfUndoPieceMoves() >= 1)
        {
            //fetches a list of vector2ints containing the latest move, [0] = new, [1] = old
            List<Vector2Int> LatestMove = new List<Vector2Int>(board.GetUndoPieceMoves());

            //check if a piece was delegated this turn
            if(LatestMove[0] == board.delegatedMove)
            {
                board.RevertDelegation(board.delegatedPiece);
                board.pieceDelegatedThisTurn = false;
                return;
            }

            List<GameObject> moveListList = new List<GameObject>();


            Piece piece = board.GetPieceOnSquare(LatestMove[0]);

            //get the literal position in unity of the square the undo piece is on
            //Vector3 piecePosition = board.GetPositionFromCoords(piece.occupiedSquare);

            //send the standard command for moving the piece backwards
            board.UpdateBoardOnPieceMove(LatestMove[1], piece.occupiedSquare, piece, null);
            piece.MovePiece(LatestMove[1]);
            //Player.GenerateAllMoves();

            //regenerate the list of available moves for the specified piece
            piece.AvailableMoves = piece.FindAvailableSquares();

            //reset the fact that the piece has moved
            piece.setHasMoved(false);

            //reduce the number of moves which the piece's corp has taken
            piece.ReduceCorpMoveNumber();

            //if the corp move number has been returned to 0, the commander may now use his "move 1 without expending authority" again.
            if (piece.CorpMoveNumber() == 0)
                board.UndoCommanderMovedOne(piece);

            //reduce the turn iterator by 1 to indicate the active player has gained a move back
            turnIterator--;

            //delete the movelist entry for the undone move
            //turn the information about the move into a string which matches the chess notation name of the move. I.E. a piece moved to [1, 2] would become "b3"
            string moveCNotation = MakeChessNotation[LatestMove[0].x.ToString()] + (LatestMove[0].y + 1);

            //regenerate moves for team white after every undo press. Prevents undone moves from blocking spaces.
            GameController controller = GameObject.Find("Game Controller").GetComponent<GameController>();
            controller.UndoGeneratePlayerMoves();

            //searches through the list of gameobjects that directly reference the listings on the movelist. adds all objects with the specified name to a new list
            foreach (GameObject CMove in moveListObjects)
            {
                if (CMove.name == moveCNotation)
                {
                    moveListList.Add(CMove);
                }
            }

            //deletes the last found instance of the requested named game object
            Destroy(moveListList[moveListList.Count - 1]);
            moveListObjects.Remove(moveListList[moveListList.Count - 1]);

            Debug.Log("Successfully undone move by: " + piece.pieceType.ToString());
        }
    }

    public void UI_Skip()
    {
        Debug.Log("Skip");

        GameController controller = GameObject.Find("Game Controller").GetComponent<GameController>();

        skippedTurns.Add(board.GetNumberOfPieceMoves());
        skippedTurn = true;
        updateMoveList();

        // Code to handle what happens before the player can skip
        if (GameManager.Instance.State == GameState.PlayerTurn)
        {
            GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
            controller.OpenCorpSelection();
            board.ResetCommanderData();
            AIController.AI_TakeTurn();
        }

        else if (GameManager.Instance.State == GameState.EnemyTurn)

        {
            GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
            controller.OpenCorpSelection();
            board.ResetCommanderData();
        }
    }

    //Heavily modified - TW
    public void UI_Move()
    {
        Debug.Log("Moves");
        bool open = movesList.activeSelf;
        movesList.SetActive(!open);
        if (open != true)
        {
            turnIterator--;
            updateMoveList();
        }

        // Shows moves transcript
    }

    public void UI_DelegateLeft()
    {
        CorpType corp = CorpType.Left;
        board.Delegate(corp);
    }

    public void UI_DelegateRight()
    {
        CorpType corp = CorpType.Right;
        board.Delegate(corp);
    }

    public void UI_Recall() 
    {
        board.Recall();
    }

    public void updateMoveList()
    {
        //There are 12 total turn iterations 1 - 6 being Player Actions and 7 - 12 being Opponent Actions
        if (turnIterator == (NUMBER_OF_ACTIONS * 2))
            turnIterator = 0;
        turnIterator++;

        //Instantiate(mainSample, ListParent.transform);
        //ChessBoard cBoard = GameObject.GetComponent<ChessBoard>();
        List<string> newMoves = new List<string>(board.GetNewPieceMoves());

        //Add Skips Into newMoves list
        if (skippedTurn)
        {
            newMoves.Insert(skippedTurns[skippedTurns.Count - 1], "skip");
            skippedTurn = false;
        }
        int numberOfSkips = 0;

        //Add each move to the list (This isn't needed anymore, as each move is added as it's taken.
        //However, it will be left in, in order to avoid breaking something.)
        foreach (string element in newMoves)
        {
            //creates an array of info about the piece movement. movesArr[0] is the name and [1] is the move itself.
            var newListing = Instantiate(mainSample, ListParent.transform);

            string takenIndicator = "";

            //check if a piece was taken that turn, then put an "x" in the string takenIndicator,
            //which will, by default, always be included in the final move string, "x" or not
            if(pieceTaken == true)
            {
                takenIndicator = "x";
                pieceTaken = false;
            }

            //Check if a piece failed to take another piece this turn. If so, change takenindicator to
            //"?", which denotes the failure of the piece to take its target.
            if (pieceTakeFail == true)
            {
                takenIndicator = "?";
                pieceTakeFail = false;
            }

            //unique logic for if the skip button is activated
            if (element == "skip")
            {
                if (turnIterator == 1 || turnIterator == 7)
                    numberOfSkips = 3;
                else if (turnIterator == 2 || turnIterator == 8 || turnIterator == 5 || turnIterator == 11)
                    numberOfSkips = 2;
                else if (turnIterator == 4 || turnIterator == 10)
                    numberOfSkips = 1;

                if (turnIterator == 1 || turnIterator == 7) 
                {
                    GameObject childCount = newListing.transform.Find("TurnNumb").gameObject;
                    childCount.GetComponent<TMPro.TextMeshProUGUI>().text = (turnCount.ToString() + "<space=4.5>. ");
                }

                GameObject childText = newListing.transform.Find("MoverText").gameObject;
                childText.GetComponent<TMPro.TextMeshProUGUI>().text = ("");

                if (GameManager.Instance.State == GameState.PlayerTurn)
                    turnIterator = 0;
                else if (GameManager.Instance.State == GameState.EnemyTurn)
                    turnIterator = 6;

            }
            //logic for general moves
            else {
                string[] movesArr = element.Split('|');
                //[0] = piece type, [1] = new location, [2] = old location, [3] = name of piece on new location
                Debug.Log("Printing array 0: " + movesArr[0]);
                Debug.Log("Printing array 1: " + movesArr[1]);
                Debug.Log("Printing array 2: " + movesArr[2]);
                Debug.Log("Printing array 2: " + movesArr[3]);

                //sets the listing's name so it may be searched for easily.
                newListing.name = (MakeChessNotation[movesArr[1][1].ToString()] + (char.GetNumericValue(movesArr[1][4]) + 1));
                //newListing.tag = "MoveList";

                //Apply turn number to only the beginning action for each player
                if (turnIterator == 1 || turnIterator == 7)
                {
                    GameObject childCount = newListing.transform.Find("TurnNumb").gameObject;
                    childCount.GetComponent<TMPro.TextMeshProUGUI>().text = (turnCount.ToString() + "<space=4.5>. ");
                }
                if (turnIterator == 4 || turnIterator == 10)
                {
                    GameObject childCount2 = newListing.transform.Find("TurnNumb").gameObject;
                    childCount2.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 0, 0, 0);
                    childCount2.GetComponent<TMPro.TextMeshProUGUI>().text = (turnCount.ToString() + "<space=4.5>. ");
                }

                //move update
                //update the text that will appear under the destination, be it an opposing piece or simply an empty square
                GameObject opposerText = newListing.transform.Find("OpposerText").gameObject;
                opposerText.GetComponent<TMPro.TextMeshProUGUI>().text = (MakeChessNotation[movesArr[1][1].ToString()] + (char.GetNumericValue(movesArr[1][4]) + 1));
                //update the text that will appear under the starting point.
                GameObject moverText = newListing.transform.Find("MoverText").gameObject;
                moverText.GetComponent<TMPro.TextMeshProUGUI>().text = (MakeChessNotation[movesArr[2][1].ToString()] + (char.GetNumericValue(movesArr[2][4]) + 1));


                //if a piece was taken, set the action image accordingly
                if (takenIndicator.Equals("x"))
                {
                    opposerText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                    //update the action sprite as well
                    newListing.transform.Find("ActionImage").gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("MoveSprites/swordicon");
                }
                //if the attack was repelled, set the action image accordingly
                else if (takenIndicator.Equals("?"))
                {
                    opposerText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                    //update the action sprite as well
                    newListing.transform.Find("ActionImage").gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("MoveSprites/finalshieldblank");
                }
                //define default behavior for what to set the action image
                else
                {
                    newListing.transform.Find("ActionImage").gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("MoveSprites/mspaint_2022-04-11_17-05-35");
                }    

                //sprite update
                //update the image for the starter piece.
                GameObject moverImage = newListing.transform.Find("MoverImage").gameObject;
                moverImage.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("PieceSprites/" + pieceColor + movesArr[0]);
                    Debug.Log("Piece color for this turn: " + pieceColor);

                //determine if there was a piece on the square. If so, give the transparent image a new icon.
                if(takenIndicator.Equals("x") || takenIndicator.Equals("?"))
                {
                    GameObject opposerImage = newListing.transform.Find("OpposerImage").gameObject;
                    SwapPieceColor();
                    opposerImage.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("PieceSprites/" + pieceColor + movesArr[3]);
                    SwapPieceColor();
                } else
                {
                    //adjust the scale and position of the opposer text so that it fills up the space where the piece icon would've been
                    GameObject childOpposerText = newListing.transform.Find("OpposerText").gameObject;
                    childOpposerText.GetComponent<TMPro.TextMeshProUGUI>().rectTransform.anchoredPosition = new Vector3(119, -38, 0);
                    childOpposerText.GetComponent<TMPro.TextMeshProUGUI>().transform.localScale = Vector3.one;
                }

            }

            //If the turn is over there is an addition to the count as well as a color switch.
            if (turnIterator % NUMBER_OF_ACTIONS == 0) 
            {
                SwapPieceColor();
                turnCount++;
            }

            //Code to fill in X's for all turns skipped
            if (numberOfSkips >= 2) 
            {
                var skip1 = Instantiate(mainSample, ListParent.transform);
                GameObject childText1 = skip1.transform.Find("MoverText").gameObject;
                childText1.GetComponent<TMPro.TextMeshProUGUI>().text = ("");
                skip1.SetActive(true);

                if (numberOfSkips == 3)
                {
                    var skip2 = Instantiate(mainSample, ListParent.transform);
                    GameObject childText2 = skip2.transform.Find("MoverText").gameObject;
                    childText2.GetComponent<TMPro.TextMeshProUGUI>().text = ("");
                    skip2.SetActive(true);
                }
            }
            if (numberOfSkips != 1)
                newListing.SetActive(true);
            moveListObjects.Add(newListing);
        }
        //Debug.Log(newMoves);
    }

    //Swaps the color of the sprite's piece when called
    public void SwapPieceColor()
    {
        if (pieceColor.Equals("White")) pieceColor = "Black";
        else if (pieceColor.Equals("Black")) pieceColor = "White";
    }

    //Returns info on turn count
    public int GetIteratorCount()
    {
        return turnIterator;
    }

    //Called by another function to indicate that a piece was taken successfully. Movelist functionality.
    public void PieceWasTaken()
    {
        pieceTaken = true;
    }

    // Called by another function to indicate a piece take has failed. Used for movelist functionality.
    public void PieceTakeFailed()
    {
        pieceTakeFail = true;
    }

    public bool GetMoveListState()
    {
        return movesList.activeSelf;
    }

    // Changes camera rotation angle and position
    public void UI_Cam()
    {
        // Switch from 2d / top down view to 3d / POV 
        if (cam2d.activeSelf)
        {
            cam3d.SetActive(true);
            cam2d.SetActive(false);
            Debug.Log("Switched to 3D Cam");
        }
        // Vice versa
        else if (cam3d.activeSelf)
        {
            cam360.SetActive(true);
            cam3d.SetActive(false);
            Debug.Log("Switched to 360 Cam");
        }
        else 
        {
            cam2d.SetActive(true);
            cam360.SetActive(false);
            Debug.Log("Switched to 2D Cam");
        }
    }

    // Restarts the game 
    public void UI_Retry()
    {
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        SceneManager.LoadScene(1);
    }

    public void ResultDie(int result)
    {
        GameManager.Instance.UpdateRollState(RollState.TrueRoll);
        Rigidbody rb = diceObj.GetComponent<Rigidbody>();
        if (result == 1)
        {
            Debug.Log("Static 1");
            rb.transform.rotation = Quaternion.Euler(0, 180, -90);
        }
        else if (result == 2)
        {
            Debug.Log("Static 2");
            rb.transform.rotation = Quaternion.Euler(-90f, 180, -90);
        }
        else if (result == 3)
        {
            Debug.Log("Static 3");
            rb.transform.rotation = Quaternion.Euler(0, 90, -90);
        }
        else if (result == 4)
        {
            Debug.Log("Static 4");
            rb.transform.rotation = Quaternion.Euler(0, -90, -90);
        }
        else if (result == 5)
        {
            Debug.Log("Static 5");
            rb.transform.rotation = Quaternion.Euler(90f, 0, 90);
        }
        else if (result == 6)
        {
            Debug.Log("Static 6");
            rb.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        StartCoroutine(IWait());
    }

    private IEnumerator IWait()
    {
        yield return new WaitForSeconds(3);
        GameManager.Instance.UpdateRollState(RollState.FalseRoll);
    }
}
