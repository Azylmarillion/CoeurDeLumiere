using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PAF_Fruit)), CanEditMultipleObjects]
public class PAF_FruitEditor : Editor
{
    #region Fields / Properties

    #region Serialized Properties

    #region Editor
    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="Color"/>.</summary>
    SerializedProperty gizmosColor = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="List{T}"/> of <see cref="Vector3"/>.</summary>
    SerializedProperty collisionPos = null;
    #endregion

    #region Parameters
    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="Transform"/>.</summary>
    SerializedProperty renderer = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="SphereCollider"/>.</summary>
    SerializedProperty collider = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="Rigidbody"/>.</summary>
    SerializedProperty rigidbody = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="bool"/>.</summary>
    SerializedProperty doFreezeXRotation = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="bool"/>.</summary>
    SerializedProperty isFalling = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="bool"/>.</summary>
    SerializedProperty isGolden = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="int"/>.</summary>
    SerializedProperty fruitScore = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="LayerMask"/>.</summary>
    SerializedProperty whatCollide = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="LayerMask"/>.</summary>
    SerializedProperty whatIsGround = null;

    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="Vector3"/>.</summary>
    SerializedProperty velocity = null;
    #endregion

    #region Sounds
    /// <summary>SerializedProperty from class <see cref="PAF_Fruit"/> of type <see cref="AudioSource"/>.</summary>
    SerializedProperty audioSource = null;
    #endregion

    #endregion

    #region Parameters
    /// <summary>
    /// Vector3 used to add velocity to the editing fruit(s).
    /// </summary>
    private Vector3 velocityImpulse = Vector3.zero;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the buttons area in the inspector of the editing fruit(s).
    /// </summary>
    private void DrawButtonsEditor()
    {
        if (!Application.isPlaying) return;

        GUILayout.Space(15);

        if (collisionPos.arraySize > 1)
        {
            GUI.color = new Color(.9f, .25f, .25f);
            if (GUILayout.Button(new GUIContent("Clear Gizmos", "Remove all drawn gizmos of past positions."), GUILayout.Width(250)))
            {
                collisionPos.arraySize = 0;
            }
        }

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        Color _originalColor = GUI.color;
        GUILayout.Space(75);
        
        EditorGUILayout.BeginVertical();

        velocityImpulse = EditorGUILayout.Vector3Field(string.Empty, velocityImpulse, GUILayout.Width(150));

        GUI.color = new Color(.25f, .9f, .25f);
        if (GUILayout.Button(new GUIContent("Add Velocity", "Adds this velocity to all editing fruit(s)."), GUILayout.Width(150)))
        {
            serializedObject.targetObjects.ToList().ForEach(f => ((PAF_Fruit)f).AddForce(velocityImpulse));
        }

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();

        if (velocity.vector3Value != Vector3.zero)
        {
            GUI.color = new Color(.9f, .25f, .25f);
            if (GUILayout.Button(new GUIContent("Reset Velocity", "Reset to zero the velocity of this fruit."), GUILayout.Width(250)))
            {
                serializedObject.targetObjects.ToList().ForEach(f => ((PAF_Fruit)f).Velocity = Vector3.zero);
            }
        }

        GUILayout.Space(75);
        GUI.color = _originalColor;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(15);
    }

    /// <summary>
    /// Draws the editor of the editing script(s).
    /// </summary>
    private void DrawEditor()
    {
        serializedObject.Update();

        DrawEditorEditor();
        DrawButtonsEditor();
        DrawParametersEditor();

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draws the inspector of the editor part of the editing fruit(s).
    /// </summary>
    private void DrawEditorEditor()
    {
        EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);

        GUILayout.Space(5);

        EditorGUILayout.PropertyField(gizmosColor, new GUIContent("Gizmos Color", "Color used to draw this fruit gizmos."));
    }

    /// <summary>
    /// Draws the inspector of the parameters of the editing fruit(s).
    /// </summary>
    private void DrawParametersEditor()
    {
        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(audioSource, new GUIContent("Audio Source", "Audio Source of the fruit."));
        EditorGUILayout.PropertyField(renderer, new GUIContent("Renderer", "Renderer transform of the fruit."));
        EditorGUILayout.PropertyField(rigidbody, new GUIContent("Rigidbody", "Rigidbody of the fruit."));
        EditorGUILayout.PropertyField(collider, new GUIContent("Collider", "Sphere collider of the fruit."));
        EditorGUILayout.PropertyField(whatCollide, new GUIContent("Collide Layers", "Layermask used to indicate what should the object detect or not."));
        EditorGUILayout.PropertyField(whatIsGround, new GUIContent("Ground Layers", "Layermask used to indicate what is ground."));


        GUILayout.Space(5);
        GUI.enabled = false;

        GUI.enabled = true;
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(isGolden, new GUIContent("Golden", "Indicates if the fruit is a golden one or not."));
        EditorGUILayout.PropertyField(doFreezeXRotation, new GUIContent("Freeze X Rot.", "Should the X rotation of the obejct be frozenor not."));

        GUILayout.Space(5);

        EditorGUILayout.PropertyField(fruitScore, new GUIContent("Fruit Score", "Amount of point this fruit gives to a player when eaten."));

        GUILayout.Space(5);
        
        if (EditorGUILayout.PropertyField(velocity, new GUIContent("Velocity", "Current velocity of the fruit.")))
        {
            serializedObject.targetObjects.ToList().ForEach(f => ((PAF_Fruit)f).Velocity = velocity.vector3Value);
        }

        EditorGUILayout.Toggle(new GUIContent("Falling", "Is the object falling down or not."), isFalling.boolValue, EditorStyles.radioButton);
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get required properties
        gizmosColor = serializedObject.FindProperty("gizmosColor");
        collisionPos = serializedObject.FindProperty("collisionPos");

        audioSource = serializedObject.FindProperty("audioSource");

        renderer = serializedObject.FindProperty("renderer");
        collider = serializedObject.FindProperty("collider");
        rigidbody = serializedObject.FindProperty("rigidbody");
        doFreezeXRotation = serializedObject.FindProperty("doFreezeXRotation");
        isFalling = serializedObject.FindProperty("isFalling");
        isGolden = serializedObject.FindProperty("isGolden");
        fruitScore = serializedObject.FindProperty("fruitScore");
        whatCollide = serializedObject.FindProperty("whatCollide");
        whatIsGround = serializedObject.FindProperty("whatIsGround");
        velocity = serializedObject.FindProperty("velocity");
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}
