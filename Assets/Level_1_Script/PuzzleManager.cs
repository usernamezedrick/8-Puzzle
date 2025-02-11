using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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
    public GameObject exitButton;

    [Header("Grid Settings")]
    public float tileSize = 1.0f;
    private bool gameWon = false;
    private bool isShuffling = true;

    void Start()
    {
        winText.SetActive(false);
        tryAgainButton.SetActive(false);
        exitButton.SetActive(true);
        StartCoroutine(ShuffleTilesAnimated(10f)); // 10 seconds shuffle
    }

    void Update()
    {
        if (!gameWon && !isShuffling && CheckWinCondition())
        {
            ShowWinMessage();
        }

        if (!gameWon && !isShuffling && IsInputDetected(out Vector3 touchPosition))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));
            HandleTileMove(worldPosition);
        }
    }

    IEnumerator ShuffleTilesAnimated(float shuffleDuration)
    {
        isShuffling = true;

        int moveCount = Random.Range(100, 150); // More moves for 4x4 medukseeee
        float delay = shuffleDuration / moveCount;

        Transform lastMovedTile = null;

        for (int i = 0; i < moveCount; i++)
        {
            List<Transform> adjacentTiles = GetMovableTiles();

            if (adjacentTiles.Count > 1 && lastMovedTile != null)
            {
                adjacentTiles.Remove(lastMovedTile);
            }

            Transform randomTile = adjacentTiles[Random.Range(0, adjacentTiles.Count)];
            SwapTiles(randomTile, emptyTile);
            lastMovedTile = randomTile;

            yield return new WaitForSeconds(delay);
        }

        isShuffling = false;
    }

    List<Transform> GetMovableTiles()
    {
        List<Transform> movableTiles = new List<Transform>();
        foreach (Transform tile in numberedTiles)
        {
            if (Vector3.Distance(tile.position, emptyTile.position) == tileSize)
            {
                movableTiles.Add(tile);
            }
        }
        return movableTiles;
    }

    bool CheckWinCondition()
    {
        for (int i = 0; i < numberedTiles.Count; i++)
        {
            if (Vector3.Distance(numberedTiles[i].position, GetCorrectPosition(i + 1)) > 0.01f)
            {
                return false;
            }
        }
        return Vector3.Distance(emptyTile.position, GetEmptyCorrectPosition()) <= 0.01f;
    }

    Vector3 GetCorrectPosition(int tileNumber)
    {
        int row = (tileNumber - 1) / 4;
        int col = (tileNumber - 1) % 4;
        return new Vector3(-1.5f + col * tileSize, 1.5f - row * tileSize, 0);
    }

    Vector3 GetEmptyCorrectPosition()
    {
        return new Vector3(1.5f, -1.5f, 0);
    }

    void ShowWinMessage()
    {
        winText.SetActive(true);
        tryAgainButton.SetActive(true);
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
        ShowWinMessage();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main_Menu");
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
        if (isShuffling) return;

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform selectedTile = hit.transform;
            if (numberedTiles.Contains(selectedTile))
            {
                if (Vector3.Distance(selectedTile.position, emptyTile.position) == tileSize)
                {
                    SwapTiles(selectedTile, emptyTile);
                }
            }
        }
    }

    void SwapTiles(Transform tile, Transform empty)
    {
        Vector3 temp = tile.position;
        tile.position = empty.position;
        empty.position = temp;
    }
}
