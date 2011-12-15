using UnityEngine;
using System.Collections;

public class iCS_EditorStrings {
    // Icons
	public const string FoldedIcon           = "iCS_FoldedIcon.psd";
	public const string UnfoldedIcon         = "iCS_UnfoldedIcon.psd";
	public const string MinimizeIcon         = "iCS_MinimizeIcon.psd";
	public const string MaximizeIcon         = "iCS_MaximizeIcon.psd";
	public const string JoystickIcon         = "iCS_JotstickIcon.psd";
	public const string ClockIcon            = "iCS_ClockIcon.psd";
	public const string UpArrowHeadIcon      = "iCS_UpArrowHeadIcon.psd";
	public const string DownArrowHeadIcon    = "iCS_DownArrowHeadIcon.psd";
	public const string LeftArrowHeadIcon    = "iCS_LeftArrowHeadIcon.psd";
	public const string RightArrowHeadIcon   = "iCS_RightArrowHeadIcon.psd";
	public const string ModuleIcon           = "iCS_ModuleIcon.png";
	public const string MethodIcon           = "iCS_FunctionIcon.png";
    public const string TransitionModuleIcon = "iCS_MaximizeIcon.psd";
	public const string TransitionTriggerIcon= "iCS_TransitionTriggerIcon.png";
	public const string RandomIcon           = "iCS_RandomIcon.png";
	
    // Textures
    public const string AALineTexture     = "iCS_LineTexture.psd";
    public const string DefaultNodeTexture= "iCS_DefaultNodeTexture.psd";
    public const string NodeMaskTexture   = "iCS_NodeMaskTexture.psd";

    // Special node names.
    public const string UpdateNode     = "Update";
    public const string LateUpdateNode = "LateUpdate";
    public const string FixedUpdateNode= "FixedUpdate";
    public const string OnEntryNode    = "OnEntry";
    public const string OnExitNode     = "OnExit";
    public const string OnUpdateNode   = "OnUpdate";

    // Special port names.
    public const string EnablePort= "enable";

    // Parsing identifiers (unused for now)
    public const string BeginBlock = "{";
    public const string EndBlock   = "}";
    public const string Seperator  = ":";
    public const string Assignment = ":=";
    public const string BeginParam = "<";
    public const string EndParam   = ">";

    // Product identifiers
    public const string Company       = "Infaunier";
    public const string DefaultPackage= "DefaultPackage";
}
