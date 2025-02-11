using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Puzzle Tiles (Numbered 1-8)")]
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
    private bool isShuffling = true; // Prevents player input during shuffle

    void Start()
    {
        winText.SetActive(false);
        tryAgainButton.SetActive(false);
        exitButton.SetActive(true);
        StartCoroutine(ShuffleTilesAnimated(5f)); // 5 seconds shuffles may be enough?
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

        int moveCount = Random.Range(50, 90); // hehe I randomize go brr
        float delay = shuffleDuration / moveCount;

        Transform lastMovedTile = null;

        for (int i = 0; i < moveCount; i++)
        {
            List<Transform> adjacentTiles = GetMovableTiles();

            // Avoid moving the same tile twice in a row like fr
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


    void MakeRandomMove()
    {
        List<Transform> adjacentTiles = GetMovableTiles();
        if (adjacentTiles.Count > 0)
        {
            Transform randomTile = adjacentTiles[Random.Range(0, adjacentTiles.Count)];
            SwapTiles(randomTile, emptyTile);
        }
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
        int row = (tileNumber - 1) / 3;
        int col = (tileNumber - 1) % 3;
        return new Vector3(-1 + col * tileSize, 1 - row * tileSize, 0);
    }

    Vector3 GetEmptyCorrectPosition()
    {
        return new Vector3(1, -1, 0);
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
