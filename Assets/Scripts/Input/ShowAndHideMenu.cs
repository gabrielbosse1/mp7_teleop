using UnityEngine;

public class ShowAndHideMenu : MonoBehaviour
{
    // The VR controller to track input and position from (e.g., left or right controller)
    public OVRInput.Controller controller;

    // Reference to the menu GameObject (assumed to have a CanvasGroup for UI)
    public GameObject menu;

    // Cached CanvasGroup component to control menu visibility and interactivity
    private CanvasGroup canvasGroup;

    // Boolean flag to track whether the menu should be shown or hidden
    public bool show;

    void Start()
    {
        // Get the CanvasGroup component; add one if it doesn't exist
        canvasGroup = menu.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = menu.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        // Toggle the 'show' flag when the Start button on the specified controller is released
        if (OVRInput.GetUp(OVRInput.Button.Start, controller))
        {
            show = !show;
        }

        // Set the menu visibility and interactivity based on the 'show' flag
        canvasGroup.alpha = show ? 1f : 0f;              // Show or hide the menu visually
        canvasGroup.interactable = show;                  // Enable or disable interaction
        canvasGroup.blocksRaycasts = show;                // Enable or disable blocking raycasts (for clicks)

        if (show)
        {
            // Move the menu to the current position of the VR controller
            transform.position = OVRInput.GetLocalControllerPosition(controller);

            // Rotate the menu to match the current rotation of the VR controller
            transform.rotation = OVRInput.GetLocalControllerRotation(controller);
        }
    }
}
