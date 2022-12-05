using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
* Dynamically changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 
*/

public class EnvironmentInteractions : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] float rayLength;

    Camera playerCamera;
    UI_BlockSelector blockSelector;

    private void Awake()
    {
        playerCamera = Camera.main;
    }

    private void Start()
    {
        blockSelector = FindObjectOfType<UI_BlockSelector>();
    }

    private void Update()
    {
        //Create the forward pointing ray
        Ray interactionRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hitInfo;

        //Cast the ray...
        if (Physics.Raycast(interactionRay, out hitInfo, rayLength))
        {
            //...if the ray hits an Interactable tagged gameObject
            if (hitInfo.transform.CompareTag("Interactable"))
            {
                //...and the player presses the E button...
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //...Cache the interaction script of the hit gameObject...
                    IInteractable interaction = hitInfo.collider.GetComponent<IInteractable>();

                    if (interaction != null)
                    {
                        //...and finally call its Interact() method.
                        interaction.PlaceBlock((BlockType)blockSelector.GetSelectedItem());
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    //...Cache the interaction script of the hit gameObject...
                    IInteractable interaction = hitInfo.collider.GetComponent<IInteractable>();

                    if (interaction != null)
                    {
                        //...and finally call its Interact() method.
                        interaction.DeleteBlock();
                    }
                }
            }
        }
    }
}
