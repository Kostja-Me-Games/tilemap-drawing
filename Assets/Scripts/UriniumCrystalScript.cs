using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UriniumCrystalScript : MonoBehaviour
{
    public BoundsInt area;
    public Paintbrush pb;
    public float urinium = 2000;
    // Start is called before the first frame update
    void Start()
    {
      // find game object called "Grid" and get the paintbrush component

        pb = GameObject.Find("Grid").GetComponent<Paintbrush>();   
        BoundsInt newArea = pb.GetAreaByPosition(
            transform.position,
            area
            );
        pb.TakeAreaTile(newArea, "urinium");
    }
    public float Harvest(float amount) {
        // reduce amount of urinium by amount, save the amount of mined urinium in a variable
        // can't mine more than the amount of urinium left
        if (amount > urinium) {
            amount = urinium;
        }
        urinium -= amount;
        if (urinium <= 0) {
            // remove the tile under it
            BoundsInt newArea = pb.GetAreaByPosition(
                transform.position,
                area
                );
            pb.ClearAreaUnderUrinium(newArea);
            Destroy(gameObject);
        }

        return amount;
    }
    void Awake() {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
