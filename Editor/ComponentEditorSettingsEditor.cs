﻿using UnityEngine;
using UnityEditor;

namespace Toolbox.Editor
{
    using Toolbox.Editor.Internal;

    [CanEditMultipleObjects, CustomEditor(typeof(ComponentEditorSettings), true, isFallback = false)]
    public class ComponentEditorSettingsEditor : ComponentEditor
    {
        private SerializedProperty useOrderedDrawersProperty;
        private SerializedProperty useHierarchyEditorProperty;

        private ReorderableList drawHandlersList;


        protected override void OnEnable()
        {
            useOrderedDrawersProperty = serializedObject.FindProperty("useOrderedDrawers");
            useHierarchyEditorProperty = serializedObject.FindProperty("useHierarchyEditor");

            drawHandlersList = ToolboxEditorUtility.CreateBoxedList(serializedObject.FindProperty("drawHandlers"));
        }

        protected override void OnDisable()
        { }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(useOrderedDrawersProperty);
            EditorGUILayout.PropertyField(useHierarchyEditorProperty);
            if (useOrderedDrawersProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Select all wanted property drawers in specific order. " +
                        "Remember that they will be drawn in provided way, " +
                        "if any component drawer interrupts this cycle, " +
                        "then each next one will be ignored.", MessageType.Info);
    
                drawHandlersList.DoLayoutList();
            }
            serializedObject.ApplyModifiedProperties();
        }


        internal static class Style
        {
            static Style()
            {

            }
        }
    }
}