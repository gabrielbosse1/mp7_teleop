using UnityEngine;

public static class RobotJointExtensions
{
    /// <summary>
    /// Converts the RobotJoint.Axis enum to a Vector3 axis.
    /// </summary>
    public static Vector3 ToVector(this RobotJoint.Axis axis)
    {
        return axis switch
        {
            RobotJoint.Axis.X => Vector3.right,
            RobotJoint.Axis.Y => Vector3.up,
            RobotJoint.Axis.Z => Vector3.forward,
            _ => Vector3.forward
        };
    }

    /// <summary>
    /// Gets the normalized angle from euler angles for the specified axis.
    /// </summary>
    public static float GetAngleFromEuler(Vector3 euler, RobotJoint.Axis axis)
    {
        return axis switch
        {
            RobotJoint.Axis.X => AngleUtils.NormalizeAngle(euler.x),
            RobotJoint.Axis.Y => AngleUtils.NormalizeAngle(euler.y),
            RobotJoint.Axis.Z => AngleUtils.NormalizeAngle(euler.z),
            _ => 0
        };
    }
}
