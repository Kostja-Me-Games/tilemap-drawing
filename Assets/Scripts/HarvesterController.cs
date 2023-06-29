using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HarvesterController : MonoBehaviour
{
    public Paintbrush pb;
    public CreditsController creditsController;

    [SerializeField] private SpriteRenderer spriteRenderer;

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
    Int32 resourceCountPerSecond = 300;
    Int32 resourceCountUnloadingPerSecond = 600;
    [SerializeField] private UriniumCrystalScript uriniumCrystalScript;
    [SerializeField] private RefineryController refineryController;

    [SerializeField] private float distanceToRefinery = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Specific unit controller started: Harvester");
        pb = GameObject.Find("Grid").GetComponent<Paintbrush>();
        creditsController = GameObject.Find("Credits").GetComponent<CreditsController>();
        rtsUnit = transform.GetComponent<RTSUnit>();
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
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

        if (harvesterState == HarvesterState.Unloading)
        {
            WhileUnloading();
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

        if (currentTargetResourceTile != Vector3Int.zero && pb.WorldToCell(transform.position) != currentTargetResourceTile && !pb.IsWalkable(currentTargetResourceTile))
        {
            currentTargetResourceTile = FindNearestResource();
        }

        if (currentTargetResourceTile == Vector3Int.zero)
        {
            // looks like there is no applicable resource tile, set state to idle
            if (resourceCount > 0)
            {
                SetStateMovingToBase();
            }
            else
            {
                // do nothing, in case if a resource tile becomes available again
            }

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
            uriniumCrystalScript = null;
            return;
        }

        if (uriniumCrystalScript == null)
        {
            // Harvester has the right to harvest only if it is located at the same time, so we get position by the current position of harvester.
            Vector3Int tileCenterPosition = pb.WorldToCell(transform.position);
            uriniumCrystalScript = pb.GetResourceAtTile(tileCenterPosition);
        }

        if (!uriniumCrystalScript)
        {
            currentTargetResourceTile = Vector3Int.zero;
            harvesterState = HarvesterState.MovingToResource;
            return;
        }

        float spaceLeft = maxResourceCount - resourceCount;
        // harvestAmount can't be greater than the resourceCountPerSecond or spaceLeft
        float harvestAmount = Mathf.Min(resourceCountPerSecond * Time.deltaTime, spaceLeft);

        resourceCount += uriniumCrystalScript.Harvest(harvestAmount);
    }

    private void WhileMovingToBase()
    {
        // find the nearest refinery tile and move to it
        if (!refineryController)
        {
            refineryController = FindNearestRefinery();
        }

        if (!refineryController)
        {
            harvesterState = HarvesterState.Idle;
            return;
        }

        distanceToRefinery =
            Physics2D.Distance(refineryController.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>())
                .distance;

        if (distanceToRefinery < 0.2f)
        {
            if (refineryController.harvester == null)
            {
                rtsUnit.ClearMovement();
                transform.position = pb.GetTileCenterPosition(refineryController.transform.position) +
                                     new Vector3(1.6f, -0.15f, 0);
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, -90);
                refineryController.SetUnloadingHarvester(this);
                harvesterState = HarvesterState.Unloading;
                return;
            }
            else
            {
                refineryController = null;
            }
        }

        if (rtsUnit.isMoving == false && pb.WorldToCell(transform.position) != refineryController.transform.position)
        {
            Vector3Int walkablePositionNearRefinery = FindWalkablePositionAroundRefinery(refineryController);
            
            rtsUnit.MoveTo(walkablePositionNearRefinery);
        }

        // check if the harvester is near the refinery (the distance to refinery is 1) then move to state UNLOADING
    }

    private void WhileUnloading()
    {
        // unload the resource tile: reduce the harvester's resource count by 1, increase the refinery's resource count by 1, until empty then switch to state MOVING_TO_RESOURCE
        if (resourceCount > 0)
        {
            double unloadAmount =
                Math.Floor(Mathf.Min(resourceCountUnloadingPerSecond * Time.deltaTime, resourceCount));
            creditsController.AddCredits(unloadAmount);
            resourceCount -= (float)unloadAmount;
        }

        if (resourceCount < 1)
        {
            resourceCount = 0;
            Vector3Int walkablePositionNearRefinery = FindWalkablePositionAroundRefinery(refineryController);
            if (walkablePositionNearRefinery == Vector3Int.zero)
            {
                // do nothing, wait when spot becomes available
                return;
            }
            refineryController.ClearUnloadingHarvester();
            Vector3 exactPosition = pb.GetTileCenterPosition(walkablePositionNearRefinery);
            transform.position = exactPosition;
            SetStateMovingToResource();
        }
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

    public Vector3Int FindWalkablePositionAroundRefinery(RefineryController targetRefineryController)
    {
        // get the position of refinery and the area from RefineryController, cycle through neighbor tiles and find which is walkable
        BoundsInt refineryArea = targetRefineryController.building.area;
        refineryArea.x -= 1;
        refineryArea.y -= 1;
        Vector3Int walkablePosition = Vector3Int.zero;
        foreach (Vector3Int tilePosition in refineryArea.allPositionsWithin)
        {
            if (pb.IsWalkable(tilePosition))
            {
                walkablePosition = tilePosition;
                break;
            }
        }
        return walkablePosition;
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
            if (tilemap.HasTile(tilePosition) && tilemap.GetTile(tilePosition) == pb.tiles["urinium"] && pb.IsWalkable(tilePosition))
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


    // find all Refinery objects and find the nearest one
    public RefineryController FindNearestRefinery()
    {
        RefineryController nearestRefinery = null;
        float shortestDistance = Mathf.Infinity;
        foreach (RefineryController refinery in FindObjectsOfType<RefineryController>())
        {
            if (refinery.harvester != null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, refinery.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestRefinery = refinery;
            }
        }

        return nearestRefinery;
    }
}