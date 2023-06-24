using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionPropertiesController : MonoBehaviour
{
    public int cost;
    public int secondsToBuild;
    CreditsController creditsController;
    public bool enoughCreditsToBuild()
    {
        return !CreditsController.instance.CheckEnoughCredits(cost);
    }
    public bool Build()
    {
        
            return CreditsController.instance.SubtractCredits(cost);
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // get credits controller
        creditsController = GetComponent<CreditsController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
