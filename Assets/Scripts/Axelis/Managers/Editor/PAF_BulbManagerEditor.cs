using UnityEngine; 
using UnityEditor; 

[CustomEditor(typeof(PAF_BulbManager))]
public class PAF_BulbManagerEditor : Editor 
{
    /* PAF_BulbManagerEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    private SerializedProperty m_bulbsPositions = null;
    private SerializedProperty m_bulbLimit = null;
    private SerializedProperty m_bulbDelay = null;
    #endregion

    #region Methods

    #region Original Methods
    private void DrawEditor()
    {
        GUIStyle _title = new GUIStyle();
        _title.fontStyle = FontStyle.Bold;
        _title.fontSize = 18;
        EditorGUILayout.BeginVertical("HelpBox"); 
        EditorGUILayout.LabelField("BULBS SETTINGS", _title);
        EditorGUILayout.Space(); 
        m_bulbLimit.intValue = EditorGUILayout.IntSlider("Bulb Limit", m_bulbLimit.intValue, 0, m_bulbsPositions.arraySize - 1);
        m_bulbDelay.floatValue = EditorGUILayout.Slider("Bulb Delay", m_bulbDelay.floatValue, 0.1f, 120.0f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("BULBS POSITIONS", _title);
        EditorGUILayout.Space();
        for (int i = 0; i < m_bulbsPositions.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            m_bulbsPositions.GetArrayElementAtIndex(i).vector3Value = EditorGUILayout.Vector3Field($"Bulb n°{i + 1}", m_bulbsPositions.GetArrayElementAtIndex(i).vector3Value);
            if (GUILayout.Button("X"))
            {
                m_bulbsPositions.DeleteArrayElementAtIndex(i); 
            }
            EditorGUILayout.EndHorizontal(); 
        }
        if (GUILayout.Button("Add New Position"))
        {
            m_bulbsPositions.InsertArrayElementAtIndex(m_bulbsPositions.arraySize);
        }
        EditorGUILayout.EndVertical(); 

    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        m_bulbsPositions = serializedObject.FindProperty("m_bulbsPositions");
        m_bulbLimit = serializedObject.FindProperty("m_bulbLimit");
        m_bulbDelay = serializedObject.FindProperty("m_bulbDelay");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); 

        DrawEditor();

        serializedObject.ApplyModifiedProperties(); 
    }

    public void OnSceneGUI()
    {
        for (int i = 0; i < m_bulbsPositions.arraySize; i++)
        {
            m_bulbsPositions.GetArrayElementAtIndex(i).vector3Value = Handles.PositionHandle(m_bulbsPositions.GetArrayElementAtIndex(i).vector3Value, Quaternion.identity);
            Handles.Label(m_bulbsPositions.GetArrayElementAtIndex(i).vector3Value + Vector3.up, $"Bulb Position n°{i + 1}"); 
        }
        serializedObject.ApplyModifiedProperties(); 
    }
    #endregion

    #endregion
}
