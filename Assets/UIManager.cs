using UnityEngine;
using TMPro;  // Make sure TextMeshPro package is imported

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public void SetTurnText(int currentPlayer)
    {
        statusText.text = $"Player {currentPlayer}'s Turn";
    }

    public void SetWinText(int winner)
    {
        statusText.text = $"Player {winner} Wins!";
    }

    public void SetDrawText()
    {
        statusText.text = "Game Draw!";
    }
}
