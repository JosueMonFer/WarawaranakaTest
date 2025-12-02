using UnityEngine;
using UnityEngine.UI;

public class ControladorJuegoMinijuego2 : MonoBehaviour
{
    public static ControladorJuegoMinijuego2 instance; // CAMBIADO

    public CeldaMinijuego2[] cells; // CAMBIADO
    public Text turnText;
    public Text resultText;
    public Button restartButton;

    private string currentPlayer = "X";
    private string[] board = new string[9];
    private bool gameEnded = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        UpdateTurnText();
        resultText.text = "";
    }

    public void MakeMove(int index)
    {
        if (gameEnded || board[index] != null)
            return;

        board[index] = currentPlayer;
        cells[index].SetSymbol(currentPlayer);

        if (CheckWinner())
        {
            resultText.text = "¡Jugador " + currentPlayer + " gana!";
            resultText.color = Color.green;
            gameEnded = true;
            return;
        }

        if (CheckDraw())
        {
            resultText.text = "¡Empate!";
            resultText.color = Color.yellow;
            gameEnded = true;
            return;
        }

        SwitchPlayer();
        UpdateTurnText();
    }

    void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == "X") ? "O" : "X";
    }

    void UpdateTurnText()
    {
        turnText.text = "Turno del jugador: " + currentPlayer;
    }

    bool CheckWinner()
    {
        // Combinaciones ganadoras
        int[,] winConditions = new int[,]
        {
            {0, 1, 2}, // Fila 1
            {3, 4, 5}, // Fila 2
            {6, 7, 8}, // Fila 3
            {0, 3, 6}, // Columna 1
            {1, 4, 7}, // Columna 2
            {2, 5, 8}, // Columna 3
            {0, 4, 8}, // Diagonal 1
            {2, 4, 6}  // Diagonal 2
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            if (board[a] != null && board[a] == board[b] && board[b] == board[c])
            {
                return true;
            }
        }

        return false;
    }

    bool CheckDraw()
    {
        foreach (string cell in board)
        {
            if (cell == null)
                return false;
        }
        return true;
    }

    public void RestartGame()
    {
        currentPlayer = "X";
        board = new string[9];
        gameEnded = false;

        foreach (CeldaMinijuego2 cell in cells) // CAMBIADO
        {
            cell.Reset();
        }

        UpdateTurnText();
        resultText.text = "";
    }
}