using UnityEngine; 
using UnityEditor; 

[CustomEditor(typeof(PAF_Player)), CanEditMultipleObjects]
public class PAF_PlayerEditor : Editor
{

    private SerializedProperty sightRange = null;
    private SerializedProperty fieldOfView = null;
    private SerializedProperty closeRange = null;


    private void OnEnable()
    {
        sightRange = serializedObject.FindProperty("sightRange");
        fieldOfView = serializedObject.FindProperty("fieldOfView");
        closeRange = serializedObject.FindProperty("closeRange"); 
    }

    private void OnSceneGUI()
    {
        Handles.color = new Color(1, 0, 1, .25f); 
        PAF_FlowerEditor.DrawFieldOfView((target as PAF_Player).transform.position, (target as PAF_Player).transform.forward, sightRange.floatValue, fieldOfView.intValue);
        Handles.DrawSolidDisc((target as PAF_Player).transform.position, Vector3.up, closeRange.floatValue);
    }
}
