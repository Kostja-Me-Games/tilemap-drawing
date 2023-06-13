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
    public Tilemap TempTilemap; //Temp tilemap
    public Tilemap BuildingsTilemap; //Buildings tilemap used to track buildings boundaries
    public Tile tile;
    // array of Tile objects with string key
    public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
    public MouseOverPanel mouseOverPanel;
    public Building building;
    
    // to clear the area of previous location of unplaced building on temp tilemap
    private BoundsInt prevArea;
    
    // public void setBrush(string brushName)
    // {
    //     
    //     tile = tiles[brushName];
    //     clearBrushBuilding();
    // }
    // public void clearBrush()
    // {
    //     tile = null;
    // }
    //
    // public void setDirtBrush() {
    //
    //     setBrush("dirt");
    //
    // }
    //
    // public void setGrassBrush() {
    //
    //     setBrush("grass");
    //
    // }
    //
    // public void setWaterBrush() {
    //
    //     setBrush("water");
    //
    // }
    // public void setConcreteBrush() {
    //     setBrush("concrete");
    //
    // }
    // public void setSandBrush() {
    //
    //     setBrush("sand");
    //
    // }

    public void setBrushBuilding(GameObject prefab)
    {
        clearBrushBuilding();
        // clearBrush();
        Vector3 position = gridLayout.CellToLocalInterpolated(new Vector3(.5f, .5f, 0f));
        building = Instantiate(prefab, position, Quaternion.identity).GetComponent<Building>();
        FollowBuilding();
    }

    public void clearBrushBuilding()
    {
        if (building != null)
        {
            Destroy(building.gameObject);
        }
    }

    public void FollowBuilding()
    {
        ClearArea();
        // Get a cell 
        building.area.position = gridLayout.WorldToCell(building.transform.position);
        // get tiles block from MainTilemap
        TileBase[] groundTilesInBuildingArea = MainTilemap.GetTilesBlock(building.area);
        int sizeOfGroundTiles = groundTilesInBuildingArea.Length;
        BoundsInt buildingArea = building.area;
        TileBase[] tileArray = new TileBase[sizeOfGroundTiles];

        for (int i = 0; i < groundTilesInBuildingArea.Length; i++)
        {
            // only allow building on grass
            if (groundTilesInBuildingArea[i] == tiles["grass"])
            {
                tileArray[i] = tiles["white"];
            }
            else
            {
                FillTiles(tileArray, "red");
                break;

            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public bool AreaCanBeTaken(BoundsInt area)
    {
        TileBase[] arrayOfTiles = MainTilemap.GetTilesBlock(area);
        foreach (TileBase tile in arrayOfTiles)
        {
            if (tile != tiles["grass"])
            {
                return false;
            }
        }
        return true;
    }

    private void ClearArea()
    {
        TileBase[] tilesToClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(tilesToClear, "empty");
        TempTilemap.SetTilesBlock(prevArea, tilesToClear);
    }
    private static void FillTiles(TileBase[] arr, string type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = current.tiles[type];
        }
    }
    
    public void TakeArea(BoundsInt area)
    {
        TileBase[] tilesToTake = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(tilesToTake, "concrete");
        MainTilemap.SetTilesBlock(area, tilesToTake);
        ClearArea();
    }

    private void Awake()
    {
        current = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Load all tiles into the dictionary tiles
        tiles.Add("dirt", Resources.Load<Tile>("tiles/Dirt"));
        tiles.Add("grass", Resources.Load<Tile>("tiles/Grass"));
        tiles.Add("water", Resources.Load<Tile>("tiles/Water"));
        tiles.Add("concrete", Resources.Load<Tile>("tiles/Concrete"));
        tiles.Add("white", Resources.Load<Tile>("tiles/White"));
        tiles.Add("red", Resources.Load<Tile>("tiles/Red"));
        tiles.Add("empty", null);
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
                    if (building.CanBePlacedHere())
                    {
                        building.Place();
                        building = null;
                    }
                }
            }
        

    }
}
