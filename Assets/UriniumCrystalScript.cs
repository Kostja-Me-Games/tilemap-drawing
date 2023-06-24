using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UriniumCrystalScript : MonoBehaviour
{
    public BoundsInt area;
    public paintbrush pb;
    // Start is called before the first frame update
    void Start()
    {
      // find game object called "Grid" and get the paintbrush component

        pb = GameObject.Find("Grid").GetComponent<paintbrush>();   
        BoundsInt newArea = pb.GetAreaByPosition(
            transform.position,
            area
            );
        pb.TakeAreaTile(newArea, "urinium");
    }
    void Awake() {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
