using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class paintbrush : MonoBehaviour
{
    // singleton
    public static paintbrush current;
    
    public GridLayout gridLayout; //Grid Object
    public Tilemap MainTilemap; //Main tilemap

    public Tile tile;
    
    private void Awake()
    {
        current = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Load a tile named "blue" in the folder (Tiles) into the tile variable
        tile = Resources.Load<Tile>("tiles/water");
        Debug.Log(tile);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = gridLayout.WorldToCell(mousePos);
        // check if the mouse button is pressed
        if (Input.GetMouseButton(0))
        {
            // check if the mouse is over the tilemap
            if (MainTilemap.HasTile(cell))
            {
                // set the tile
                MainTilemap.SetTile(cell, tile);
            }
        }

    }
}
