using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSUnit : MonoBehaviour
{
    private GameObject selectedGameObject;
    public float movementSpeed = 1f;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Transform spriteTransform;
    public BoundsInt area;
    public Paintbrush pb;
    private Pathfinding pathfinding;
    private int currentPathIndex = 0;
    public bool isMoving = false;
    [SerializeField] private List<Vector3Int> path;
    [SerializeField] private Vector3Int startMapPosition;
    [SerializeField] private Vector3Int destinationMapPosition;

    private void Awake() {
        selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
        
    }
	private void Start() {
        spriteTransform = transform.Find("Sprite").transform;
        pb = GameObject.Find("Grid").GetComponent<Paintbrush>();
        UpdateTakenArea(true);
        pathfinding = new Pathfinding();
    }
    
    
    public void SetSelectedVisible(bool visible) {
        selectedGameObject.SetActive(visible);	
    }

	
	public bool IsSelected() {
        return selectedGameObject.activeSelf;
    }


    public void MoveTo(Vector3 endPos) {
        // get current position on tilemap
        startMapPosition = pb.WorldToCell(transform.position);
        destinationMapPosition = pb.WorldToCell(endPos);
        path = pathfinding.FindPath(startMapPosition, destinationMapPosition);
        if (path != null && path.Count > 0) {
            currentPathIndex = 0;
            isMoving = true;
        }

    }
    public void ClearMovement() {
        isMoving = false;
        path = null;
        currentPathIndex = 0;
    }

    public void TurnToTargetPosition(Vector3 target) {
        startPosition = transform.position;
        Vector2 vectorToTarget = target - startPosition;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        spriteTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


    public void SetMoving(bool moving) {
        // access animator of the child object "Sprite" and set "Moving" parameter to true
        transform.Find("Sprite").GetComponent<Animator>().SetBool("Moving", moving);	
    }


    public void UpdateTakenArea(bool initial = false) {
        BoundsInt newArea = pb.GetAreaByPosition(
            pb.GetTileCenterPosition(transform.position),
            area);
        Debug.Log("new area: " + newArea + " old area: " + area + " Same: " + (area == newArea).ToString());//+ newArea.Equals(area));
        if (area != newArea)
        {
            Debug.Log("Redrawing");
            pb.TakeAreaUnderUnit(newArea);
            pb.ClearAreaUnderUnit(area);
        }
        else
        {
            Debug.Log("Not redrawing, they are the same");
        }

        area = newArea;
    }



    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0 || path.Count <= currentPathIndex)
        {
            // No valid path found
            isMoving = false;
            return;
        }

        // Move towards the next waypoint in the path
        Vector3 targetPosition = pb.GetTileCenterPosition(path[currentPathIndex]);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        
        // Check if the unit has reached the current waypoint
        if (transform.position == targetPosition)
        {
            // Check if there are more waypoints left to go to
            if (currentPathIndex < path.Count - 1)
            {
                // check if the next waypoint is walkable
                Vector3Int nextTilePosition = path[currentPathIndex + 1];
                BoundsInt nextArea = new BoundsInt(nextTilePosition.x, nextTilePosition.y, 0, 1, 1, 1);
                bool canMoveToNextArea = pb.IsWalkable(nextArea.position);
                if (!canMoveToNextArea)
                {
                    // if the tile is not empty, stop moving
                    isMoving = false;
                    endPosition = transform.position;
                    return;
                }
            }

            // Move to the next waypoint
            currentPathIndex++;

            // Check if the unit has reached the end of the path
            if (currentPathIndex >= path.Count)
            {
                // Reached the end of the path
                isMoving = false;
            }
        }

        if (isMoving)
        {
            TurnToTargetPosition(targetPosition);
        }
    }
    void Update()
    {
        MoveAlongPath();
        SetMoving(isMoving);
        UpdateTakenArea();
    }
}
