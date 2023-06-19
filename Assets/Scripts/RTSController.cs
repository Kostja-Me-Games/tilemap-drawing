using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSController : MonoBehaviour
{

public Vector3 startPosition;
[SerializeField] private List<RTSUnit> selectedRTSUnitList;
private void Awake() {
    selectedRTSUnitList = new List<RTSUnit>();
}
void Update()
{
 if (Input.GetMouseButtonDown(0))
 {
    // Left Mouse Button Pressed
    startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
 }


 if (Input.GetMouseButtonUp(0))
 {
     // Left mouse button held down
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
