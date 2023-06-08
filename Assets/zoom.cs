using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoom : MonoBehaviour
{
    public float zoomSize = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (zoomSize > 2)
            {
                zoomSize -= 1;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (zoomSize < 10)
            {
                zoomSize += 1;
            }
        }


        Camera.main.orthographicSize = zoomSize;
    }
}
