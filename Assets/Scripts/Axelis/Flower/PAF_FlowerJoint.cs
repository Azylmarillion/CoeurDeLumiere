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

    [SerializeField]
    private float m_currentAngle = 0; 
    public float CurrentAngle
    { 
        get 
        { 
            return m_currentAngle; 
        }
        set
        {
            m_currentAngle = value; 
        }
    }

    public float MinAngle { get { return m_minAngle;  } }
    public float MaxAngle { get { return m_maxAngle; } }


    public void Init()
    {
        Axis = new Vector3(0, 1, 0); 
        StartOffset = m_baseTransform.localPosition;
    }
}
