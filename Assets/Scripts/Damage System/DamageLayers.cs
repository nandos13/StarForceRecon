using UnityEngine;
using UnityEditor;

namespace JakePerry
{
    [System.Serializable]
    public struct DamageLayerMask
    {
        [SerializeField]
        private int _mask;

        public DamageLayerMask(int mask)
        {
            _mask = mask;
        }

        public static implicit operator int(DamageLayerMask mask)
        {
            return mask._mask;
        }

        public static implicit operator DamageLayerMask(int i)
        {
            return new DamageLayerMask(i);
        }

        /// <returns>True if layer at index [0-31] is contained by the mask.</returns>
        public bool ContainsLayer(int index)
        {
            if (index < 0 || index > 31) return false;

            return _mask == (_mask | (1 << index));
        }

        /// <param name="index">Layer index to set [0-31].</param>
        /// <param name="state">New inclusion state.</param>
        public void SetLayerState(int index, bool state)
        {
            if (index < 0 || index > 31) return;

            // Clear bit
            _mask &= ~(1 << index);

            // Set bit if state is true
            if (state)
                _mask |= 1 << index;
        }
    }

    [System.Serializable]
    public struct DamageLayer
    {
        private int _layer;
        public int value { get { return _layer; } }

        public DamageLayer(int layerIndex)
        {
            if (layerIndex <= 31 && layerIndex >= 0)
                _layer = layerIndex;
            else
                _layer = 0;
        }
    }

    public static class DamageLayerUtils
    {
        #region Layers

        public class Definition : ScriptableObject
        {
            [SerializeField]    private string[] _strings = new string[32];

            public string[] strings
            {
                get { return _strings; }
                set { _strings = value; }
            }
        }

        private static string[] _layerNames = new string[32];

        #endregion

        #region Functionality

        /// <summary>Initialization constructor.</summary>
        static DamageLayerUtils()
        {
            LoadLayers();
        }

        private static Definition GetLayerFile()
        {
            const string PATH = "DamageSystem/LayerDefinition";
            Definition file = Resources.Load<Definition>(PATH);

            if (file == null)
            {
                Definition newFile =
                                (Definition)ScriptableObject.CreateInstance(typeof(Definition));
                AssetDatabase.CreateAsset(newFile,
                                "Assets/Resources/" + PATH + ".asset");

                file = newFile;
            }

            return file;
        }

        private static void SaveLayers()
        {
            Definition namesFile = GetLayerFile();

            if (namesFile != null)
                namesFile.strings = _layerNames;
        }

        private static void LoadLayers()
        {
            Definition namesFile = GetLayerFile();

            if (namesFile != null)
                namesFile.strings.CopyTo(_layerNames, 0);
        }

        // SetLayerName function only available for editor application.
#if UNITY_EDITOR
        /// <summary>Names the damage layer.</summary>
        /// <param name="name">New layer name.</param>
        /// <param name="index">Layer index to set [0-31].</param>
        public static void SetLayerName(string name, int index)
        {
            if (index < 0 || index > 31) return;

            _layerNames[index] = name;
            SaveLayers();
        }
#endif

        /// <returns>Index of layer if found. If layer does not exist, returns -1.</returns>
        public static int NameToLayer(string layer)
        {
            for (int i = 0; i < _layerNames.Length; i++)
            {
                if (_layerNames[i] == layer)
                    return i;
            }

            return -1;
        }

        /// <summary>Returns the name of the layer with index i [0-31].</summary>
        public static string GetLayerName(int i)
        {
            if (i < 0 || i > 31) return "Nothing";
            return _layerNames[i];
        }

        /// <returns>Index of newly created layer. If layer could not be created, returns -1.</returns>
        public static int CreateNewLayer(string name)
        {
            // Find first layer with blank name
            for (int i = 0; i < _layerNames.Length; i++)
            {
                if (_layerNames[i] == null)
                {
                    _layerNames[i] = name;
                    SaveLayers();
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
