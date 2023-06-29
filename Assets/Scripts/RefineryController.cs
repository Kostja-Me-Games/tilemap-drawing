using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineryController : MonoBehaviour
{
    // harvester prefab
    public Building building;
    public GameObject harvesterPrefab;
    public GameObject harvester;
    public Vector3Int harvesterSpawnPoint;
    public bool harvesterSpawned = false;
    public bool harvesterSent = false;


    public void SetUnloadingHarvester(HarvesterController harvesterController)
    {
        harvester = harvesterController.gameObject;
    }

    public void ClearUnloadingHarvester()
    {
        harvester = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        building = GetComponent<Building>();
        harvesterSpawnPoint = new Vector3Int(2, 0, 0);
        
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (building.Placed && !harvesterSpawned)
        {
            harvesterSpawned = true;
            SpawnHarvester();
        }
        
    }

    private void FixedUpdate()
    {
        if (harvesterSpawned && !harvesterSent) {
            var position = harvester.transform.position;
            Vector3 harvesterMovePoint = new Vector3(position.x, position.y - 1, 0);
            harvester.GetComponent<RTSUnit>().MoveTo(harvesterMovePoint);
            harvesterSent = true;
            harvester.GetComponent<HarvesterController>().SetStateMovingToResource();
            harvester = null;
        }
    }

    public void SpawnHarvester()
    {
        Vector3 exactSpawnPoint = transform.position + harvesterSpawnPoint;
        harvester = Instantiate(harvesterPrefab, exactSpawnPoint, Quaternion.identity);
        // find a point to move the harvester to which must be 1 tile down from refinery
        
    }
}
