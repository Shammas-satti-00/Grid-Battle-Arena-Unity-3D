using UnityEngine;
using UnityEngine.InputSystem;

public class Tile : MonoBehaviour
{
    [HideInInspector] public GridManager gridManager; // Assigned by GridManager on instantiation
    public int x; // Tile coordinate X
    public int y; // Tile coordinate Y

    private SpriteRenderer spriteRenderer;
    private bool isOccupied = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("Tile: No SpriteRenderer found on " + name);
    }

    void Update()
    {
        // Mouse input (editor/desktop)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            CheckClick(worldPos);
        }

        // Touch input (mobile)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Touchscreen.current.primaryTouch.position.ReadValue());
            CheckClick(worldPos);
        }
    }

    void CheckClick(Vector2 worldPos)
    {
        if (isOccupied) return;

        if (gridManager == null)
        {
            Debug.LogWarning("Tile: gridManager not assigned for " + name);
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.gameObject == gameObject)
        {
            // Notify GridManager about the tile clicked
            gridManager.TileClicked(x, y);

            // Set tile as occupied and update color based on current player
            isOccupied = true;
            spriteRenderer.color = gridManager.GetCurrentPlayerColor();

            Debug.Log($"{gameObject.name} clicked by Player {gridManager.currentPlayer}");
        }
    }
    public void ResetTile()
{
    isOccupied = false;
    spriteRenderer.color = Color.white; // or default tile color
}

}
