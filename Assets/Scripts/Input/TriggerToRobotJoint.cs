using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerToRobotJoint : MonoBehaviour
{
    // Reference to the RobotJoint component that will be controlled
    public RobotJoint robotJoint;

    // The VR controller to read the trigger input from (e.g., left or right controller)
    public OVRInput.Controller controller;

    // Minimum angle limit for the robot joint movement
    public float minAngle;

    // Maximum angle limit for the robot joint movement
    public float maxAngle;

    // Called once before the first frame update
    void Start()
    {
        // If minAngle is approximately zero, initialize it from the robotJoint's configured minimum angle
        if (Mathf.Approximately(minAngle, 0f))
            minAngle = robotJoint.minAngle;

        // If maxAngle is approximately zero, initialize it from the robotJoint's configured maximum angle
        if (Mathf.Approximately(maxAngle, 0f))
            maxAngle = robotJoint.maxAngle;
    }

    // Called once per frame
    void Update()
    {
        // Get the current value of the primary index trigger on the specified controller
        // Value ranges from 0 (not pressed) to 1 (fully pressed)
        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);

        // Map the trigger value linearly from the range [0..1] to the angle range [minAngle..maxAngle]
        float mappedAngle = Mathf.Lerp(minAngle, maxAngle, triggerValue);

        // Apply the mapped angle to the RobotJoint via its external setter method
        robotJoint.SetAngleExternally(mappedAngle);
    }
}

