using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(OptiTrackInputModule))]
public class OptiTrackInputModuleEditor : Editor
{
    SerializedProperty _scrollTreshold;
    SerializedProperty _scrollSpeed;
    SerializedProperty _waitOverTime;

    void OnEnable()
    {      

        _scrollSpeed = serializedObject.FindProperty("_scrollSpeed");
        _scrollTreshold = serializedObject.FindProperty("_scrollTreshold");
        _waitOverTime = serializedObject.FindProperty("_waitOverTime");
    }

   public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        serializedObject.Update();
        // Draw other properties
        EditorGUILayout.PropertyField(_scrollSpeed, new GUIContent("Scroll Speed"));
        EditorGUILayout.PropertyField(_scrollTreshold, new GUIContent("Scroll Treshold"));
        EditorGUILayout.PropertyField(_waitOverTime, new GUIContent("Wait Over Time"));
        serializedObject.ApplyModifiedProperties();
    }

}