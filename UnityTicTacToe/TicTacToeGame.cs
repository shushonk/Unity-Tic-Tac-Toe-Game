using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming you are using TextMeshPro for modern Unity UI

public class TicTacToeGame : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] cellButtons; // Array of 9 buttons for the grid
    public TextMeshProUGUI[] cellTexts; // Array of 9 text components inside the buttons
    public TextMeshProUGUI statusText; // Text to display whose turn it is or the winner
    public GameObject gameOverPanel; // Optional: A panel to show when the game is over
    public TextMeshProUGUI gameOverText; // Text inside the game over panel

    private string currentPlayer = "X";
    private int turnCount = 0;
    private bool gameActive = true;

    private int[,] winConditions = new int[,]
    {
        {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Rows
        {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columns
        {0, 4, 8}, {2, 4, 6}             // Diagonals
    };

    void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        currentPlayer = "X";
        turnCount = 0;
        gameActive = true;
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateStatusText();

        for (int i = 0; i < cellButtons.Length; i++)
        {
            int index = i; // Local copy for the closure
            cellButtons[i].interactable = true;
            cellTexts[i].text = "";
            
            // Remove previous listeners to avoid duplicates if reset is called
            cellButtons[i].onClick.RemoveAllListeners(); 
            cellButtons[i].onClick.AddListener(() => OnCellClicked(index));
        }
    }

    void OnCellClicked(int index)
    {
        if (!gameActive || cellTexts[index].text != "") return;

        // Set the text and disable the button
        cellTexts[index].text = currentPlayer;
        cellTexts[index].color = currentPlayer == "X" ? new Color(0.2f, 0.7f, 1f) : new Color(1f, 0.4f, 0.7f);
        cellButtons[index].interactable = false;
        
        turnCount++;

        CheckWinner();
    }

    void CheckWinner()
    {
        bool roundWon = false;

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            string t1 = cellTexts[a].text;
            string t2 = cellTexts[b].text;
            string t3 = cellTexts[c].text;

            if (t1 == "" || t2 == "" || t3 == "") continue;

            if (t1 == t2 && t2 == t3)
            {
                roundWon = true;
                break;
            }
        }

        if (roundWon)
        {
            EndGame($"{currentPlayer} Wins!");
        }
        else if (turnCount >= 9)
        {
            EndGame("It's a Draw!");
        }
        else
        {
            // Switch player
            currentPlayer = currentPlayer == "X" ? "O" : "X";
            UpdateStatusText();
        }
    }

    void EndGame(string message)
    {
        gameActive = false;
        statusText.text = message;

        // Disable all remaining buttons
        foreach (var btn in cellButtons)
        {
            btn.interactable = false;
        }

        if (gameOverPanel != null && gameOverText != null)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = message;
        }
    }

    void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = $"{currentPlayer}'s Turn";
            statusText.color = currentPlayer == "X" ? new Color(0.2f, 0.7f, 1f) : new Color(1f, 0.4f, 0.7f);
        }
    }

    // Call this method from your UI Reset Button's OnClick event
    public void RestartGame()
    {
        InitializeGame();
    }
}
