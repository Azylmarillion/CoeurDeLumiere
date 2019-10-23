using System;
using UnityEngine; 

[Serializable]
public class PAF_FlowerJoint
{
    [SerializeField] private Transform m_baseTransform = null; 
    public Transform BaseTransform { get { return m_baseTransform; } }
    public Vector3 Axis { get; private set; } 
    public Vector3 StartOffset { get; private set; }

    [SerializeField]
    private float m_minAngle = 0;
    [SerializeField]
    private float m_maxAngle = 180;

    public float MinAngle { get { return m_minAngle;  } }
    public float MaxAngle { get { return m_maxAngle; } }


    public void Init()
    {
        m_minAngle = -45;
        m_maxAngle = 45; 
        Axis = new Vector3(0, 1, 0); 
        StartOffset = m_baseTransform.localPosition;
    }
}
