using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    public bool Placed { get; private set; }
    
    public BoundsInt area;
    private SpriteRenderer spriteRenderer;

    private void OnMouseDOwn()
    {
        if (!Placed)
        {
            
        }
    }

    private void OnMouseDrag()
    {
        if (!Placed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            paintbrush.current.FollowBuilding();
        }
    }
    private void OnMouseUp()
    {
        if (!Placed)
        {
            Vector3Int cellPosition = paintbrush.current.gridLayout.LocalToCell(transform.position);
            transform.localPosition = paintbrush.current.gridLayout.CellToLocalInterpolated(cellPosition + new Vector3(.5f, .5f, 0f));
        }
    }

    public bool CanBePlacedHere()
    {
        BoundsInt newArea = area;
        newArea.position = paintbrush.current.gridLayout.LocalToCell(transform.position);
        return paintbrush.current.AreaCanBeTaken(newArea);
    }

    public void Place()
    {
        if (!Placed)
        {
            Placed = true;
            ChangeOpacity(1f);
            paintbrush.current.TakeArea(area);
        }
    }
    private void ChangeOpacity(float opacity)
    {
        // Get the current color of the sprite
        Color spriteColor = spriteRenderer.color;

        // Set the new alpha value (opacity)
        spriteColor.a = opacity;

        // Assign the updated color back to the sprite
        spriteRenderer.color = spriteColor;
    }
    // Start is called before the first frame update
    void Start()
    {
        // get a sprite renderer component of a child element called "Sprite"
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        ChangeOpacity(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
