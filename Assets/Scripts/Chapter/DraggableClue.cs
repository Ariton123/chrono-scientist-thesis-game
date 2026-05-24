using UnityEngine;
public class DraggableClue : MonoBehaviour
{
    public Transform targetSlot;
    public Stage1WorldIntroController introController;
    public HighlightPulse highlightPulse;
    public float snapDistance = 1.0f;
    public Vector3 placedScale = new Vector3(0.32f, 0.32f, 0.32f);

    private Vector3 startPosition;
    private bool isDragging = false;
    private bool isPlaced = false;

    void Start()
    {
        startPosition = transform.position;
        Debug.Log("DraggableClue started on " + gameObject.name);
    }

    void Update()
    {
        if (isPlaced) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                Debug.Log("Started dragging " + gameObject.name);
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            transform.position = mouseWorld;
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            float distance = Vector2.Distance(transform.position, targetSlot.position);

            if (distance <= snapDistance)
            {
                isPlaced = true;

                if (highlightPulse != null)
                    highlightPulse.StopPulse();

                transform.position = targetSlot.position;
                transform.localScale = placedScale;

                Debug.Log("Clue placed correctly!");
                introController.OnCluePlacedCorrectly();
            }
            else
            {
                transform.position = startPosition;
                Debug.Log("Clue reset.");
            }
        }
    }
}