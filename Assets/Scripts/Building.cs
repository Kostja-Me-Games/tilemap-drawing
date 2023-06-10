using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    
    public bool Placed { get; private set; }
    
    public BoundsInt area;

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
    public void Place()
    {
        if (!Placed)
        {
            Placed = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
