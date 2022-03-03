using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject mainGameUI, captureTable, cam2d, cam3d, cam360, winScreen, loseScreen, rollScreen, diceObj, movesList, mainSample, ListParent, chessBoard;
    [SerializeField] private Button exitButton, skipButton, moveButton, camButton, rollButton, recallButton, delegateLeftButton, delegateRightButton;
    //[SerializeField] private Sprite ReplaceSprite;
    private Dictionary<string, string> MakeChessNotation = new Dictionary<string, string>();
    [SerializeField] private ChessBoard board;
    public const int NUMBER_OF_ACTIONS = 6;
    private int turnCount = 1;
    private int turnIterator = 0;
    private List<int> skippedTurns = new List<int>();
    private bool skippedTurn = false;
    private string pieceColor = "White";
    private bool pieceTaken = false;
    private bool pieceTakeFail = false;

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
        moveButton.interactable = open;
        camButton.interactable = open;
    }

    public void UI_Exit()
    {
        Debug.Log("Exit");

        // Code to handle what happens before the player can exit

        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        SceneManager.LoadScene(0);
        // Quits to soft start / main menu
    }

    public void UI_Skip()
    {
        Debug.Log("Skip");

        // Code to handle what happens before the player can skip
        if (GameManager.Instance.State == GameState.PlayerTurn)
            GameManager.Instance.UpdateGameState(GameState.EnemyTurn);
        else if (GameManager.Instance.State == GameState.EnemyTurn)
            GameManager.Instance.UpdateGameState(GameState.PlayerTurn);

        GameController controller = GameObject.Find("Game Controller").GetComponent<GameController>();
        controller.OpenCorpSelection();

        board.ResetCommanderData();

        skippedTurns.Add(board.GetNumberOfPieceMoves());
        skippedTurn = true;
        updateMoveList();
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
        //newMoves.ForEach(move => Debug.Log(move));

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

                GameObject childText = newListing.transform.Find("TestText").gameObject;
                childText.GetComponent<TMPro.TextMeshProUGUI>().text = ("");

                if (GameManager.Instance.State == GameState.PlayerTurn)
                    turnIterator = 0;
                else if (GameManager.Instance.State == GameState.EnemyTurn)
                    turnIterator = 6;

            }
            //logic for general moves
            else {
                string[] movesArr = element.Split('|');
                Debug.Log(movesArr[0]);
                Debug.Log(movesArr[1]);

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
                GameObject childText = newListing.transform.Find("TestText").gameObject;
                childText.GetComponent<TMPro.TextMeshProUGUI>().text = (takenIndicator + MakeChessNotation[movesArr[1][1].ToString()] + (char.GetNumericValue(movesArr[1][4]) + 1));
                //if a piece was taken, make the color of the take stand out
                if(takenIndicator.Equals("x")) childText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                if (takenIndicator.Equals("?")) childText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;

                //sprite update
                GameObject childImage = newListing.transform.Find("TestImage").gameObject;
                childImage.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("PieceSprites/" + pieceColor + movesArr[0]);
                Debug.Log("Piece color for this turn: " + pieceColor);
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
                GameObject childText1 = skip1.transform.Find("TestText").gameObject;
                childText1.GetComponent<TMPro.TextMeshProUGUI>().text = ("");
                skip1.SetActive(true);

                if (numberOfSkips == 3)
                {
                    var skip2 = Instantiate(mainSample, ListParent.transform);
                    GameObject childText2 = skip2.transform.Find("TestText").gameObject;
                    childText2.GetComponent<TMPro.TextMeshProUGUI>().text = ("");
                    skip2.SetActive(true);
                }
            }
            if (numberOfSkips != 1)
                newListing.SetActive(true);
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
