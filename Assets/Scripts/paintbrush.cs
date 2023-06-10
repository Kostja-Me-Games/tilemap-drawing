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
    // array of Tile objects with string key
    public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
    public void setBrush(string brushName)
    {
        tile = tiles[brushName];
    }

    public void setDirtBrush() {
        Debug.Log("setDirtBrush");
        setBrush("dirt");
        Debug.Log(tile);
    }
    
    public void setGrassBrush() {
        Debug.Log("setGrassBrush");
        setBrush("grass");
        Debug.Log(tile);
    }
    
    public void setWaterBrush() {
        Debug.Log("setWaterBrush");
        setBrush("water");
        Debug.Log(tile);
    }

    private void Awake()
    {
        current = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Load all tiles into the dictionary tiles
        tiles.Add("dirt", Resources.Load<Tile>("tiles/dirt"));
        tiles.Add("grass", Resources.Load<Tile>("tiles/grass"));
        tiles.Add("water", Resources.Load<Tile>("tiles/water"));
        // Load a tile named "blue" in the folder (Tiles) into the tile variable
        setBrush("dirt");
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