#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using JakePerry;

public class DamageLayerDefinitionCreator
{
    const string ASSETPATH = "Assets/Resources/DamageSystem/";
    const string ASSETNAME = "LayerDefinition.asset";

    [InitializeOnLoadMethod]
    private static void InitializeLayerFile()
    {
        AssetDatabase.Refresh(ImportAssetOptions.Default);

        DamageLayerDefinition file =
            AssetUtilities.GetAsset<DamageLayerDefinition>(string.Concat(ASSETPATH, ASSETNAME));

        if (file == null)
        {
            Debug.Log("No Definition file for damage layers was found. Creating one now.");
            AssetUtilities.CreateAsset<DamageLayerDefinition>(
                ScriptableObject.CreateInstance<DamageLayerDefinition>(),
                ASSETPATH, ASSETNAME, true);
        }
    }
}
#endif
