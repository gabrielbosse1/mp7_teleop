using System.Collections.Generic;
using UnityEngine;

public class CCDIKSolver : MonoBehaviour
{
    [Header("IK Settings")]
    public Transform target;
    public Transform endEffector;
    public int maxIterations = 10;
    public float tolerance = 0.01f;
    [Range(0.1f, 1f)] public float damping = 0.9f;

    [Header("Optimization")]
    public bool minimizeAngles = true;
    public float angleThreshold = 5f;

    private List<Transform> joints = new List<Transform>();
    private List<RobotJoint> jointSettings = new List<RobotJoint>();

    void Start()
    {
        InitializeJoints();
        InitializeJointAngles();
    }

    /// <summary>
    /// Collects all joints from end effector up to this transform.
    /// </summary>
    void InitializeJoints()
    {
        Transform current = endEffector;
        while (current != null && current != transform)
        {
            joints.Insert(0, current);
            jointSettings.Insert(0, current.GetComponent<RobotJoint>());
            current = current.parent;
        }
    }

    /// <summary>
    /// Initializes each joint's current angle from local rotation.
    /// </summary>
    void InitializeJointAngles()
    {
        foreach (var joint in jointSettings)
        {
            Vector3 euler = joint.transform.localRotation.eulerAngles;
            joint.currentAngle = joint.rotationAxis switch
            {
                RobotJoint.Axis.X => AngleUtils.NormalizeAngle(euler.x),
                RobotJoint.Axis.Y => AngleUtils.NormalizeAngle(euler.y),
                RobotJoint.Axis.Z => AngleUtils.NormalizeAngle(euler.z),
                _ => 0
            };
        }
    }

    void LateUpdate()
    {
        SolveIK();
    }

    /// <summary>
    /// Performs the CCD IK solving algorithm with optional angle minimization.
    /// </summary>
    void SolveIK()
    {
        if (joints.Count == 0 || target == null) return;

        for (int i = 0; i < maxIterations; i++)
        {
            bool solutionFound = false;

            // Backward pass
            for (int j = joints.Count - 1; j >= 0; j--)
            {
                Transform joint = joints[j];
                RobotJoint settings = jointSettings[j];

                if (!settings.enabledInSolver) continue;

                Quaternion deltaRot = CalculateDeltaRotation(joint, settings);
                if (deltaRot != Quaternion.identity)
                {
                    ApplyOptimalRotation(joint, settings, deltaRot);
                }

                if (Vector3.Distance(endEffector.position, target.position) < tolerance)
                {
                    solutionFound = true;
                    break;
                }
            }

            if (solutionFound) break;

            // Forward pass (optional)
            if (minimizeAngles && i % 2 == 0)
            {
                for (int j = 0; j < joints.Count; j++)
                {
                    Transform joint = joints[j];
                    RobotJoint settings = jointSettings[j];

                    if (!settings.enabledInSolver) continue;

                    Quaternion deltaRot = CalculateDeltaRotation(joint, settings);
                    if (deltaRot != Quaternion.identity)
                    {
                        ApplyOptimalRotation(joint, settings, deltaRot, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculates the delta rotation needed to align the joint's end effector direction toward the target.
    /// </summary>
    Quaternion CalculateDeltaRotation(Transform joint, RobotJoint settings)
    {
        Vector3 toEnd = endEffector.position - joint.position;
        Vector3 toTarget = target.position - joint.position;

        if (toEnd.magnitude > 0.001f && toTarget.magnitude > 0.001f)
        {
            return Quaternion.FromToRotation(toEnd, toTarget);
        }
        return Quaternion.identity;
    }

    /// <summary>
    /// Applies the calculated rotation to the joint, respecting damping and angle limits.
    /// </summary>
    void ApplyOptimalRotation(Transform joint, RobotJoint settings, Quaternion deltaRot, bool reversePass = false)
    {
        if (!settings.enabledInSolver) return;

        Vector3 axis = settings.rotationAxis.ToVector();
        float angleBefore = settings.currentAngle;

        Quaternion newRot = Quaternion.Lerp(Quaternion.identity, deltaRot, damping) * joint.rotation;
        Vector3 newEuler = (Quaternion.Inverse(joint.parent.rotation) * newRot).eulerAngles;

        float rawAngle = RobotJointExtensions.GetAngleFromEuler(newEuler, settings.rotationAxis);
        float clampedAngle = AngleUtils.ClampAngle(rawAngle, settings.minAngle, settings.maxAngle);

        float deltaAngle = Mathf.DeltaAngle(angleBefore, clampedAngle);
        float finalAngle = angleBefore + deltaAngle * damping;

        if (minimizeAngles && Mathf.Abs(deltaAngle) > angleThreshold)
        {
            finalAngle = angleBefore + Mathf.Sign(deltaAngle) * angleThreshold;
        }

        finalAngle = AngleUtils.ClampAngle(finalAngle, settings.minAngle, settings.maxAngle);
        settings.currentAngle = finalAngle;

        joint.localRotation = Quaternion.Euler(
            settings.rotationAxis == RobotJoint.Axis.X ? finalAngle : 0,
            settings.rotationAxis == RobotJoint.Axis.Y ? finalAngle : 0,
            settings.rotationAxis == RobotJoint.Axis.Z ? finalAngle : 0
        );

        if (reversePass)
        {
            joint.localRotation = Quaternion.Slerp(
                joint.localRotation,
                Quaternion.Euler(axis * finalAngle),
                0.5f
            );
        }
    }
}
