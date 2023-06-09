using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Building : MonoBehaviour
{
    public bool Placed { get; private set; }
    ConstructionPropertiesController constructionPropertiesController;
    public BoundsInt area;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    private Camera _camera;
    
    private void OnMouseDown()
    {
        if (!Placed)
        {
        }
    }

    private void OnMouseDrag()
    {
    }

    private void OnMouseUp()
    {
        if (!Placed)
        {
            Vector3Int cellPosition = Paintbrush.current.gridLayout.LocalToCell(transform.position);
            transform.localPosition =
                Paintbrush.current.gridLayout.CellToLocalInterpolated(cellPosition + new Vector3(.5f, .5f, 0f));
        }
    }

    public bool CanBePlacedHere()
    {
        BoundsInt newArea = area;
        newArea.position = Paintbrush.current.gridLayout.LocalToCell(transform.position);
        return Paintbrush.current.AreaCanBeTaken(newArea);
    }

    public void Place()
    {
        if (!Placed)
        {
            bool result = constructionPropertiesController.Build();
            if (result)
            {
                Placed = true;
                ChangeOpacity(1f);
                Paintbrush.current.TakeArea(area);
                // get animator and set the "Placed" trigger
                if (animator != null)
                {
                    animator.SetBool("Placed", true);
                }

            }
        }
    }

    private void ChangeOpacity(float opacity)
    {
        // Get the current color of the sprite
        Color spriteColor = spriteRenderer.color;

        // Set the new alpha value (opacity)
        spriteColor.a = opacity;

        // Assign the updated color back to the sprite
        spriteRenderer.color = spriteColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        constructionPropertiesController = GetComponent<ConstructionPropertiesController>();
        // try / catch to avoid errors if the animator or sprite renderer are not found
        try
        {
            animator = transform.Find("Sprite").GetComponent<Animator>();
        }
        catch
        {
            Debug.Log("No animator found for building " + gameObject.name);
        }

        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        ChangeOpacity(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Placed)
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            Vector3Int cellPosition = Paintbrush.current.gridLayout.LocalToCell(transform.position);
            transform.localPosition =
                Paintbrush.current.gridLayout.CellToLocalInterpolated(cellPosition + new Vector3(.5f, .5f, 0f));
            Paintbrush.current.FollowBuilding();
        }
    }
}