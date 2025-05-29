using UnityEngine;

public class ShowAndHideMenu : MonoBehaviour
{
    // The VR controller to track input and position from (e.g., left or right controller)
    public OVRInput.Controller controller;

    public GameObject menu;

    // Boolean flag to track whether the menu should be shown or hidden
    public bool show;
    
    // Start is called once before the first frame update
    void Start()
    {
        // No initialization needed here currently
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Start button on the specified controller was released this frame
        if (OVRInput.GetUp(OVRInput.Button.Start, controller)) {
            // Toggle the 'show' flag to show or hide the menu
            show = !show;
        }

        if (show) {
            // Enable the Renderer component to make the menu visible
            menu.GetComponent<Renderer>().enabled = true;

            // Move the menu to the current position of the controller
            transform.position = OVRInput.GetLocalControllerPosition(controller);

            // Rotate the menu to match the current rotation of the controller
            transform.rotation = OVRInput.GetLocalControllerRotation(controller);
        } else {
            // Disable the Renderer component to hide the menu
            menu.GetComponent<Renderer>().enabled = false;
        }
    }
}

