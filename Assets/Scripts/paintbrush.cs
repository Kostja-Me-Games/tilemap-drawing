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
    public MouseOverPanel mouseOverPanel;
    public Building building;
    public void setBrush(string brushName)
    {
        
        tile = tiles[brushName];
        clearBrushBuilding();
    }
    public void clearBrush()
    {
        tile = null;
    }

    public void setDirtBrush() {

        setBrush("dirt");

    }
    
    public void setGrassBrush() {

        setBrush("grass");

    }
    
    public void setWaterBrush() {

        setBrush("water");

    }
    public void setConcreteBrush() {
        setBrush("concrete");

    }
    public void setSandBrush() {

        setBrush("sand");

    }

    public void setBrushBuilding(GameObject prefab)
    {
        clearBrushBuilding();
        clearBrush();
        Vector3 position = gridLayout.CellToLocalInterpolated(new Vector3(.5f, .5f, 0f));
        building = Instantiate(prefab, position, Quaternion.identity).GetComponent<Building>();
    }

    public void clearBrushBuilding()
    {
        if (building != null)
        {
            Destroy(building.gameObject);
        }
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
        tiles.Add("concrete", Resources.Load<Tile>("tiles/concrete"));
        tiles.Add("sand", Resources.Load<Tile>("tiles/sand"));
        // Load a tile named "blue" in the folder (Tiles) into the tile variable
        setBrush("dirt");
        Debug.Log(tile);

    }
    // Update is called once per frame
    void Update()
    {
        if (!mouseOverPanel.isMouseOver)
        {
            // check if the mouse button is pressed
            if (Input.GetMouseButton(0) && tile != null && building == null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cell = gridLayout.WorldToCell(mousePos);
                Debug.Log(cell);
                Debug.Log("mouse button pressed");
                // check if the mouse is over the tilemap
                if (MainTilemap.HasTile(cell))
                {
                    Debug.Log("mouse over tilemap");
                    // set the tile
                    MainTilemap.SetTile(cell, tile);
                }
            }
        }
            // if Enter/Return button pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (building != null)
                {
                    building.Place();
                    building = null;
                }
            }
        

    }
}
