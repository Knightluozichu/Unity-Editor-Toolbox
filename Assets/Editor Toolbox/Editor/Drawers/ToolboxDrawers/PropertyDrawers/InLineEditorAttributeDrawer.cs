﻿using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Drawers
{
    using Toolbox.Editor.Internal;

    using Editor = UnityEditor.Editor;

    public class InLineEditorAttributeDrawer : ToolboxSelfPropertyDrawer<InLineEditorAttribute>
    {
        static InLineEditorAttributeDrawer()
        {
            storage = new DrawerDataStorage<Editor, InLineEditorAttribute>(false,
                (p, a) =>
                {
                    var value = p.objectReferenceValue;
                    if (value)
                    {
                        var editor = Editor.CreateEditor(value);
                        if (editor.HasPreviewGUI())
                        {
                            editor.ReloadPreviewInstances();
                        }

                        return editor;
                    }
                    else
                    {
                        return null;
                    }
                },
                (e) =>
                {
                    Object.DestroyImmediate(e);
                });
        }

        private static readonly DrawerDataStorage<Editor, InLineEditorAttribute> storage;


        private void DrawEditor(Editor editor, InLineEditorAttribute attribute)
        {
            using (new EditorGUILayout.VerticalScope(Style.inlinedStyle))
            {
                //draw and prewarm the inlined Editor version
                DrawEditor(editor, attribute.DrawPreview, attribute.DrawSettings, attribute.PreviewHeight);
            }
        }

        private void DrawEditor(Editor editor, bool drawPreview, bool drawSettings, float previewHeight)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                //draw the whole inspector and apply changes 
                editor.serializedObject.Update();
                editor.OnInspectorGUI();
                editor.serializedObject.ApplyModifiedProperties();

                if (editor.HasPreviewGUI())
                {
                    //draw the preview if possible and needed
                    if (drawPreview)
                    {
                        editor.OnPreviewGUI(EditorGUILayout.GetControlRect(false, previewHeight), Style.previewStyle);
                    }

                    if (drawSettings)
                    {
                        //draw additional settings associated to the Editor
                        //for example:
                        // - audio management for the AudioClip
                        // - model settings within the Previews
                        using (new EditorGUILayout.HorizontalScope(Style.settingStyle))
                        {
                            editor.OnPreviewSettings();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handles the property drawing process and tries to create a inlined version of the <see cref="Editor"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        protected override void OnGuiSafe(SerializedProperty property, GUIContent label, InLineEditorAttribute attribute)
        {
            //create a standard property field for given property
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, label, property.isExpanded);
            if (EditorGUI.EndChangeCheck())
            {
                //make sure previously cached Editor is disposed
                storage.ClearItem(property);
            }

            //NOTE: multiple different Editors are not supported
            if (property.hasMultipleDifferentValues)
            {
                return;
            }

            var propertyValue = property.objectReferenceValue;
            if (propertyValue == null)
            {
                return;
            }

            //create additional foldout for the associated Editor 
            if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, Style.foldouContent, true, Style.foldoutStyle))
            {
                var editor = storage.ReturnItem(property, attribute);
                InspectorUtility.SetIsEditorExpanded(editor, true);

                //make useage of the created (returned) instance
                using (new FixedFieldsScope())
                {
                    DrawEditor(editor, attribute);
                }
            }
        }


        public override bool IsPropertyValid(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }


        private static class Style
        {
            internal static readonly GUIStyle inlinedStyle;
            internal static readonly GUIStyle foldoutStyle;
            internal static readonly GUIStyle previewStyle;
            internal static readonly GUIStyle settingStyle;

            internal static readonly GUIContent foldouContent = new GUIContent("Inspector Preview");

            static Style()
            {
                inlinedStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(13, 13, 8, 8)
                };
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 10,
#else
                    fontSize = 9,
#endif
                    alignment = TextAnchor.MiddleLeft
                };

                previewStyle = new GUIStyle();
                previewStyle.normal.background = EditorGuiUtility.CreateColorTexture();

                settingStyle = new GUIStyle()
                {
#if UNITY_2019_3_OR_NEWER
                    padding = new RectOffset(4, 0, 0, 0)
#else
                    padding = new RectOffset(5, 0, 0, 0)
#endif
                };
            }
        }
    }
}