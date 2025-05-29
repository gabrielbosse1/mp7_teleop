using UnityEngine;

public class RobotJoint : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis rotationAxis = Axis.Z;
    public float minAngle = -180f;
    public float maxAngle = 180f;
    public float currentAngle;
    public float angleFactor = 1f;

    public float offset = 0f;
    public bool invert = false;

    [Header("IK Control")]
    public bool enabledInSolver = true;

    /// <summary>
    /// Returns the current angle adjusted by offset and inversion.
    /// </summary>
    public float GetAdjustedAngle()
    {
        float angle = currentAngle;
        if (invert) angle *= -1f;
        angle += offset;
        angle *= angleFactor;
        return angle;
    }

    /// <summary>
    /// Allows external control of the joint angle, applying clamping and offset.
    /// </summary>
    public void SetAngleExternally(float externalAngle)
    {
        float convertedAngle = (externalAngle / angleFactor) - offset;

        if (invert)
            convertedAngle *= -1f;

        currentAngle = Mathf.Clamp(convertedAngle, minAngle, maxAngle);

        Vector3 currentEuler = transform.localEulerAngles;

        // Normalize angles to avoid jumps
        currentEuler.x = AngleUtils.NormalizeAngle(currentEuler.x);
        currentEuler.y = AngleUtils.NormalizeAngle(currentEuler.y);
        currentEuler.z = AngleUtils.NormalizeAngle(currentEuler.z);

        // Update only the configured axis
        switch (rotationAxis)
        {
            case Axis.X:
                currentEuler.x = currentAngle;
                break;
            case Axis.Y:
                currentEuler.y = currentAngle;
                break;
            case Axis.Z:
                currentEuler.z = currentAngle;
                break;
        }

        transform.localEulerAngles = currentEuler;
    }
}
