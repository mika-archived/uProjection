/*-------------------------------------------------------------------------------------------
 * Copyright (c) Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Mochizuki.UProjection
{
    internal class TemplateGenerator : EditorWindow
    {
        private int _index;
        private Vector2 _scroll = Vector2.zero;
        private List<ProjectTemplate> _templates;

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Template Generator");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Select a uProjection Template");
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            _index = EditorGUILayout.Popup(new GUIContent("Template"), _index, _templates.Select(w => w.name).ToArray());

            if (0 <= _index && _index < _templates.Count)
            {
                var template = _templates[_index];
                foreach (var file in template.files.Select((w, i) => new { Index = i, Value = w }))
                {
                    EditorGUILayout.LabelField($"File #{file.Index}");
                    using (new EditorGUI.IndentLevelScope())
                    {
                        using (new EditorGUI.DisabledGroupScope(true))
                        {
                            EditorGUILayout.TextField("File Name", file.Value.name);
                            EditorGUILayout.Toggle("Is Directory", file.Value.isDirectory);

                            if (!file.Value.isDirectory)
                            {
                                var source = file.Value.source ?? new SourceFile();
                                Utils.ObjectField("File Source", AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(source.uuid)));
                            }
                        }
                    }
                }

                if (GUILayout.Button("Create from it"))
                {
                    Utils.ExtractTemplate(template);
                    Close();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Assets/Create/Create from uProjection Template")]
        public static void ShowWindow()
        {
            var window = CreateInstance<TemplateGenerator>();
            window.titleContent = new GUIContent("uProjection Template Generator");
            window._templates = Utils.LoadTemplates();
            window.ShowUtility();
        }
    }
}