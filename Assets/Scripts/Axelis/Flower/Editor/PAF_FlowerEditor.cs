using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(PAF_Flower))]
public class PAF_FlowerEditor : Editor 
{
    /* PAF_FlowerEditor :
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
    private SerializedProperty m_detectionRange = null;
    private SerializedProperty m_eatingRange = null;
    private SerializedProperty m_fieldOfView = null;
    private SerializedProperty m_joints = null; 
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draw the field of view as a solid arc from the origin, oriented with the local forward with a range and an angle
    /// </summary>
    /// <param name="_origin">Origin of the arc</param>
    /// <param name="_localForward">local forward of the arc</param>
    /// <param name="_range">Range of the arc</param>
    /// <param name="_angle">Angle of the arc</param>
    private void DrawFieldOfView(Vector3 _origin, Vector3 _localForward, float _range, int _angle)
    {
        float _totalAngle = Vector3.SignedAngle(Vector3.forward, _localForward, Vector3.up) - (_angle / 2);
        Vector3 _start = new Vector3(Mathf.Sin(_totalAngle * Mathf.Deg2Rad), 0, Mathf.Cos(_totalAngle * Mathf.Deg2Rad)).normalized;
        Handles.DrawSolidArc(_origin, Vector3.up, _start, _angle, _range);
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        m_fieldOfView = serializedObject.FindProperty("m_fieldOfView");
        m_detectionRange = serializedObject.FindProperty("m_detectionRange");
        m_eatingRange = serializedObject.FindProperty("m_eatingRange");
        m_joints = serializedObject.FindProperty("m_joints"); 
    }

    private void OnSceneGUI()
    {
        Handles.color = new Color(1, 0, 0, .5f); 
        DrawFieldOfView((serializedObject.targetObject as PAF_Flower).transform.position, (serializedObject.targetObject as PAF_Flower).transform.forward, m_detectionRange.floatValue, m_fieldOfView.intValue);
        Handles.color = new Color(0, 0, 1, .5f);
        DrawFieldOfView((serializedObject.targetObject as PAF_Flower).transform.position, (serializedObject.targetObject as PAF_Flower).transform.forward, m_eatingRange.floatValue, m_fieldOfView.intValue);
    }
    #endregion

    #endregion
}
