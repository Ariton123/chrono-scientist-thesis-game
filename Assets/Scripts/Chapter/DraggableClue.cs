using UnityEngine;

public class DraggableClue : MonoBehaviour
{
    [Header("Placement")]
    public Transform targetSlot;
    public Stage1WorldIntroController introController;
    public HighlightPulse highlightPulse;
    public float snapDistance = 1.0f;

    [Header("Scale")]
    [Tooltip("If true, the clue keeps its original scale when placed.")]
    [SerializeField] private bool keepOriginalScaleOnPlace = true;

    [Tooltip("Only used if Keep Original Scale On Place is false.")]
    [SerializeField] private Vector3 placedScale = new Vector3(0.32f, 0.32f, 0.32f);

    private Vector3 startPosition;
    private Vector3 startScale;
    private float startZ;

    private bool isDragging = false;
    private bool isPlaced = false;

    private void Start()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        startZ = transform.position.z;

        Debug.Log("DraggableClue started on " + gameObject.name);
    }

    private void Update()
    {
        if (isPlaced)
            return;

        if (Camera.main == null)
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = startZ;

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

            if (targetSlot == null)
            {
                ResetToStart();
                return;
            }

            float distance = Vector2.Distance(transform.position, targetSlot.position);

            if (distance <= snapDistance)
            {
                PlaceCorrectly();
            }
            else
            {
                ResetToStart();
            }
        }
    }

    private void PlaceCorrectly()
    {
        isPlaced = true;

        if (highlightPulse != null)
            highlightPulse.StopPulse();

        Vector3 placedPosition = targetSlot.position;
        placedPosition.z = startZ;
        transform.position = placedPosition;

        transform.localScale = keepOriginalScaleOnPlace ? startScale : placedScale;

        Debug.Log("Clue placed correctly!");

        if (introController != null)
            introController.OnCluePlacedCorrectly();
    }

    private void ResetToStart()
    {
        transform.position = startPosition;
        transform.localScale = startScale;

        Debug.Log("Clue reset.");
    }
}