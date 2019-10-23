using System.Collections;
using System.Linq; 
using UnityEngine; 
public static class PAF_ProceduralAnimationHelper
{
    public const float SAMPLING_DISTANCE = .1f;
    public const float LEARNING_RATE = 10f; 

    /// <summary>
    /// CHECKED
    /// Get the last point position if every parent is rotated by a certain angle
    /// </summary>
    /// <param name="_joints">Every joints</param>
    /// <param name="_angles">Angle of every joints</param>
    /// <returns></returns>
    public static Vector3 GetForwardKinematic(PAF_FlowerJoint[] _joints, float[] _angles)
   {
        Vector3 _prevPoint = _joints[0].BaseTransform.position;
        Vector3 _nextPoint = Vector3.zero; 
        Quaternion _rotation = _joints[0].BaseTransform.rotation;
        for (int i = 1; i < _joints.Length; i++)
        {
            _rotation *= Quaternion.AngleAxis(_angles[i - 1], _joints[i - 1].Axis);
            _prevPoint = _prevPoint + (_rotation * _joints[i].StartOffset);
        }
        return _prevPoint; 
   }

    public static float PartialGradiant(Vector3 _target, PAF_FlowerJoint[] _joints, float[] _angles, int _i)
    {
        float _angle = _angles[_i];
        // Gradiant = [F(x+samplingDistance) - F(x)] / h
        float f_x = DistanceFromTarget(_target, _joints, _angles);

        _angles[_i] += SAMPLING_DISTANCE;
        float f_xPlusD = DistanceFromTarget(_target, _joints, _angles);

        _angles[_i] = _angle; 

        return (f_xPlusD - f_x) / SAMPLING_DISTANCE;
    }

    public static float DistanceFromTarget(Vector3 target, PAF_FlowerJoint[] _joints,  float[] angles)
    {
        Vector3 point = GetForwardKinematic(_joints, angles);
        return Vector3.Distance(point, target);
    }
	
    public static void InverseKinematics (Vector3 _target, PAF_FlowerJoint[] _joints, float [] _angles, float _distanceThreshold)
    {
        //Debug.LogError(_target + " -> " + GetForwardKinematic(_joints, _angles) + " = " + DistanceFromTarget(_target, _joints, _angles));

        if (DistanceFromTarget(_target, _joints, _angles) < _distanceThreshold)
        {
            return;
        }

        for (int i = _joints.Length -1; i >= 0; i --)
        {
            // Gradient descent
            // Update : Solution -= LearningRate * Gradient
            float gradient = PartialGradiant(_target, _joints, _angles, i);
            _angles[i] -= (LEARNING_RATE * gradient);

            // Clamp
            //_angles[i] = Mathf.Clamp(_angles[i], _joints[i].MinAngle, _joints[i].MaxAngle);
            _joints[i].BaseTransform.localRotation = Quaternion.RotateTowards(_joints[i].BaseTransform.localRotation, Quaternion.AngleAxis(_angles[i], _joints[i].Axis), .1f); 
            
            // Early termination
            if (DistanceFromTarget(_target, _joints , _angles) < _distanceThreshold)
                return;
        }
    }
}
