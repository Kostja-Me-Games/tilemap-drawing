using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSUnit : MonoBehaviour
{
    private GameObject selectedGameObject;
    public Vector3 startPosition;
    public Vector3 endPosition;
        public Transform spriteTransform;

	private void Start() {
        spriteTransform = transform.Find("Sprite").transform;
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
            
            transform.position = Vector3.MoveTowards(transform.position, endPosition, 2 * Time.deltaTime);
            if (transform.position == endPosition)
            {
                SetMoving(false);
            }else {
                SetMoving(true);
            }
    }

    public void SetMoving(bool moving) {
        // access animator of the child object "Sprite" and set "Moving" parameter to true
        transform.Find("Sprite").GetComponent<Animator>().SetBool("Moving", moving);	
    }

    void Update()
    {
        
        if (transform.position != endPosition && endPosition != Vector3.zero)
        {
            StartMoving();
        }
    }
}
