using UnityEngine;
using System.Linq;
using System.Collections.Generic;
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

                if (definition == null)
                    throw new System.Exception(
                        "DamageLayer Definition file does not exist! All subsequent calls to DamageLayer methods will fail.");
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
            
            /// <returns>An array of all layer names defined in the definition file.</returns>
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

        /// <summary>Describes damage modifiers for each layer.</summary>
        [System.Serializable]
        public struct Modifier
        {
            public Dictionary<DamageLayer, float> modifiers;

#if UNITY_EDITOR
            /// <summary>EDITOR USE ONLY. A list of keys used in the modifiers dictionary, for serialization.</summary>
            [SerializeField]
            private DamageLayer[] modifierKeys;
            [SerializeField]
            private float[] modifierValues;
#endif

            #region Constructors

            public Modifier(params KeyValuePair<int, float>[] modifiers)
            {
                this.modifiers = new Dictionary<DamageLayer, float>();
#if UNITY_EDITOR
                modifierKeys = new DamageLayer[1];
                modifierValues = new float[1];
#endif

                foreach (var mod in modifiers)
                {
                    SetModifier(mod);
                }
            }

            public Modifier(params KeyValuePair<DamageLayer, float>[] modifiers)
            {
                this.modifiers = new Dictionary<DamageLayer, float>();
#if UNITY_EDITOR
                modifierKeys = new DamageLayer[1];
                modifierValues = new float[1];
#endif

                foreach (var mod in modifiers)
                {
                    SetModifier(mod);
                }
            }

            #endregion

            #region public void SetModifier variants

            /// <summary>Set the modifier value for a layer.</summary>
            /// <param name="modifier">TKey: int. Layer value [0-31].
            /// <para>TValue: float. Modifier value for the layer.
            /// </para></param>
            public void SetModifier(KeyValuePair<int, float> modifier)
            {
                SetModifier(modifier.Key, modifier.Value);
            }

            /// <summary>Set the modifier value for a layer.</summary>
            public void SetModifier(KeyValuePair<DamageLayer, float> modifier)
            {
                SetModifier(modifier.Key, modifier.Value);
            }

            /// <summary>Set the modifier value for a layer.</summary>
            /// <param name="layer">Layer value [0-31].</param>
            public void SetModifier(int layer, float modifier)
            {
                SetModifier(new DamageLayer(layer), modifier);
            }
            
            /// <summary>Set the modifier value for a layer.</summary>
            public void SetModifier(DamageLayer layer, float modifier)
            {
                if (modifiers == null) modifiers = new Dictionary<DamageLayer, float>();
#if UNITY_EDITOR
                if (modifierKeys == null) modifierKeys = new DamageLayer[1];
                if (modifierValues == null) modifierValues = new float[1];

                if (!modifierKeys.Contains(layer))
                {
                    List<DamageLayer> keyList = modifierKeys.ToList();
                    keyList.Add(layer);
                    modifierKeys = keyList.ToArray();

                    List<float> valueList = modifierValues.ToList();
                    valueList.Add(modifier);
                    modifierValues = valueList.ToArray();
                }
#endif
                modifiers[layer] = modifier;
            }

            #endregion

            /// <returns>The modifier associated with the layer.</returns>
            public float GetModifier(DamageLayer layer)
            {
                if (modifiers == null) modifiers = new Dictionary<DamageLayer, float>();

                if (!modifiers.ContainsKey(layer))
                    return 1.0f;

                return modifiers[layer];
            }
        }

        [SerializeField]
        private int _layer;
        public int value { get { return _layer; } }

        #region Implicit operators & equality

        public static implicit operator int(DamageLayer layer)
        {
            return layer.value;
        }

        public static implicit operator DamageLayer(int value)
        {
            return new DamageLayer(value);
        }

        public static bool operator ==(DamageLayer lhs, DamageLayer rhs)
        {
            return lhs._layer == rhs._layer;
        }

        public static bool operator !=(DamageLayer lhs, DamageLayer rhs)
        {
            return lhs._layer != rhs._layer;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DamageLayer)) return false;

            DamageLayer other = (DamageLayer)obj;
            return this._layer == other._layer;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + this._layer.GetHashCode();
            return hash;
        }

        #endregion

        public DamageLayer(int layerIndex)
        {
            if (layerIndex <= 31 && layerIndex >= 0)
                _layer = layerIndex;
            else
                _layer = 0;
        }
    }
}
