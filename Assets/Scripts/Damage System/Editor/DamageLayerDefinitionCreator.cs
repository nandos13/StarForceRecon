using UnityEngine;
using UnityEditor;
using JakePerry;

public class DamageLayerDefinitionCreator
{
    const string FOLDERPATH = "Assets/Resources";
    const string RESOURCESPATH = "DamageSystem/LayerDefinition";

    [InitializeOnLoadMethod]
    private static void InitializeLayerFile()
    {
        AssetDatabase.Refresh(ImportAssetOptions.Default);

        DamageLayerDefinition file =  
            AssetDatabase.LoadAssetAtPath<DamageLayerDefinition>(string.Concat(FOLDERPATH, "/", RESOURCESPATH, ".asset"));
        
        if (file == null)
        {
            Debug.Log("No Damage-Layer definition file was found. Creating one now.");
            file = CreateFile();
        }
    }

    private static DamageLayerDefinition CreateFile()
    {
        if (!AssetDatabase.IsValidFolder(FOLDERPATH))
            AssetDatabase.CreateFolder("Assets", "Resources");

        if (!AssetDatabase.IsValidFolder(string.Concat(FOLDERPATH, "/DamageSystem")))
            AssetDatabase.CreateFolder(FOLDERPATH, "DamageSystem");
        
        AssetDatabase.CreateAsset(
            ScriptableObject.CreateInstance<DamageLayerDefinition>(),
            string.Concat(FOLDERPATH, "/", RESOURCESPATH, ".asset") );

        return AssetDatabase.LoadAssetAtPath<DamageLayerDefinition>(string.Concat(FOLDERPATH, "/", RESOURCESPATH, ".asset"));
    }
}
