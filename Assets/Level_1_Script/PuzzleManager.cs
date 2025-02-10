using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Tiles (Numbered 1-15)")]
    public List<Transform> numberedTiles;

    [Header("Empty Tile")]
    public Transform emptyTile;

    [Header("UI Elements")]
    public GameObject winText;
    public GameObject tryAgainButton;
    public GameObject mainMenuButton;

    [Header("Grid Settings")]
    public float tileSize = 1.0f;

    private bool gameWon = false;

    void Start()
    {
        ShuffleTiles();
        winText.SetActive(false);
        tryAgainButton.SetActive(false);  
    }

    void Update()
    {
        if (!gameWon && CheckWinCondition())
        {
            ShowWinMessage();
        }
    }

    void ShuffleTiles()
    {
        for (int i = 0; i < numberedTiles.Count; i++)
        {
            int randomIndex = Random.Range(0, numberedTiles.Count);
            Vector3 temp = numberedTiles[i].position;
            numberedTiles[i].position = numberedTiles[randomIndex].position;
            numberedTiles[randomIndex].position = temp;
        }
    }

    bool CheckWinCondition()
    {
        float tolerance = 0.01f;

        for (int i = 0; i < numberedTiles.Count; i++)
        {
            Vector3 correctPos = GetCorrectPosition(i + 1);
            if (Vector3.Distance(numberedTiles[i].position, correctPos) > tolerance)
            {
                return false;
            }
        }

        if (Vector3.Distance(emptyTile.position, GetEmptyCorrectPosition()) > tolerance)
        {
            return false;
        }

        return true;
    }

    Vector3 GetCorrectPosition(int tileNumber)
    {
        int row = (tileNumber - 1) / 4;
        int col = (tileNumber - 1) % 4;
        float x = -1.5f + col * tileSize;
        float y = 1.5f - row * tileSize;
        return new Vector3(x, y, 0);
    }

    Vector3 GetEmptyCorrectPosition()
    {
        return new Vector3(1.5f, -1.5f, 0);
    }

    void ShowWinMessage()
    {
        winText.SetActive(true);
        tryAgainButton.SetActive(true); 
        Debug.Log("You won the game!");
        Time.timeScale = 0;
        gameWon = true;
    }

    public void TryAgain()
    {
        Time.timeScale = 1;
        ShuffleTiles();
        winText.SetActive(false);
        tryAgainButton.SetActive(false); 
        gameWon = false;
    }

    public void InstantWin()
    {
        for (int i = 0; i < numberedTiles.Count; i++)
        {
            numberedTiles[i].position = GetCorrectPosition(i + 1);
        }

        emptyTile.position = GetEmptyCorrectPosition();

        Debug.Log("Instant win applied. Empty tile at: " + emptyTile.position);

        ShowWinMessage();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}
