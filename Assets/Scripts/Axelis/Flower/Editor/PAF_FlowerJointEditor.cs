using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PAF_FlowerJoint))]
public class PAF_FlowerJointEditor : PropertyDrawer
{

    #region Fields and Properties
    SerializedProperty m_baseTransform = null; 
    SerializedProperty m_minAngle = null; 
    SerializedProperty m_maxAngle = null;
    SerializedProperty m_currentAngle = null; 
    #endregion

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIStyle _title = new GUIStyle();
        _title.fontStyle = FontStyle.Bold; 
        EditorGUI.DrawRect(position, new Color(.6f,.6f,.6f)); 

        m_baseTransform = property.FindPropertyRelative("m_baseTransform");
        m_minAngle = property.FindPropertyRelative("m_minAngle"); 
        m_maxAngle = property.FindPropertyRelative("m_maxAngle");
        m_currentAngle = property.FindPropertyRelative("m_currentAngle");

        Rect _rect = new Rect(position.position.x, position.position.y + 1, position.width, 15);
        EditorGUI.LabelField(_rect, $"JOINT {(m_baseTransform != null ? m_baseTransform.objectReferenceValue.name : string.Empty)}",_title);
        _rect = new Rect(position.position.x, position.position.y + 20, position.width, 15);
        EditorGUI.ObjectField(_rect, m_baseTransform);
        _rect = new Rect(position.position.x, position.position.y + 40, position.width, 15);
        EditorGUI.Slider(_rect, m_minAngle, -180, 180, "Min Angle");
        _rect = new Rect(position.position.x, position.position.y + 60, position.width, 15);
        EditorGUI.Slider(_rect, m_maxAngle, -180, 180, "Max Angle");
        _rect = new Rect(position.position.x, position.position.y + 80, position.width, 15);
        EditorGUI.LabelField(_rect, "Current Angle", m_currentAngle.floatValue.ToString()); 
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 100; 
    }
}
