using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HarvesterController : MonoBehaviour
{
    public paintbrush pb;

    // harvesterState enum with list of states
    public enum HarvesterState
    {
        Idle,
        MovingToResource,
        Harvesting,
        MovingToBase,
        Unloading
    }

    public HarvesterState harvesterState = HarvesterState.Idle;
    private RTSUnit rtsUnit;
    [SerializeField] private Vector3Int currentTargetResourceTile;

    [SerializeField] private float resourceCount = 0;

    [SerializeField] private Int32 maxResourceCount = 1500;
    Int32 resourceCountPerSecond = 100;
    [SerializeField] private UriniumCrystalScript uriniumCrystalScript;
    // Start is called before the first frame update
    void Start()
    {
        pb = GameObject.Find("Grid").GetComponent<paintbrush>();
        rtsUnit = GetComponent<RTSUnit>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        /*
         * Harvester logic should be based on the current state of the harvester
         * IDLE: no automation, player must manually control the harvester
         * MOVING_TO_RESOURCE: find the nearest resource tile and move to it
         * HARVESTING: harvest the resource tile: reduce the resource tile's resource count by 1, increase the harvester's resource count by 1, until full then switch to state MOVING_TO_BASE
         * MOVING_TO_BASE: find the nearest refinery tile and move to it
         * UNLOADING: unload the resource tile: reduce the harvester's resource count by 1, increase the refinery's resource count by 1, until empty then switch to state MOVING_TO_RESOURCE
         */
        if (harvesterState == HarvesterState.Idle)
        {
            WhileIdle();
        }
        if (harvesterState == HarvesterState.MovingToResource)
        {
            WhileMovingToResource();
        }

        if (harvesterState == HarvesterState.Harvesting)
        {
            WhileHarvesting();
        }
        
        if (harvesterState == HarvesterState.MovingToBase)
        {
            WhileMovingToBase();
        }
    }

    private void WhileIdle()
    {
        // no automation, player must manually control the harvester
    }

    private void WhileMovingToResource()
    {
        if (currentTargetResourceTile == Vector3Int.zero)
        {
            // if there is no current target resource tile, find the nearest resource tile
            currentTargetResourceTile = FindNearestResource();
        }

        if (currentTargetResourceTile == Vector3Int.zero)
        {
            // looks like there is no applicable resource tile, set state to idle
            harvesterState = HarvesterState.Idle;
            return;
        }
        
        // if the harvester is not at the current target resource tile, not moving, 
        if (rtsUnit.isMoving == false && pb.WorldToCell(transform.position) != currentTargetResourceTile)
        {
            // move to the current target resource tile
            rtsUnit.MoveTo(pb.GetTileCenterPosition(currentTargetResourceTile));
        }
        else if (rtsUnit.isMoving == false && pb.WorldToCell(transform.position) == currentTargetResourceTile)
        {
            // if the harvester is at the current target resource tile, set state to harvesting
            harvesterState = HarvesterState.Harvesting;
        }
        else
        {
            // if the harvester is moving, do nothing
        }

        // find the nearest resource tile and move to it
    }

    private void WhileHarvesting()
    {
        // harvest the resource tile: reduce the resource tile's resource count by 1, increase the harvester's resource count by 1, until full then switch to state MOVING_TO_BASE
        // get the resource at the current tile position
        if (resourceCount >= maxResourceCount)
        {
            harvesterState = HarvesterState.MovingToBase;
            return;
        }
        if (uriniumCrystalScript == null)
        {
            uriniumCrystalScript = pb.GetResourceAtTile(currentTargetResourceTile);
        }

        if (!uriniumCrystalScript)
        {
            harvesterState = HarvesterState.MovingToResource;
        }
        float spaceLeft = maxResourceCount - resourceCount;
        // harvestAmount can't be greater than the resourceCountPerSecond or spaceLeft
        float harvestAmount = Mathf.Min(resourceCountPerSecond * Time.deltaTime, spaceLeft);
        
        resourceCount += uriniumCrystalScript.Harvest(harvestAmount);
    }

    private void WhileMovingToBase()
    {
        // find the nearest refinery tile and move to it
    }

    private void WhileUnloading()
    {
        // unload the resource tile: reduce the harvester's resource count by 1, increase the refinery's resource count by 1, until empty then switch to state MOVING_TO_RESOURCE
    }

    public void SetStateIdle()
    {
        harvesterState = HarvesterState.Idle;
    }

    public void SetStateMovingToResource()
    {
        harvesterState = HarvesterState.MovingToResource;
    }

    public void SetStateHarvesting()
    {
        harvesterState = HarvesterState.Harvesting;
    }

    public void SetStateMovingToBase()
    {
        harvesterState = HarvesterState.MovingToBase;
    }

    public void SetStateUnloading()
    {
        harvesterState = HarvesterState.Unloading;
    }


    // find the nearest tile in BuildingTilemap that has type "urinium"
    public Vector3Int FindNearestResource()
    {
        // get the position of this harvester
        // find the nearest tile in the BuildingTilemap tilemap that has type "urinium"
        // move to that tile
        Tilemap tilemap = pb.BuildingsTilemap;
        Vector3Int harvesterPosition = pb.WorldToCell(transform.position);
        Vector3Int nearestTile = Vector3Int.zero;
        float shortestDistance = Mathf.Infinity;
        foreach (Vector3Int tilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(tilePosition) && tilemap.GetTile(tilePosition) == pb.tiles["urinium"])
            {
                float distance = Vector3Int.Distance(harvesterPosition, tilePosition);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTile = tilePosition;
                }
            }
        }

        return nearestTile;
    }
}