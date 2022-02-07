using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject mainGameUI, captureTable, cam2d, cam3d, winScreen, loseScreen, rollScreen, dice, movesList, mainSample, ListParent;
    [SerializeField] private Button exitButton, skipButton, moveButton, camButton, rollButton;
    [SerializeField] private Sprite ReplaceSprite;
    private Dictionary<string, string> MakeChessNotation = new Dictionary<string, string>();
    private int turnCount = 1;
    private List<int> skippedTurns = new List<int>();
    private bool skippedTurn = false;
    private string pieceColor = "White";
    private bool pieceTaken = false;
    public Camera DiceCamera;

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
    }

    private void OnDestroy()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
    }

    private void GameManager_StateChanged(GameState state)
    {
        // Can only skip on player turn
        //exitButton.interactable = (state == GameState.PlayerTurn);
        skipButton.interactable = (state == GameState.PlayerTurn);
        //moveButton.interactable = (state == GameState.PlayerTurn);
        //camButton.interactable = (state == GameState.PlayerTurn);
        rollButton.interactable = (state == GameState.PlayerTurn);

        winScreen.SetActive(state == GameState.Win);
        loseScreen.SetActive(state == GameState.Lose);
        rollScreen.SetActive(state == GameState.Rolling);

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

        GameManager.Instance.UpdateGameState(GameState.EnemyTurn);

        ChessBoard cBoard = GameObject.Find("Chess Board").GetComponent<ChessBoard>();
        skippedTurns.Add(cBoard.GetNumberOfPieceMoves());
        skippedTurn = true;
        SwapPieceColor();
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
            updateMoveList();
        }

        // Shows moves transcript
    }

    public void updateMoveList()
    {
        //Instantiate(mainSample, ListParent.transform);
        ChessBoard cBoard = GameObject.Find("Chess Board").GetComponent<ChessBoard>();
        //ChessBoard cBoard = GameObject.GetComponent<ChessBoard>();
        List<string> newMoves = new List<string>(cBoard.GetNewPieceMoves());
        //newMoves.ForEach(move => Debug.Log(move));

        //Add Skips Into newMoves list
        if (skippedTurn)
        {
            newMoves.Insert(skippedTurns[skippedTurns.Count - 1], "skip");
            skippedTurn = false;
        }

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

            //unique logic for if the skip button is activated
            if (element == "skip")
            {
                GameObject childCount = newListing.transform.Find("TurnNumb").gameObject;
                childCount.GetComponent<TMPro.TextMeshProUGUI>().text = (turnCount.ToString() + ".");

                GameObject childText = newListing.transform.Find("TestText").gameObject;
                childText.GetComponent<TMPro.TextMeshProUGUI>().text = ("SKIP");
            }
            //logic for general moves
            else {
                string[] movesArr = element.Split('|');
                Debug.Log(movesArr[0]);
                Debug.Log(movesArr[1]);

                //turn number update (to be changed later)
                GameObject childCount = newListing.transform.Find("TurnNumb").gameObject;
                childCount.GetComponent<TMPro.TextMeshProUGUI>().text = (turnCount.ToString() + ".");

                //move update
                GameObject childText = newListing.transform.Find("TestText").gameObject;
                childText.GetComponent<TMPro.TextMeshProUGUI>().text = (takenIndicator + MakeChessNotation[movesArr[1][1].ToString()] + (char.GetNumericValue(movesArr[1][4]) + 1));
                //if a piece was taken, make the color of the take stand out
                if(takenIndicator.Equals("x")) childText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;

                //sprite update
                GameObject childImage = newListing.transform.Find("TestImage").gameObject;
                childImage.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load<Sprite>("PieceSprites/" + pieceColor + movesArr[0]);
                Debug.Log("Piece color for this turn: " + pieceColor);
            }
            turnCount++;
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

    public void PieceWasTaken()
    {
        pieceTaken = true;
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
        else
        {
            cam2d.SetActive(true);
            cam3d.SetActive(false);
            Debug.Log("Switched to 2D Cam");
        }
    }

    // Restarts the game 
    public void UI_Retry()
    {
        GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
        SceneManager.LoadScene(1);
    }

    public void RollDie()
    {
        if (rollScreen.activeSelf)
        {
            rollScreen.SetActive(false);
        }
        else 
        { 
            rollScreen.SetActive(true);
        }

    }
}
