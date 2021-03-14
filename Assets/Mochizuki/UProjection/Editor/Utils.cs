/*-------------------------------------------------------------------------------------------
 * Copyright (c) Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Mochizuki.UProjection
{
    internal static class Utils
    {
        private const string TemplatesDirectoryName = "Templates";
        private const string ProjectGuid = "21928d0dd1174e698d700ee57cd0e23b";

        private static readonly string ProjectRoot = Path.Combine(Application.dataPath, "..", AssetDatabase.GUIDToAssetPath(ProjectGuid));
        private static readonly string TemplatesDir = Path.Combine(ProjectRoot, TemplatesDirectoryName);

        public static void CreateTemplate(ProjectTemplate template)
        {
            CheckTemplatesDirExists();

            var guid = Guid.NewGuid();
            template.uuid = guid.ToString();

            using (var sw = new StreamWriter(Path.Combine(TemplatesDir, $"{guid}.json")))
                sw.WriteLine(JsonUtility.ToJson(template));

            AssetDatabase.Refresh();
        }

        public static void DeleteTemplate(ProjectTemplate template)
        {
            CheckTemplatesDirExists();

            var templateJson = Path.Combine(TemplatesDir, $"{template.uuid}.json");
            if (System.IO.File.Exists(templateJson))
                System.IO.File.Delete(templateJson);

            AssetDatabase.Refresh();
        }

        public static List<ProjectTemplate> LoadTemplates()
        {
            CheckTemplatesDirExists();

            var templates = new List<ProjectTemplate>();

            foreach (var file in Directory.GetFiles(TemplatesDir).Where(w => w.EndsWith(".json")))
                try
                {
                    using (var sr = new StreamReader(Path.Combine(TemplatesDir, file)))
                        templates.Add(JsonUtility.FromJson<ProjectTemplate>(sr.ReadToEnd()));
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }

            return templates;
        }

        public static void ExtractTemplate(ProjectTemplate template)
        {
            foreach (var directory in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var directoryPath = Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(directory).Replace("Assets/", ""));
                if (!Directory.Exists(directoryPath))
                    continue;

                foreach (var file in template.files.Where(w => !string.IsNullOrWhiteSpace(w.name)))
                    if (file.isDirectory)
                    {
                        var path = Path.Combine(directoryPath, file.name.Substring(0, file.name.Length - 1));
                        Directory.CreateDirectory(path);
                    }
                    else
                    {
                        var path = Path.Combine(directoryPath, file.name);
                        var basename = Path.GetDirectoryName(path) ?? throw new InvalidOperationException();
                        if (!Directory.Exists(basename))
                        {
                            Directory.CreateDirectory(basename);
                            AssetDatabase.Refresh();
                        }

                        if (file.source == null || string.IsNullOrWhiteSpace(file.source.uuid))
                        {
                            // create an empty file
                            using (var sr = new StreamWriter(path))
                                sr.WriteLine("");
                        }
                        else
                        {
                            // create from the source
                            var extension = Path.GetExtension(AssetDatabase.GUIDToAssetPath(file.source.uuid));
                            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(file.source.uuid), Path.Combine(AssetDatabase.GetAssetPath(directory), $"{file.name}{extension}"));
                        }

                        AssetDatabase.Refresh();
                    }
            }
        }

        private static void CheckTemplatesDirExists()
        {
            if (Directory.Exists(TemplatesDir))
                return;

            Directory.CreateDirectory(TemplatesDir);
            AssetDatabase.Refresh();
        }

        public static T ObjectField<T>(string label, T obj) where T : Object
        {
            return EditorGUILayout.ObjectField(new GUIContent(label), obj, typeof(T), false) as T;
        }
    }
}