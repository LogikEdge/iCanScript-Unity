using UnityEngine;
using System.Collections;

public static class iCS_Config {
    // ----------------------------------------------------------------------
    // Product descriptions. 
	public const int    MajorVersion = 1;
	public const int    MinorVersion = 2;
	public const int    BugFixVersion= 7;
    public const string ProductName= "iCanScript";
    public const string ProductAcronym= "iCS";
	public const string ProductPrefix= ProductAcronym+"_";

    // ----------------------------------------------------------------------
    // Product paths.
    public const string AssetsPath               = "Assets";
	public const string EditorPath               = AssetsPath+"/"+ProductName+"/Editor";
	public const string EnginePath               = AssetsPath+"/"+ProductName+"/Engine";
    public const string GizmosFolder             = "Gizmos";
    public const string GizmosPath               = AssetsPath+"/"+GizmosFolder;
	public const string ImagePath                = EditorPath+"/Images";
	public const string CodeGenerationFolder     = ProductName+"_GeneratedCode";
    public const string CodeGenerationPath       = AssetsPath+"/"+CodeGenerationFolder;
    public const string BehaviourGenerationFolder= "Behaviours";
    public const string BehaviourGenerationPath  = CodeGenerationPath+"/"+BehaviourGenerationFolder;
}
