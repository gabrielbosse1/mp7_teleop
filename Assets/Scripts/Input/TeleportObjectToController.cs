using UnityEngine;

public class TeleportObjectToController : MonoBehaviour
{
    // The VR controller to track input and position from (e.g., left or right controller)
    public OVRInput.Controller controller;

    // Start is called once before the first frame update
    void Start()
    {
        // No initialization needed here currently
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Button One (usually the 'A' button on Oculus controllers) was released this frame on the specified controller
        if (OVRInput.GetUp(OVRInput.Button.One, controller)) {
            // Teleport this GameObject to the current position of the controller
            transform.position = OVRInput.GetLocalControllerPosition(controller);
        }
    }
}

