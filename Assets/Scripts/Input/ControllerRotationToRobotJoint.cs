using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerRotationToRobotJoint : MonoBehaviour
{
    // Reference to the RobotJoint component to control
    public RobotJoint robotJoint;

    // The Oculus VR controller to get rotation from
    public OVRInput.Controller controller;

    // Minimum and maximum angles for the robot joint rotation
    public float minAngle;
    public float maxAngle;

    // Enum to select which axis of the controller rotation to use
    public enum Axis { X, Y, Z }
    public Axis rotationAxis = Axis.Z;

    // Offset angle to add to the controller rotation angle
    public float offset;

    // Option to invert the rotation input
    public bool invert = false;
    
    // Flag to enable or disable the rotation control
    public bool active = false;

    // Internal variable to store the current angle from the controller
    private float Angle;

    void Start()
    {
        // If minAngle or maxAngle are not set (0), initialize them from the RobotJoint limits
        if (Mathf.Approximately(minAngle, 0f))
            minAngle = robotJoint.minAngle;
        if (Mathf.Approximately(maxAngle, 0f))
            maxAngle = robotJoint.maxAngle;
    }

    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two, controller)) {
            active = !active;
        }
        // Only process rotation input if the controller is active
        if (active) {
            // Get the current rotation of the specified controller as a Quaternion
            Quaternion rotation = OVRInput.GetLocalControllerRotation(controller);

            // Convert the Quaternion rotation to Euler angles (in degrees)
            Vector3 euler = rotation.eulerAngles;

            // Extract the angle from the selected axis
            switch (rotationAxis)
            {
                case Axis.X:
                    Angle = euler.x;
                    break;
                case Axis.Y:
                    Angle = euler.y;
                    break;
                case Axis.Z:
                    Angle = euler.z;
                    break;
            }

            // Add the offset to the angle
            Angle += offset;

            // Normalize the angle to be within -180 to 180 degrees for consistent mapping
            Angle = AngleUtils.NormalizeAngle(Angle);

            // If invert option is enabled, invert the angle value
            if (invert)
            {
                Angle = -Angle;
            }

            // Map the normalized angle from controller space (-180 to 180) to the robot joint's allowed range (minAngle to maxAngle)
            float mappedAngle = AngleUtils.Map(Angle, -180f, 180f, minAngle, maxAngle);

            // Apply the mapped angle to the robot joint externally
            robotJoint.SetAngleExternally(mappedAngle);
        }
    }
}

