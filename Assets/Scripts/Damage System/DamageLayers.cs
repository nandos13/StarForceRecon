using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JakePerry
{
    [System.Serializable]
    public struct DamageLayer
    {
        /// <summary>A collection of utilities functions for Damage Layers.</summary>
        public static class Utils
        {
            #region Definition

            private static readonly DamageLayerDefinition definition;

            #endregion

            #region Functionality

            /// <summary>Initialization constructor.</summary>
            static Utils()
            {
                definition = GetDefinition();
            }

            /// <returns>The layer definition file.</returns>
            private static DamageLayerDefinition GetDefinition()
            {
                const string PATH = "DamageSystem/LayerDefinition";
                DamageLayerDefinition file = Resources.Load<DamageLayerDefinition>(PATH);

#if UNITY_EDITOR
                if (file == null)
                    file = GetDefinitionFile();
#endif

                if (file == null)
                    throw new System.Exception("DamageLayer Definition file does not exist!");

                return file;
            }

#if UNITY_EDITOR

            const string ASSETPATH = "Assets/Resources/DamageSystem/";
            const string ASSETNAME = "LayerDefinition.asset";
            /// <summary>Returns the definition file. If non-existent, creates the file then returns it.</summary>
            public static DamageLayerDefinition GetDefinitionFile()
            {
                AssetDatabase.Refresh(ImportAssetOptions.Default);

                DamageLayerDefinition file =
                    AssetUtilities.GetAsset<DamageLayerDefinition>(string.Concat(ASSETPATH, ASSETNAME));

                if (file == null)
                {
                    Debug.Log("No Definition file for damage layers was found. Creating one now.");
                    file = AssetUtilities.CreateAsset<DamageLayerDefinition>(
                            ScriptableObject.CreateInstance<DamageLayerDefinition>(),
                            ASSETPATH, ASSETNAME, true);
                }

                return file;
            }

#endif

            public static string[] GetLayerNames()
            {
                return definition.strings.Where(x => !string.IsNullOrEmpty(x)).ToArray<string>();
            }

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
                for (int i = 0; i < definition.strings.Length; i++)
                {
                    if (definition.strings[i] == name)
                        return i;
                }

                return -1;
            }

            /// <summary>Returns the name of the layer with index i [0-31].</summary>
            public static string GetLayerName(int i)
            {
                if (i < 0 || i > 31) return null;
                return definition.strings[i];
            }

            #endregion
        }

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
}
