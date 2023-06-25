using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSUnit : MonoBehaviour
{
    private GameObject selectedGameObject;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Transform spriteTransform;
    public BoundsInt area;
    public paintbrush pb;

	private void Start() {
        spriteTransform = transform.Find("Sprite").transform;
        pb = GameObject.Find("Grid").GetComponent<paintbrush>();
        UpdateTakenArea(true);
    }
    private void Awake() {
        selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
    }
    
    public void SetSelectedVisible(bool visible) {
        selectedGameObject.SetActive(visible);	
    }

	
	public bool IsSelected() {
        return selectedGameObject.activeSelf;
    }

    public void MoveTo(Vector3 endPos) {
        endPosition = endPos;
        startPosition = transform.position;
        endPosition.z = 0;
        Vector2 vectorToTarget = endPosition - startPosition;
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
    void Update()
    {
        
        if (transform.position != endPosition && endPosition != Vector3.zero)
        {
            StartMoving(); 
        }
        UpdateTakenArea();
    }
}
