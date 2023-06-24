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
    [SerializeField] public Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
    public MouseOverPanel mouseOverPanel;
    public Building building;
    
    // to clear the area of previous location of unplaced building on temp tilemap
    private BoundsInt prevArea;
    
    public void setBrushBuilding(GameObject prefab)
    {
        clearBrushBuilding();
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = gridLayout.CellToLocalInterpolated(new Vector3(mousePos.x, mousePos.y, 0));
        // access prefab's ConstructionPropertiesController
        ConstructionPropertiesController constructionPropertiesController = prefab.GetComponent<ConstructionPropertiesController>();
        if (constructionPropertiesController.enoughCreditsToBuild()) {
            building = Instantiate(prefab, position, Quaternion.identity).GetComponent<Building>();
            FollowBuilding();
        }
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
        TileBase[] buildingTilesInBuildingArea = BuildingsTilemap.GetTilesBlock(building.area);
        int sizeOfGroundTiles = groundTilesInBuildingArea.Length;
        BoundsInt buildingArea = building.area;
        TileBase[] tileArray = new TileBase[sizeOfGroundTiles];

        for (int i = 0; i < groundTilesInBuildingArea.Length; i++)
        {
            // only allow building on grass
            if (groundTilesInBuildingArea[i] == tiles["grass"] && buildingTilesInBuildingArea[i] == tiles["empty"])
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
        TileBase[] groundArrayOfTiles = MainTilemap.GetTilesBlock(area);
        TileBase[] buildingsArrayOfTiles = BuildingsTilemap.GetTilesBlock(area);
        for (int i = 0; i < groundArrayOfTiles.Length; i++)
        {
            if (groundArrayOfTiles[i] != tiles["grass"] || buildingsArrayOfTiles[i] != tiles["empty"])
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
        BuildingsTilemap.SetTilesBlock(area, tilesToTake);
        ClearArea();
        Debug.Log("TakeArea");
        Debug.Log(area);
    }

    public BoundsInt GetAreaByTransform(Transform transform, BoundsInt area)
    {
        area.position = gridLayout.WorldToCell(transform.position);
        return area;
    }

    public void TakeAreaTile(BoundsInt area, string tile)
    {
        TileBase[] tilesToTake = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(tilesToTake, tile);
        BuildingsTilemap.SetTilesBlock(area, tilesToTake);
        Debug.Log("TakeAreaTile");
        Debug.Log(area);
    }

    private void Awake()
    {
        current = this;
        // Load all tiles into the dictionary tiles
        tiles.Add("dirt", Resources.Load<Tile>("tiles/Dirt"));
        tiles.Add("grass", Resources.Load<Tile>("tiles/Grass"));
        tiles.Add("water", Resources.Load<Tile>("tiles/Water"));
        tiles.Add("concrete", Resources.Load<Tile>("tiles/Concrete"));
        tiles.Add("white", Resources.Load<Tile>("tiles/White"));
        tiles.Add("red", Resources.Load<Tile>("tiles/Red"));
        tiles.Add("urinium", Resources.Load<Tile>("tiles/Urinium"));
        tiles.Add("empty", null);
        // Debug.Log keys of tiles array
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!mouseOverPanel.isMouseOver)
        {
            // check if the mouse button is pressed
            if (Input.GetMouseButton(0) && building != null && building.CanBePlacedHere())
            {
                building.Place();
                building = null;
            }
            if (Input.GetMouseButton(1) && building != null)
            {
                clearBrushBuilding();
                ClearArea();
            }
        }
    }
}
