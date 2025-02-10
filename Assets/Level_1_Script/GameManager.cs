using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Puzzle Tiles (Numbered 1-8)")]
    public List<Transform> numberedTiles;

    [Header("Empty Tile")]
    public Transform emptyTile;

    [Header("UI Elements")]
    public GameObject winText;
    public GameObject tryAgainButton; // Shows only on win
    public GameObject exitButton; // Always visible

    [Header("Grid Settings")]
    public float tileSize = 1.0f;

    private bool gameWon = false;

    void Start()
    {
        ShuffleTiles();
        winText.SetActive(false);
        tryAgainButton.SetActive(false);
        exitButton.SetActive(true); // Main Menu button always visible
    }

    void Update()
    {
        if (!gameWon && CheckWinCondition())
        {
            ShowWinMessage();
        }

        if (!gameWon && IsInputDetected(out Vector3 touchPosition))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
            HandleTileMove(worldPosition);
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

        return Vector3.Distance(emptyTile.position, GetEmptyCorrectPosition()) <= tolerance;
    }

    Vector3 GetCorrectPosition(int tileNumber)
    {
        int row = (tileNumber - 1) / 3;
        int col = (tileNumber - 1) % 3;
        float x = -1 + col * tileSize;
        float y = 1 - row * tileSize;
        return new Vector3(x, y, 0);
    }

    Vector3 GetEmptyCorrectPosition()
    {
        return new Vector3(1, -1, 0);
    }

    void ShowWinMessage()
    {
        winText.SetActive(true);
        tryAgainButton.SetActive(true); // Show only on win
        Debug.Log("You won the game!");
        Time.timeScale = 0;
        gameWon = true;
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

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume game speed
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1; // Resume game speed
        SceneManager.LoadScene("Main_Menu"); // Change to your actual Main Menu scene name
    }

    bool IsInputDetected(out Vector3 position)
    {
        position = Vector3.zero;
        if (Input.GetMouseButtonDown(0))
        {
            position = Input.mousePosition;
            return true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            position = Input.GetTouch(0).position;
            return true;
        }
        return false;
    }

    void HandleTileMove(Vector3 inputPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform selectedTile = hit.transform;
            if (numberedTiles.Contains(selectedTile))
            {
                Vector3 tilePosition = selectedTile.position;
                if (Vector3.Distance(tilePosition, emptyTile.position) == tileSize)
                {
                    selectedTile.position = emptyTile.position;
                    emptyTile.position = tilePosition;
                }
            }
        }
    }
}
