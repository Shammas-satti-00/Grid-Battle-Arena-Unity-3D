using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 5;
    public int height = 5;

    [Header("Tile Settings")]
    public GameObject tilePrefab;
    [Range(0.5f, 1f)]
    public float tileScale = 0.92f; // Smaller than 1.0 leaves gaps

    [Header("Players")]
    public Color player1Color = Color.red;
    public Color player2Color = Color.blue;
    public int currentPlayer = 1;

    [Header("References")]
    public UIManager uiManager;  // Assign in inspector

    private int[,] board; // 0 = empty, 1 = player1, 2 = player2
    private bool gameOver = false;

    void Start()
    {
        Debug.Log("Spawning tiles...");
        board = new int[width, height];
        GenerateGrid();

        if (uiManager != null)
            uiManager.SetTurnText(currentPlayer);
    }

    void GenerateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab is not assigned in the Inspector!");
            return;
        }

        float xOffset = (width - 1) / 2f;
        float yOffset = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x - xOffset, y - yOffset);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile {x} {y}";

                tile.transform.localScale = new Vector3(tileScale, tileScale, 1f);

                // Pass reference to GridManager into the Tile script
                Tile tileScript = tile.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.gridManager = this;
                    tileScript.x = x;
                    tileScript.y = y;
                }
            }
        }
    }

    public void TileClicked(int x, int y)
    {
        if (gameOver)
        {
            Debug.Log("Game over! No more moves allowed.");
            return;
        }

        if (board[x, y] != 0)
        {
            Debug.LogWarning("Tile already occupied!");
            return;
        }

        board[x, y] = currentPlayer;

        if (CheckWin(x, y, currentPlayer))
        {
            Debug.Log($"Player {currentPlayer} wins!");
            gameOver = true;
            if (uiManager != null)
                uiManager.SetWinText(currentPlayer);
            return;
        }

        if (IsBoardFull())
        {
            Debug.Log("Game draw!");
            gameOver = true;
            if (uiManager != null)
                uiManager.SetDrawText();
            return;
        }

        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        Debug.Log($"Now it's Player {currentPlayer}'s turn");
        if (uiManager != null)
            uiManager.SetTurnText(currentPlayer);
    }

    public Color GetCurrentPlayerColor()
    {
        return (currentPlayer == 1) ? player1Color : player2Color;
    }

    // Check if current player has 4 in a row from position x,y
    bool CheckWin(int x, int y, int player)
    {
        // Check all directions for 4 consecutive
        return (CountInDirection(x, y, 1, 0, player) + CountInDirection(x, y, -1, 0, player) - 1 >= 4) ||  // Horizontal
               (CountInDirection(x, y, 0, 1, player) + CountInDirection(x, y, 0, -1, player) - 1 >= 4) || // Vertical
               (CountInDirection(x, y, 1, 1, player) + CountInDirection(x, y, -1, -1, player) - 1 >= 4) || // Diagonal \
               (CountInDirection(x, y, 1, -1, player) + CountInDirection(x, y, -1, 1, player) - 1 >= 4);   // Diagonal /
    }

    int CountInDirection(int x, int y, int dx, int dy, int player)
    {
        int count = 0;
        int cx = x;
        int cy = y;

        while (cx >= 0 && cx < width && cy >= 0 && cy < height && board[cx, cy] == player)
        {
            count++;
            cx += dx;
            cy += dy;
        }

        return count;
    }

    bool IsBoardFull()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (board[x, y] == 0)
                    return false;

        return true;
    }

    public void EndTurn()
    {
        // We no longer need this since TileClicked handles turn switching
    }
    public void RestartGame()
{
    // Reset game state
    gameOver = false;
    currentPlayer = 1;
    board = new int[width, height];

    // Reset all tiles visually and occupied flag
    foreach (Transform child in transform)
    {
        Tile tile = child.GetComponent<Tile>();
        if (tile != null)
        {
            tile.ResetTile();
        }
    }

    if (uiManager != null)
        uiManager.SetTurnText(currentPlayer);

    Debug.Log("Game restarted!");
}

}
