using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorJuegoMinijuego2 : MonoBehaviour
{
    public CeldaMinijuego2[] cells;
    public Text turnText;
    public Text resultText;
    public Button restartButton;
    public Button menuButton;

    private string currentPlayer = "X";
    private string[] board = new string[9];
    private bool gameOver = false;

    void Start()
    {
        // Configurar los botones
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
            menuButton.gameObject.SetActive(false);
        }

        // Inicializar el tablero
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = "";
        }

        // Configurar las celdas
        for (int i = 0; i < cells.Length; i++)
        {
            int index = i;
            cells[i].GetComponent<Button>().onClick.AddListener(() => OnCellClick(index));
        }

        UpdateTurnText();
    }

    void OnCellClick(int index)
    {
        if (gameOver || board[index] != "")
            return;

        board[index] = currentPlayer;
        cells[index].SetSymbol(currentPlayer);

        if (CheckWin())
        {
            gameOver = true;
            resultText.text = "¡Jugador " + currentPlayer + " Gana!";
            resultText.color = Color.green;
            ShowEndGameButtons();
            return;
        }

        if (CheckDraw())
        {
            gameOver = true;
            resultText.text = "¡Empate!";
            resultText.color = Color.yellow;
            ShowEndGameButtons();
            return;
        }

        // Cambiar de jugador
        currentPlayer = currentPlayer == "X" ? "O" : "X";
        UpdateTurnText();
    }

    bool CheckWin()
    {
        // Combinaciones ganadoras
        int[,] winConditions = new int[,]
        {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Filas
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columnas
            {0, 4, 8}, {2, 4, 6}             // Diagonales
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            if (board[a] != "" && board[a] == board[b] && board[b] == board[c])
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
            if (cell == "")
                return false;
        }
        return true;
    }

    void UpdateTurnText()
    {
        if (turnText != null)
        {
            turnText.text = "Turno del Jugador: " + currentPlayer;
        }
    }

    void ShowEndGameButtons()
    {
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        if (menuButton != null)
            menuButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        // Reiniciar variables
        gameOver = false;
        currentPlayer = "X";

        // Limpiar el tablero
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = "";
            cells[i].ClearSymbol();
        }

        // Limpiar textos
        resultText.text = "";
        UpdateTurnText();

        // Ocultar botones
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);

        if (menuButton != null)
            menuButton.gameObject.SetActive(false);
    }

    public void GoToMenu()
    {
        // Cambiar esto por el nombre de tu escena de menú
        SceneManager.LoadScene("MainMenu");
    }
}