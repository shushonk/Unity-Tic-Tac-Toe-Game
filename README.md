# Auto-Generating Unity Tic-Tac-Toe

A completely autonomous, single-script Tic-Tac-Toe game built in Unity. This project is designed to be extremely beginner-friendly by automatically generating its entire User Interface (UI) at runtime, requiring absolutely **zero manual editor setup**.

## Features

- **Zero Editor Setup Required:** No need to manually create Canvases, drag-and-drop prefabs, or attach scripts. Just hit Play!
- **Procedural UI Generation:** The game dynamically builds the board, title, status text, and buttons entirely using C# code.
- **Modern Input System Support:** Fully compatible with Unity's New Input System (`InputSystemUIInputModule`), avoiding legacy input errors.
- **Responsive Layout:** The UI automatically scales perfectly using a fixed 1920x1080 reference resolution, ensuring buttons and text never overlap.
- **Full Game Logic:** 
  - Alternating turns (Player X and Player O)
  - Automatic Win detection (horizontal, vertical, diagonal)
  - Automatic Draw detection when the board fills up
  - Fully functional Restart system

## How to Play

1. Open this project in the **Unity Editor**.
2. Make sure you have any scene open (even a completely empty one works perfectly).
3. Press the **Play** button at the top center of the Unity Editor.
4. The entire Tic-Tac-Toe game UI will automatically appear on your screen!
5. Click on the 3x3 grid cells to place your X or O.
6. The game will automatically announce the winner or a draw. Click **Restart** to play again.

## Technical Details

- **Main Script:** `Assets/Scripts/TicTacToeGame.cs`
- **Entry Point:** The game uses the `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]` attribute to automatically inject itself into the active scene when the game starts.
- **UI Framework:** Uses standard `UnityEngine.UI` for rendering and `UnityEngine.InputSystem.UI` for capturing clicks.

## Troubleshooting

- **No clicks registering?** Make sure your project has the New Input System package installed. The script will automatically upgrade the `EventSystem` at runtime.
- **Font missing or blurry?** The game uses `LegacyRuntime.ttf` by default to ensure maximum compatibility across Unity versions.
