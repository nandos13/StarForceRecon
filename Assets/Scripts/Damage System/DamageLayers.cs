using UnityEngine;
using UnityEditor;
using System.Linq;

namespace JakePerry
{
    [System.Serializable]
    public struct DamageLayer
    {
        [System.Serializable]
        public struct Mask
        {
            [SerializeField]
            private int _mask;

            public Mask(int mask)
            {
                _mask = mask;
            }

            public static implicit operator int(Mask mask)
            {
                return mask._mask;
            }

            public static implicit operator Mask(int i)
            {
                return new Mask(i);
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

        [SerializeField]
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

        private static string[] _layerNames = new string[32];

        #endregion

        #region Functionality

        /// <summary>Initialization constructor.</summary>
        static DamageLayerUtils()
        {
            LoadLayers();
        }

        private static DamageLayerDefinition GetLayerFile()
        {
            const string PATH = "DamageSystem/LayerDefinition";
            DamageLayerDefinition file = Resources.Load<DamageLayerDefinition>(PATH);

#if UNITY_EDITOR
            if (file == null)
            {
                DamageLayerDefinition newFile =
                                (DamageLayerDefinition)ScriptableObject.CreateInstance(typeof(DamageLayerDefinition));
                AssetDatabase.CreateAsset(newFile,
                                "Assets/Resources/" + PATH + ".asset");

                file = newFile;
            }
#endif

            if (file == null)
                throw new System.Exception("DamageLayer Definition file does not exist!");

            return file;
        }

        public static string[] GetLayerNames()
        {
            return GetLayerFile().strings.Where(x => !string.IsNullOrEmpty(x)).ToArray<string>();
        }

        private static void SaveLayers()
        {
            DamageLayerDefinition namesFile = GetLayerFile();

            if (namesFile != null)
                namesFile.strings = _layerNames;
        }

        private static void LoadLayers()
        {
            DamageLayerDefinition namesFile = GetLayerFile();

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
        
        /// <param name="names">Names of layers to include in the mask.</param>
        /// <returns>Mask containing each layer specified by name.</returns>
        public static DamageLayer.Mask GetMask(params string[] names)
        {
            DamageLayer.Mask mask = new DamageLayer.Mask();

            foreach (string name in names)
            {
                int layerID = NameToLayer(name);
                mask.SetLayerState(layerID, true);
            }

            return mask;
        }

        /// <param name="layers">Layer values to include in the mask.</param>
        /// <returns>Mask containing each layer specified by name.</returns>
        public static DamageLayer.Mask GetMask(params int[] layers)
        {
            DamageLayer.Mask mask = new DamageLayer.Mask();

            foreach (int layer in layers)
            {
                mask.SetLayerState(layer, true);
            }

            return mask;
        }

        /// <param name="names">Names of layers to include in the mask.</param>
        /// <returns>Int value for mask containing each layer specified by name.</returns>
        public static int GetMaskInt(params string[] names)
        {
            return (int)GetMask(names);
        }

        /// <param name="layers">Layer values to include in the mask.</param>
        /// <returns>Mask containing each layer specified by name.</returns>
        public static int GetMaskInt(params int[] layers)
        {
            return (int)GetMask(layers);
        }

        /// <returns>Index of layer if found. If layer does not exist, returns -1.</returns>
        public static int NameToLayer(string name)
        {
            for (int i = 0; i < _layerNames.Length; i++)
            {
                if (_layerNames[i] == name)
                    return i;
            }

            return -1;
        }

        /// <summary>Returns the name of the layer with index i [0-31].</summary>
        public static string GetLayerName(int i)
        {
            if (i < 0 || i > 31) return null;
            return _layerNames[i];
        }

        /// <returns>Index of newly created layer. If layer could not be created, returns -1.</returns>
        public static int CreateNewLayer(string name)
        {
            // Find first layer with blank name
            for (int i = 0; i < _layerNames.Length; i++)
            {
                if (string.IsNullOrEmpty(_layerNames[i]))
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
