using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class iCS_TextureCache {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    static Dictionary<string, Texture2D>    OurCachedTextures  = new Dictionary<string, Texture2D>();
    static Dictionary<string, bool>         OurErrorAlreadySeen= new Dictionary<string, bool>();

    // ======================================================================
    // Release Used textures
    // ----------------------------------------------------------------------
    public static void FlushAndClear() {
        // Remove reference to cache textures.
        foreach(var pair in OurCachedTextures) {
            Resources.UnloadAsset(pair.Value);
        }
        OurCachedTextures  = new Dictionary<string, Texture2D>();
        OurErrorAlreadySeen= new Dictionary<string, bool>();
        Resources.UnloadUnusedAssets();
    }
    [MenuItem("Edit/iCanScript/Release Editor Textures",false,1000)]
    public static void ReleaseEditorTextures() {
        iCS_TextureCache.FlushAndClear();
    }
    
    // ======================================================================
    // Texture access
    // ----------------------------------------------------------------------
    public static Texture2D GetTextureFromGUID(string guid) {
        return guid != null ? GetTexture(AssetDatabase.GUIDToAssetPath(guid)) : null;
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetTexture(string fileName) {
        Texture2D texture= null;
        if(OurCachedTextures.ContainsKey(fileName)) {
            OurCachedTextures.TryGetValue(fileName, out texture);
            if(texture != null) return texture;
            OurCachedTextures.Remove(fileName);
        }
        texture= Resources.LoadAssetAtPath(fileName, typeof(Texture2D)) as Texture2D;
        if(texture != null) {
//            var clone= iCS_TextureUtil.Clone(texture);
//            OurCachedTextures.Add(fileName, clone);
//            clone.hideFlags= HideFlags.DontSave;
//            Resources.UnloadAsset(texture);
//            return clone;
            OurCachedTextures.Add(fileName, texture);
            texture.hideFlags= HideFlags.DontSave;
        }
        return texture;
    }
    // ----------------------------------------------------------------------
    public static bool GetTexture(string fileName, out Texture2D texture) {
        string texturePath= iCS_Config.GuiAssetPath+"/"+fileName;
        texture= GetTexture(texturePath);
        if(texture == null) {
            ResourceMissingError(texturePath);
            return false;
        }        
        return true;            
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetIconFromGUID(string guid) {
        return GetTextureFromGUID(guid);
    }
    // ----------------------------------------------------------------------
    public static Texture2D GetIcon(string fileName) {
        // Try with the WarpDrice Icon prefix.
        string iconPath= iCS_Config.ResourcePath+"/"+fileName;
        Texture2D icon= GetTexture(iconPath);
        if(icon == null) {
            // Try without any prefix.
            icon= GetTexture(fileName);                            
        }
        return icon;
    }
    // ----------------------------------------------------------------------
    public static string IconPathToGUID(string fileName) {
        if(fileName == null) return null;
        Texture2D icon= GetIcon(fileName);
        if(icon == null) return null;
        string path= AssetDatabase.GetAssetPath(icon);
        return AssetDatabase.AssetPathToGUID(path);
    }
    // ----------------------------------------------------------------------
    public static bool GetIcon(string fileName, out Texture2D icon) {
        icon= GetIcon(fileName);
        if(icon == null) {
            ResourceMissingError(fileName);            
            return false;
        }
        return true;
    }

    // ======================================================================
    //  ERROR MANAGEMENT
    // ----------------------------------------------------------------------
	public static void ResourceMissingError(string _name) {
        bool alreadySeen= OurErrorAlreadySeen.ContainsKey(_name);
        string errorMsg= "iCanScript: Unable to locate editor resource: " + _name;
        if(!alreadySeen) {
    		EditorUtility.DisplayDialog ("Resource Missing", errorMsg, "Ok");            
            OurErrorAlreadySeen.Add(_name, true);
        }
		else {
		    Debug.LogError(errorMsg);
	    }
	}
}
