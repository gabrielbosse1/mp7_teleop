using UnityEngine;

public class RotateObjectWithController : MonoBehaviour
{
    // Variable to store rotation step amount per frame, adjusted by frame time
    private float step;

    // Update is called once per frame
    void Update()
    {
        // Calculate rotation step based on a speed factor (5.0f) and frame time to ensure smooth rotation
        step = 5.0f * Time.deltaTime;

        // Check if the primary thumbstick is being pressed to the left
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))
            // Rotate the object around the Y-axis positively (to the left) by step amount
            transform.Rotate(0, 5.0f * step, 0);

        // Check if the primary thumbstick is being pressed to the right
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))
            // Rotate the object around the Y-axis negatively (to the right) by step amount
            transform.Rotate(0, -5.0f * step, 0);
    }
}

