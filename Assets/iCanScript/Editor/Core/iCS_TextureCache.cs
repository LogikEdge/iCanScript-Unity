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
        texture= AssetDatabase.LoadAssetAtPath(fileName, typeof(Texture2D)) as Texture2D;
        if(texture != null) {
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
    public static Texture2D GetIcon(string fileName, iCS_IStorage storage) {
        // Try with the WarpDrice Icon prefix.
        string iconPath= iCS_UserPreferences.UserIcons.uCodeIconPath+"/"+fileName;
        Texture2D icon= GetTexture(iconPath);
        if(icon == null) {
            // Try with the user definable Icon prefixes.
            foreach(var path in storage.Preferences.Icons.CustomIconPaths) {
                icon= GetTexture(path+"/"+fileName);
                if(icon != null) break;
            }
            // Try without any prefix.
            if(icon == null) {
                icon= GetTexture(fileName);                            
            }
        }
        return icon;
    }
    // ----------------------------------------------------------------------
    public static string IconPathToGUID(string fileName, iCS_IStorage storage) {
        if(fileName == null) return null;
        Texture2D icon= GetIcon(fileName, storage);
        if(icon == null) return null;
        string path= AssetDatabase.GetAssetPath(icon);
        return AssetDatabase.AssetPathToGUID(path);
    }
    // ----------------------------------------------------------------------
    public static bool GetIcon(string fileName, out Texture2D icon, iCS_IStorage storage) {
        icon= GetIcon(fileName, storage);
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
        string errorMsg= "Unable to locate editor resource: " + _name;
        if(!alreadySeen) {
    		EditorUtility.DisplayDialog ("State Chart Abort Message", errorMsg, "Ok");            
            OurErrorAlreadySeen.Add(_name, true);
        }
		else {
		    Debug.LogError(errorMsg);
	    }
	}
}
