/*-------------------------------------------------------------------------------------------
 * Copyright (c) Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Mochizuki.UProjection
{
    internal class TemplateCreator : EditorWindow
    {
        [SerializeField]
        private new string name;

        [SerializeField]
        private List<File> files;

        private Vector2 _scroll = Vector2.zero;

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Create a new uProjection template");
            EditorGUILayout.Space();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            name = EditorGUILayout.TextField("Project Name", name);

            var deletions = new List<int>();

            foreach (var file in files.ToArray().Select((w, i) => new { Index = i, Value = w }))
            {
                EditorGUILayout.LabelField($"File #{file.Index}");
                using (new EditorGUI.IndentLevelScope())
                {
                    file.Value.name = EditorGUILayout.TextField("File Name", file.Value.name ?? "");
                    file.Value.isDirectory = file.Value.name.EndsWith("/");
                    file.Value.source = file.Value.isDirectory ? null : file.Value.source;

                    if (!file.Value.isDirectory && file.Value.source == null)
                        file.Value.source = new SourceFile();

                    if (file.Value.source != null)
                    {
                        var oldAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(file.Value.source.uuid));
                        var newAsset = Utils.ObjectField("Object Reference", oldAsset);

                        file.Value.source.name = Path.GetFileName(AssetDatabase.GetAssetPath(newAsset));
                        file.Value.source.uuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newAsset));
                    }

                    if (GUILayout.Button("Delete a file"))
                        deletions.Add(file.Index);
                }
            }

            if (GUILayout.Button("Add a file"))
                files.Add(new File());

            if (GUILayout.Button("Create a new template"))
            {
                Utils.CreateTemplate(new ProjectTemplate { name = name, files = files });
                Close();
            }

            files = files.Where((w, i) => !deletions.Contains(i)).ToList();

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Mochizuki/uProjection/Template Creator")]
        public static void ShowWindow()
        {
            var window = CreateInstance<TemplateCreator>();
            window.titleContent = new GUIContent("uProjection Template Creator");
            window.files = new List<File>();
            window.ShowUtility();
        }
    }
}