using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSController : MonoBehaviour
{

public Vector3 startPosition;
[SerializeField] private List<RTSUnit> selectedRTSUnitList;
[SerializeField] private Transform selectionAreaTransform;
private void Awake() {
    selectedRTSUnitList = new List<RTSUnit>();
    selectionAreaTransform.gameObject.SetActive(false);
}
void Update()
{
 if (Input.GetMouseButtonDown(0))
 {
    // Left Mouse Button Pressed
    selectionAreaTransform.gameObject.SetActive(true);
    startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
 }

 if (Input.GetMouseButton(0)) {
     // Left Mouse Button Held Down
     Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     Vector3 lowerLeft = new Vector3(
         Mathf.Min(startPosition.x, currentMousePosition.x),
         Mathf.Min(startPosition.y, currentMousePosition.y)
     );
     Vector3 upperRight = new Vector3(
         Mathf.Max(startPosition.x, currentMousePosition.x),
         Mathf.Max(startPosition.y, currentMousePosition.y)
     );
     selectionAreaTransform.position = lowerLeft;
     selectionAreaTransform.localScale = upperRight - lowerLeft;
 }

 if (Input.GetMouseButtonUp(0))
 {
     // Left mouse button held down
     selectionAreaTransform.gameObject.SetActive(false);
     Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition));
     foreach (RTSUnit rtsUnit in selectedRTSUnitList)
     {
         rtsUnit.SetSelectedVisible(false);
     }
     selectedRTSUnitList.Clear();
        
        foreach (Collider2D collider2D in collider2DArray)
        {
            RTSUnit rtsUnit = collider2D.GetComponent<RTSUnit>();
            if (rtsUnit != null)
            {
                rtsUnit.SetSelectedVisible(true);
                selectedRTSUnitList.Add(rtsUnit);
            }
        }
        
        
        
        
 }
}
void Start()
    {
    }

    // Update is called once per frame
    
}
