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

 if (Input.GetMouseButtonDown(1)) {
            // Right Mouse Button Pressed
            Vector3 moveToPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            List<Vector3> targetPositionList = GetPositionListAround(moveToPosition, new float[] { 3f, 3f, 3f }, new int[] { 5, 10, 20 });

            int targetPositionListIndex = 0;

            foreach (RTSUnit rtsUnit in selectedRTSUnitList) {
                rtsUnit.MoveTo(targetPositionList[targetPositionListIndex]);
                targetPositionListIndex = (targetPositionListIndex + 1) % targetPositionList.Count;
            }
        }
}
void Start()
    {
    }

    // Update is called once per frame
    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray) {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++) {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount) {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++) {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle) {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}
