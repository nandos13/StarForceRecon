using UnityEngine;
using UnityEditor;
using JakePerry;

public class DamageLayerFileCreator
{
    const string FOLDERPATH = "Assets/Resources";
    const string RESOURCESPATH = "DamageSystem/LayerDefinition";

    [InitializeOnLoadMethod]
    private static void InitializeLayerFile()
    {
        DamageLayerUtils.Definition file = Resources.Load<DamageLayerUtils.Definition>(RESOURCESPATH);
        if (file == null)
        {
            Debug.Log("No Damage-Layer definition file was found. Creating one now.");
            file = CreateFile();
        }
    }

    private static DamageLayerUtils.Definition CreateFile()
    {
        if (!AssetDatabase.IsValidFolder(FOLDERPATH))
            AssetDatabase.CreateFolder("Assets", "Resources");

        if (!AssetDatabase.IsValidFolder(string.Concat(FOLDERPATH, "/DamageSystem")))
            AssetDatabase.CreateFolder(FOLDERPATH, "DamageSystem");
        
        AssetDatabase.CreateAsset(
            ScriptableObject.CreateInstance<DamageLayerUtils.Definition>(),
            string.Concat(FOLDERPATH, "/", RESOURCESPATH, ".asset") );

        return Resources.Load<DamageLayerUtils.Definition>(RESOURCESPATH);
    }
}
