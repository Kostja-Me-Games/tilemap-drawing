using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class ScrollMap : MonoBehaviour
{
    public Tilemap tilemap; //Main tilemap
    // Start is called before the first frame update
    public float scrollSpeed = 0.0f;
    public float scrollZoneSize = 50f;
    void MoveCamera(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction * scrollSpeed * Time.deltaTime;
        Vector3Int mapSize = tilemap.size;
        Debug.Log("mapSize");
        Debug.Log(mapSize);
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(newPosition.x, -mapSize.x / 2f, mapSize.x / 2f),
            Mathf.Clamp(newPosition.y, -mapSize.y / 2f, mapSize.y / 2f),
            newPosition.z
        );
        transform.position = clampedPosition;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Check if mouse position is within the scroll zone
        if (mousePosition.x < scrollZoneSize)
        {
            MoveCamera(Vector3.left);
        }
        else if (mousePosition.x > Screen.width - scrollZoneSize)
        {
            MoveCamera(Vector3.right);
        }

        if (mousePosition.y < scrollZoneSize)
        {
            MoveCamera(Vector3.down);
        }
        else if (mousePosition.y > Screen.height - scrollZoneSize)
        {
            MoveCamera(Vector3.up);
        }
    }
}
