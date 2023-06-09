using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrollMap : MonoBehaviour
{
    public Tilemap tilemap; //Main tilemap

    // Start is called before the first frame update
    public float scrollSpeed = 0.0f;
    public float scrollZoneSize = 25f;
    public MouseOverPanel mouseOverPanel;
    public zoom zoom;
    public float panelWidth = 0.0f;

    void MoveCamera(Vector3 direction)
    {
        Vector3 newPosition = transform.position + direction * scrollSpeed * Time.deltaTime;
        Vector3Int mapSize = tilemap.size;
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(newPosition.x, -mapSize.x / 2f, mapSize.x / 2f),
            Mathf.Clamp(newPosition.y, -mapSize.y / 2f, mapSize.y / 2f),
            newPosition.z
        );
        transform.position = clampedPosition;
    }

    private void UpdateScrollSpeed()
    {
        scrollSpeed = 2 + zoom.zoomSize;
    }

    void Start()
    {
        panelWidth = mouseOverPanel.GetComponent<RectTransform>().rect.width;
        zoom = GetComponent<zoom>();
        UpdateScrollSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mouseOverPanel.isMouseOver)
        {
            UpdateScrollSpeed();

            Vector3 mousePosition = Input.mousePosition;
            
            // Check if mouse position is within the scroll zone
            // if (mousePosition.x < scrollZoneSize)
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveCamera(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveCamera(Vector3.right);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveCamera(Vector3.down);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                MoveCamera(Vector3.up);
            }
        }
    }
}