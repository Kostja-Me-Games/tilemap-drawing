using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSUnit : MonoBehaviour
{
    private GameObject selectedGameObject;
    public float movementSpeed = 5f;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Transform spriteTransform;
    public BoundsInt area;
    public paintbrush pb;
    private Pathfinding pathfinding;
    private int currentPathIndex = 0;
    private bool isMoving = false;
    [SerializeField] private List<Vector3Int> path;
    [SerializeField] private Vector3Int startMapPosition;
    [SerializeField] private Vector3Int destinationMapPosition;

    private void Awake() {
        selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
        
    }
	private void Start() {
        spriteTransform = transform.Find("Sprite").transform;
        pb = GameObject.Find("Grid").GetComponent<paintbrush>();
        UpdateTakenArea(true);
        pathfinding = new Pathfinding(pb.BuildingsTilemap);
    }
    
    
    public void SetSelectedVisible(bool visible) {
        selectedGameObject.SetActive(visible);	
    }

	
	public bool IsSelected() {
        return selectedGameObject.activeSelf;
    }


    public void MoveTo(Vector3 endPos) {
        Debug.Log("Moving to " + endPos);
        // get current position on tilemap
        startMapPosition = pb.WorldToCell(transform.position);
        destinationMapPosition = pb.WorldToCell(endPos);
        path = pathfinding.FindPath(startMapPosition, destinationMapPosition);
        if (path != null && path.Count > 0) {
            currentPathIndex = 0;
            isMoving = true;
        }

    }
    public void _MoveTo(Vector3 endPos) {
        endPosition = endPos;
        startPosition = transform.position;
        endPosition.z = 0;
        Vector2 vectorToTarget = endPosition - startPosition;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        spriteTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void TurnToTargetPosition(Vector3 target) {
        startPosition = transform.position;
        Vector2 vectorToTarget = target - startPosition;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        spriteTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void StartMoving() {
        // start moving towards the end position

        
        // save the new position in a new variable and check if the building tilemap tile is empty
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, endPosition, 2 * Time.deltaTime);
        Vector3Int nextTilePosition = pb.WorldToCell(nextPosition);
        BoundsInt nextArea = new BoundsInt(nextTilePosition.x, nextTilePosition.y, 0, 1, 1, 1);
        bool nextAreaIsDifferent = !nextArea.Equals(area);
        if (nextAreaIsDifferent) {
            // we need to check if unit can step on the next tile
            bool canMoveToNextArea = pb.AreaCanBeTravelled(nextArea);
            if (!canMoveToNextArea) {
                // if the tile is not empty, stop moving
                SetMoving(false);
                endPosition = transform.position;
                return;
            }
            // if the tile is empty, move the unit to the new position
            transform.position = nextPosition;
            
            
        } else {
            // it is still the same tile, so just keep moving
            transform.position = nextPosition;
            
        }
        if (transform.position == endPosition)
        {
            SetMoving(false);
        } else {
            SetMoving(true);
        }
    }

    public void SetMoving(bool moving) {
        // access animator of the child object "Sprite" and set "Moving" parameter to true
        transform.Find("Sprite").GetComponent<Animator>().SetBool("Moving", moving);	
    }


    public void UpdateTakenArea(bool initial = false) {
        pb.ClearArea(area);
        BoundsInt newArea = pb.GetAreaByPosition(
            pb.GetTileCenterPosition(transform.position),
            area);
        pb.TakeAreaTile(newArea, "under_unit");
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
        TurnToTargetPosition(targetPosition);
        // Check if the unit has reached the current waypoint
        if (transform.position == targetPosition)
        {
            // Move to the next waypoint
            currentPathIndex++;

            // Check if the unit has reached the end of the path
            if (currentPathIndex >= path.Count)
            {
                // Reached the end of the path
                isMoving = false;
            }
        }
    }
    void Update()
    {
        MoveAlongPath();
        // if (transform.position != endPosition && endPosition != Vector3.zero)
        // {
        //     StartMoving(); 
        // }
        SetMoving(isMoving);
        UpdateTakenArea();
    }
}
