using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsController : MonoBehaviour
{

    // singleton pattern
    public static CreditsController instance;

    public int credits;
    public TextMeshProUGUI textMesh;
    public void AddCredits(int amount)
    {
        // check that amount is positive
        if (amount < 0)
        {
            Debug.LogError("Cannot add negative credits");
            return;
        }
        credits += amount;
    }

    public bool SubtractCredits(int amount)
    {
        // check that amount is positive
        if (amount < 0)
        {
            Debug.LogError("Cannot subtract negative credits");
            return false;
        }
        if (CheckEnoughCredits(amount))
        {
            Debug.LogError("Cannot subtract credits when there are not enough credits");
            return false;
        }
        credits -= amount;
        return true;
    }

    public bool CheckEnoughCredits(int amount)
    {
        return credits < amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        // set instance variable to this instance of the class
        instance = this;
        // set textMesh variable to the TextMesh component on the same GameObject
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // convert credits to a string and set the textMesh text to that string
        textMesh.text = credits.ToString();
        
    }
}
