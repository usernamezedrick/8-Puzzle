using UnityEngine;

public class TileMover : MonoBehaviour
{
    private static Transform emptyTile;
    private Vector2 targetPosition;
    private float moveSpeed = 5f;
    private bool isMoving = false;

    void Start()
    {
        if (emptyTile == null)
        {
            emptyTile = GameObject.FindWithTag("EmptyTile").transform;
        }
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if ((Vector2)transform.position == targetPosition)
            {
                isMoving = false;
                SnapToGrid();
            }
        }
    }

    void OnMouseDown()
    {
        if (isMoving) return;

        if (IsAdjacentToEmptyTile())
        {
            SwapWithEmptyTile();
        }
    }

    bool IsAdjacentToEmptyTile()
    {
        float distanceX = Mathf.Abs(transform.position.x - emptyTile.position.x);
        float distanceY = Mathf.Abs(transform.position.y - emptyTile.position.y);
        float tolerance = 0.05f;

        return (Mathf.Abs(distanceX - 1) < tolerance && distanceY < tolerance) ||
               (Mathf.Abs(distanceY - 1) < tolerance && distanceX < tolerance);
    }

    void SwapWithEmptyTile()
    {
        Vector2 emptyTilePosition = emptyTile.position;
        emptyTile.position = transform.position;
        targetPosition = emptyTilePosition;
        isMoving = true;

        // Ensure precise alignment after moving
        Invoke(nameof(SnapToGrid), 0.05f);

        // Fix collider issues
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    void SnapToGrid()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x * 100f) / 100f,
            Mathf.Round(transform.position.y * 100f) / 100f,
            transform.position.z
        );
    }
}
