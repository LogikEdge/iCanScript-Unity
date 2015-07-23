using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Engine {
	
	public static class iCS_Config {
	    // ----------------------------------------------------------------------
	    // Product descriptions. 
		public const int    MajorVersion = 2;
		public const int    MinorVersion = 0;
		public const int    BugFixVersion= 26;
	    public const string ProductName             = "iCanScript";
	    public const string ProductAcronym          = "iCS";
		public const string ProductPrefix           = ProductAcronym+"_";
		public const string kCodeGenerationNamespace= "iCanScript.Engine.GeneratedCode";

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
	
}
