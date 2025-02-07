using UnityEngine;

public class TileMover : MonoBehaviour
{
    private static Transform emptyTile; // Reference to the empty tile
    private Vector2 targetPosition;
    private float moveSpeed = 5f;
    private bool isMoving = false;

    void Start()
    {
        if (emptyTile == null)
        {
            emptyTile = GameObject.FindWithTag("EmptyTile").transform; // Find the empty tile by tag
        }
        targetPosition = transform.position; // Set initial position
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if ((Vector2)transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (isMoving) return; // Prevent multiple moves

        if (IsAdjacentToEmptyTile())
        {
            SwapWithEmptyTile();
        }
    }

    bool IsAdjacentToEmptyTile()
    {
        float distanceX = Mathf.Abs(transform.position.x - emptyTile.position.x);
        float distanceY = Mathf.Abs(transform.position.y - emptyTile.position.y);

        return (distanceX == 1 && distanceY == 0) || (distanceX == 0 && distanceY == 1);
    }

    void SwapWithEmptyTile()
    {
        Vector2 emptyTilePosition = emptyTile.position;
        emptyTile.position = transform.position;
        targetPosition = emptyTilePosition;
        isMoving = true;
    }
}