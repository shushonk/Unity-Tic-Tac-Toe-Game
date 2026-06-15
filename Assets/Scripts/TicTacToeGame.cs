using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

/// <summary>
/// A completely autonomous Tic-Tac-Toe game that builds its own UI.
/// Playable immediately by clicking Play in Unity without any manual setup.
/// </summary>
public class TicTacToeGame : MonoBehaviour
{
    // ==========================================
    // AUTO-STARTUP LOGIC
    // ==========================================
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindObjectOfType<TicTacToeGame>() != null) return;
        
        GameObject go = new GameObject("TicTacToeGameManager");
        go.AddComponent<TicTacToeGame>();
    }

    // ==========================================
    // GAME VARIABLES
    // ==========================================

    private Button[] cellButtons = new Button[9];
    private Text[] cellTexts = new Text[9];
    private Text statusText;
    
    private string currentPlayer = "X";
    private bool isGameOver = false;
    private int turnsPlayed = 0;
    
    private Font uiFont;

    // ==========================================
    // INITIALIZATION
    // ==========================================

    void Start()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (uiFont == null)
        {
            uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        CreateEventSystem();
        CreateUI();
        RestartGame();
    }

    void CreateEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
        else
        {
            StandaloneInputModule oldModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (oldModule != null)
            {
                DestroyImmediate(oldModule);
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }
    }

    // ==========================================
    // UI GENERATION (1920x1080)
    // ==========================================

    void CreateUI()
    {
        GameObject canvasObj = new GameObject("TicTacToeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.12f, 0.15f, 0.2f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title
        CreateText(canvasObj.transform, "Title", "Tic-Tac-Toe", 56, new Vector2(0, 350), Color.white);

        // Status Text
        statusText = CreateText(canvasObj.transform, "Status", "X's Turn", 34, new Vector2(0, 260), new Color(0.4f, 0.8f, 1f));

        // Board Container
        GameObject boardObj = new GameObject("Board");
        boardObj.transform.SetParent(canvasObj.transform, false);
        Image boardImg = boardObj.AddComponent<Image>();
        boardImg.color = new Color(0.08f, 0.1f, 0.15f);
        
        RectTransform boardRect = boardObj.GetComponent<RectTransform>();
        boardRect.sizeDelta = new Vector2(460, 460); // As requested
        boardRect.anchoredPosition = new Vector2(0, -40); // As requested
        
        GridLayoutGroup grid = boardObj.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // Force exactly 3 columns
        grid.constraintCount = 3;
        grid.cellSize = new Vector2(140, 140); // As requested
        grid.spacing = new Vector2(8, 8); // As requested
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.childAlignment = TextAnchor.MiddleCenter; // Center the grid items

        // 9 Cells
        for (int i = 0; i < 9; i++)
        {
            int index = i; 
            
            GameObject cellObj = new GameObject("Cell_" + i);
            cellObj.transform.SetParent(boardObj.transform, false);
            
            Image cellImg = cellObj.AddComponent<Image>();
            Button btn = cellObj.AddComponent<Button>();
            
            ColorBlock cb = btn.colors;
            cb.normalColor = new Color(0.2f, 0.25f, 0.35f);
            cb.highlightedColor = new Color(0.3f, 0.35f, 0.45f);
            cb.pressedColor = new Color(0.15f, 0.2f, 0.25f);
            cb.selectedColor = new Color(0.2f, 0.25f, 0.35f); // FIX: Stop button turning white when clicked
            cb.disabledColor = new Color(0.2f, 0.25f, 0.35f);
            cb.colorMultiplier = 1;
            btn.colors = cb;
            
            btn.onClick.AddListener(() => OnCellClicked(index));
            cellButtons[i] = btn;

            Text txt = CreateText(cellObj.transform, "Text", "", 72, Vector2.zero, Color.white);
            RectTransform txtRect = txt.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;
            cellTexts[i] = txt;
        }

        // Restart Button
        GameObject restartObj = new GameObject("RestartButton");
        restartObj.transform.SetParent(canvasObj.transform, false); // Explicitly parent to canvas, NOT board
        Image restartImg = restartObj.AddComponent<Image>();
        Button restartBtn = restartObj.AddComponent<Button>();
        
        ColorBlock rcb = restartBtn.colors;
        rcb.normalColor = new Color(0.15f, 0.6f, 0.3f);
        rcb.highlightedColor = new Color(0.2f, 0.7f, 0.35f);
        rcb.pressedColor = new Color(0.1f, 0.5f, 0.2f);
        rcb.selectedColor = new Color(0.15f, 0.6f, 0.3f); // FIX: Stop button turning white when clicked
        rcb.colorMultiplier = 1;
        restartBtn.colors = rcb;
        
        RectTransform restartRect = restartObj.GetComponent<RectTransform>();
        restartRect.sizeDelta = new Vector2(250, 70);
        restartRect.anchoredPosition = new Vector2(0, -330); // Below board, as requested
        
        restartBtn.onClick.AddListener(RestartGame);

        Text restartTxt = CreateText(restartObj.transform, "Text", "Restart", 32, Vector2.zero, Color.white);
        RectTransform rtRect = restartTxt.GetComponent<RectTransform>();
        rtRect.anchorMin = Vector2.zero;
        rtRect.anchorMax = Vector2.one;
        rtRect.sizeDelta = Vector2.zero;
    }

    Text CreateText(Transform parent, string name, string content, int size, Vector2 pos, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        Text t = obj.AddComponent<Text>();
        t.font = uiFont;
        t.text = content;
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = t.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, size + 20);
        rect.anchoredPosition = pos;
        
        return t;
    }

    // ==========================================
    // GAMEPLAY LOGIC
    // ==========================================

    void OnCellClicked(int index)
    {
        if (isGameOver || cellTexts[index].text != "") return;

        cellTexts[index].text = currentPlayer;
        cellTexts[index].color = currentPlayer == "X" ? new Color(0.4f, 0.8f, 1f) : new Color(1f, 0.4f, 0.6f);
        
        turnsPlayed++;
        CheckWinOrDraw();
    }

    void CheckWinOrDraw()
    {
        int[,] lines = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        bool won = false;

        for (int i = 0; i < 8; i++)
        {
            string c1 = cellTexts[lines[i, 0]].text;
            string c2 = cellTexts[lines[i, 1]].text;
            string c3 = cellTexts[lines[i, 2]].text;

            if (c1 != "" && c1 == c2 && c2 == c3)
            {
                won = true;
                break;
            }
        }

        if (won)
        {
            statusText.text = currentPlayer + " Wins!";
            statusText.color = Color.yellow;
            isGameOver = true;
        }
        else if (turnsPlayed == 9)
        {
            statusText.text = "Draw!";
            statusText.color = Color.white;
            isGameOver = true;
        }
        else
        {
            currentPlayer = (currentPlayer == "X") ? "O" : "X";
            statusText.text = currentPlayer + "'s Turn";
            statusText.color = currentPlayer == "X" ? new Color(0.4f, 0.8f, 1f) : new Color(1f, 0.4f, 0.6f);
        }
    }

    void RestartGame()
    {
        currentPlayer = "X";
        turnsPlayed = 0;
        isGameOver = false;

        if (statusText != null)
        {
            statusText.text = "X's Turn";
            statusText.color = new Color(0.4f, 0.8f, 1f);
        }

        for (int i = 0; i < 9; i++)
        {
            if (cellTexts[i] != null)
            {
                cellTexts[i].text = "";
            }
        }
    }
}
