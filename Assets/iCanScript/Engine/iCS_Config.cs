using UnityEngine;
using System.Collections;

public static class iCS_Config {
    // ----------------------------------------------------------------------
    public const string Version= "Version 0.9.1";
    public const string VersionLabel= "(RC #1)";
    public const string ProductName= "iCanScript";
    public const string ProductAcronym= "iCS";
	public const string EditorPath= "Assets/"+ProductName+"/Editor";
	public const string EnginePath= "Assets/"+ProductName+"/Engine";
	public const string GizmosPath= EnginePath+"/Gizmos";
	public const string ResourcePath= EditorPath + "/Resources";
	public const string GuiAssetPath= ResourcePath;
	public const string ProductPrefix= ProductAcronym+"_";
	public const char   PrivateStringPrefix= '$';
}
