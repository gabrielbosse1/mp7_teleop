using UnityEngine;

public class StayOrFollowController : MonoBehaviour
{
    // The VR controller to track input and position from (e.g., left or right controller)
    public OVRInput.Controller controller;

    // Flag to determine whether the object should follow the controller's position or stay still
    public bool Follow = false;

    // Start is called once before the first frame update
    void Start()
    {
        // No initialization needed here currently
    }

    // Update is called once per frame
    void Update()
    {
        // Check if Button Two (usually the 'B' button on Oculus controllers) was released this frame on the specified controller
        if (OVRInput.GetUp(OVRInput.Button.Two, controller)) {
            // Toggle the Follow flag to switch between following and staying still
            Follow = !Follow;
        }

        // If Follow is true, update this GameObject's position to match the controller's current position
        if (Follow) {
            transform.position = OVRInput.GetLocalControllerPosition(controller);
        }
    }
}

