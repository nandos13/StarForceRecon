using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct DamageLayerMask
{
    private int _mask;

    public DamageLayerMask(int mask)
    {
        _mask = mask;
    }
}

public static class DamageLayerManager
{
    #region Layers

    private class DamageLayerNameFile : ScriptableObject
    {
        private string[] _strings = new string[32];
        public string[] strings
        {
            get { return _strings; }
            set { _strings = value; }
        }
    }

    private static string[] _layerNames = new string[32];

    #endregion

    #region Functionality

    private static DamageLayerNameFile GetLayerFile()
    {
        const string PATH = "DamageLayers/LayerNames";
        DamageLayerNameFile file = Resources.Load<DamageLayerNameFile>(PATH);

        if (file == null)
        {
            DamageLayerNameFile newFile = 
                            (DamageLayerNameFile)ScriptableObject.CreateInstance(typeof(DamageLayerNameFile));
            AssetDatabase.CreateAsset(newFile, 
                            "Assets/Resources/" + PATH + ".asset");

            file = newFile;
        }

        return file;
    }

    private static void SaveLayers()
    {
        DamageLayerNameFile namesFile = GetLayerFile();

        if (namesFile != null)
            namesFile.strings = _layerNames;
    }

    private static void LoadLayers()
    {
        DamageLayerNameFile namesFile = GetLayerFile();

        if (namesFile != null)
            namesFile.strings.CopyTo(_layerNames, 0);
    }

    /// <summary>Names the damage layer.</summary>
    /// <param name="name">New layer name.</param>
    /// <param name="index">Layer index to set [0-31].</param>
    public static void SetLayerName(string name, int index)
    {
        if (index < 0 || index > 31) return;

        _layerNames[index] = name;
        SaveLayers();
    }
    
    /// <returns>Index of layer if found. -1 if layer does not exist</returns>
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
        if (i < 0 || i > 31) return "";
        return _layerNames[i];
    }

    #endregion
}
