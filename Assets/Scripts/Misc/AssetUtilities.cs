﻿using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace JakePerry
{
    public static class AssetUtilities
    {
        /// <summary>Returns the original path without an Assets/ prefix.</summary>
        private static string CleanAssetsPath(string path)
        {
            if (path.StartsWith("Assets/"))
                path = path.Remove(0, 7);

            if (path.StartsWith("/"))
                path = path.Remove(0, 1);

            return path;
        }

        /// <summary>Checks if the asset path exists, and creates it if not.<para>
        /// Path must be relative to the Assets folder and use forward slashes ('/') to separate folders.</para></summary>
        public static void EnsurePathExists(string path)
        {
            // Remove Assets prefix
            path = CleanAssetsPath(path);

            string[] segments = path.Split(new char[] { '/' });
            string currentWorkingPath = "Assets";

            for (int i = 0; i < segments.Length; i++)
            {
                if (string.IsNullOrEmpty(segments[i])) continue;

                string nextWorkingPath = string.Concat(currentWorkingPath, "/", segments[i]);
                if (!AssetDatabase.IsValidFolder(nextWorkingPath))
                    AssetDatabase.CreateFolder(currentWorkingPath, segments[i]);

                currentWorkingPath = nextWorkingPath;
            }

            AssetDatabase.Refresh();
        }

        /// <summary>Ensures the path exists, then creates the asset.</summary>
        /// <param name="asset">Object to use in creating the asset.</param>
        /// <param name="path">Path for the new asset, relative to the Assets folder.</param>
        /// <param name="fileName">File name, including extension.</param>
        /// <param name="overwrite">Should asset be overwritten if found in the same location?</param>
        /// /// <returns>The created asset.</returns>
        public static UnityEngine.Object CreateAsset(UnityEngine.Object asset, string path, string fileName, bool overwrite = true)
        {
            return CreateAsset<UnityEngine.Object>(asset, path, fileName, overwrite);
        }

        /// <summary>Ensures the path exists, then creates the asset.</summary>
        /// <param name="asset">Object to use in creating the asset.</param>
        /// <param name="path">Path for the new asset, relative to the Assets folder.</param>
        /// <param name="fileName">File name, including extension.</param>
        /// <param name="overwrite">Should asset be overwritten if found in the same location?</param>
        /// <returns>The created asset of type T.</returns>
        public static T CreateAsset<T>(T asset, string path, string fileName, bool overwrite = true) where T : UnityEngine.Object
        {
            if (!fileName.Contains("."))
                throw new System.Exception("Parameter fileName must contain a file extension.");

            // Remove Assets prefix
            path = CleanAssetsPath(path);

            // Ensure the path is valid
            EnsurePathExists(path);

            // Get the correct asset path
            string assetPath = (path.EndsWith("/")) ? string.Concat(path, fileName) : string.Concat(path, "/", fileName);

            if (overwrite || !AssetsFileExists(path, fileName))
                AssetDatabase.CreateAsset(asset, string.Concat("Assets/", assetPath));

            return GetAsset<T>(string.Concat("Assets/", assetPath));
        }

        /// <returns>Asset at path, if it exists.</returns>
        public static UnityEngine.Object GetAsset(string path)
        {
            return GetAsset<UnityEngine.Object>(path);
        }

        /// <returns>Asset of type T at path, if it exists.</returns>
        public static T GetAsset<T>(string path) where T : UnityEngine.Object
        {
            path = CleanAssetsPath(path);
            return AssetDatabase.LoadAssetAtPath<T>(string.Concat("Assets/", path));
        }

        /// <param name="path">Filesystem path (relative to Assets folder) containing the file.</param>
        /// <param name="fileName">The file name to query.</param>
        /// <returns>True if a file with name fileName exists in the folder path.</returns>
        public static bool AssetsFileExists(string path, string fileName)
        {
            // Remove Assets prefix
            path = CleanAssetsPath(path);

            return FileExists(string.Concat(Application.dataPath, path), fileName);
        }

        /// <param name="path">Filesystem path containing the file.</param>
        /// <param name="fileName">The file name to query.</param>
        /// <returns>True if a file with name fileName exists in the folder path.</returns>
        public static bool FileExists(string path, string fileName)
        {
            System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(path);
            System.IO.FileInfo[] files;

            if (fileName.Contains(".")) // If fileName has extension
                files = root.GetFiles(fileName);
            else
                files = root.GetFiles(string.Concat(fileName, ".*"));

            return files.Length > 0;
        }
    }
}
#endif

namespace JakePerry
{
    /// <summary>A serializable Dictionary. 
    /// You must derive from this class and tag as System.Serializable for this to serialize properly.</summary>
    [System.Serializable]
    public class Dict<TKey, TValue> : ISerializationCallbackReceiver
    {
        private Dictionary<TKey, TValue> dict;
        [SerializeField]
        private TKey[] keys;
        [SerializeField]
        private TValue[] values;

        public Dictionary<TKey, TValue> Dictionary { get { return dict; } }

        #region Constructors

        public Dict()
        { dict = new Dictionary<TKey, TValue>(); }
        public Dict(IDictionary<TKey, TValue> dictionary)
        { dict = new Dictionary<TKey, TValue>(dictionary); }
        public Dict(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        { dict = new Dictionary<TKey, TValue>(dictionary, comparer); }
        public Dict(IEqualityComparer<TKey> comparer)
        { dict = new Dictionary<TKey, TValue>(comparer); }
        public Dict(Int32 capacity)
        { dict = new Dictionary<TKey, TValue>(capacity); }
        public Dict(Int32 capacity, IEqualityComparer<TKey> comparer)
        { dict = new Dictionary<TKey, TValue>(capacity, comparer); }

        #endregion

        #region Interface Implementation

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dict.Clear();
            if (keys.Length != values.Length)
                throw new System.Exception(string.Format(
                    "Key & Value quantity mismatch. Ensure both Key type {0} and Value type {1} are serializable.",
                    typeof(TKey).Name, typeof(TValue).Name));

            for (int i = 0; i < keys.Length; i++)
                dict.Add(keys[i], values[i]);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keys = dict.Keys.ToArray();
            values = dict.Values.ToArray();
        }

        #endregion
    }
}
