using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


public class Paintbrush : MonoBehaviour
{
    // singleton
    public static Paintbrush current;
    
    public GridLayout gridLayout; //Grid Object
    public Tilemap GroundTailmap; //Main tilemap
    public Tilemap TempTilemap; //Temp tilemap
    public Tilemap BuildingsTilemap; //Buildings tilemap used to track buildings boundaries
    public Tilemap UnitsTilemap; //Units tilemap used to track units boundaries
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
        TileBase[] groundTilesInBuildingArea = GroundTailmap.GetTilesBlock(building.area);
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
        TileBase[] groundArrayOfTiles = GroundTailmap.GetTilesBlock(area);
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
    public bool AreaCanBeTravelled(BoundsInt area)
    {
        TileBase[] groundArrayOfTiles = GroundTailmap.GetTilesBlock(area);
        TileBase[] buildingsArrayOfTiles = BuildingsTilemap.GetTilesBlock(area);
        for (int i = 0; i < groundArrayOfTiles.Length; i++)
        {
            if (groundArrayOfTiles[i] != tiles["grass"] || !(buildingsArrayOfTiles[i] == tiles["empty"] || buildingsArrayOfTiles[i] == tiles["urinium"]))
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

    public void ClearAreaUnderUrinium(BoundsInt area)
    {
        TileBase[] tilesToClear = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(tilesToClear, "empty");
        BuildingsTilemap.SetTilesBlock(area, tilesToClear);
    }

    public void ClearAreaUnderUnit(BoundsInt prevArea)
    {
        List<TileBase> tilesToClear = new List<TileBase>();
        TileBase[] unitsArrayOfTiles = UnitsTilemap.GetTilesBlock(prevArea);
        // only replace tiles that were "under_unit"
        for (int i = 0; i < unitsArrayOfTiles.Length; i++)
        {
            if (unitsArrayOfTiles[i] == tiles["under_unit"])
            {
                tilesToClear.Add(tiles["empty"]);
            }
            else
            {
                tilesToClear.Add(unitsArrayOfTiles[i]);
            }
        }

        UnitsTilemap.SetTilesBlock(prevArea, tilesToClear.ToArray());
    }

    public void TakeAreaUnderUnit(BoundsInt area)
    {
        List<TileBase> tilesToTake = new List<TileBase>();
        TileBase[] unitsArrayOfTiles = UnitsTilemap.GetTilesBlock(area);
        // only replace tiles that were "empty"
        for (int i = 0; i < unitsArrayOfTiles.Length; i++)
        {
            if (unitsArrayOfTiles[i] == tiles["empty"])
            {
                tilesToTake.Add(tiles["under_unit"]);
            }
            else
            {
                tilesToTake.Add(unitsArrayOfTiles[i]);
            }
        }
        
        UnitsTilemap.SetTilesBlock(area, tilesToTake.ToArray());
        
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
    }

    public BoundsInt GetAreaByPosition(Vector3 pos, BoundsInt area)
    {
        area.position = gridLayout.WorldToCell(pos);
        return area;
    }
    public Vector3Int WorldToCell(Vector3 inputPosition)
    {
        return gridLayout.WorldToCell(inputPosition);
    }
    public Vector3 GetTileCenterPosition(Vector3 inputPosition)
    {
        Vector3 position = gridLayout.WorldToCell(inputPosition);
        position.x += 0.5f;
        position.y += 0.5f;
        return position;
    }
    public void TakeAreaTile(BoundsInt area, string tile)
    {
        TileBase[] tilesToTake = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(tilesToTake, tile);
        BuildingsTilemap.SetTilesBlock(area, tilesToTake);
    }
    public UriniumCrystalScript GetResourceAtTile(Vector3Int tilePosition)
    {   
        // find a Urinium Crystal at the tile position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(tilePosition.x, tilePosition.y), 0.2f);
        foreach (Collider2D collider in colliders)
        {
            UriniumCrystalScript uriniumCrystal = collider.GetComponent<UriniumCrystalScript>();
            if (uriniumCrystal != null)
            {
                return uriniumCrystal;
            }
        }
        return null;
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
        tiles.Add("under_unit", Resources.Load<Tile>("tiles/UnderUnit"));
        tiles.Add("empty", Resources.Load<Tile>("tiles/Empty"));
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
