using UnityEngine;
using System.Collections;

public class WD_EditorStrings {
    // Icons
	public const string FoldedIcon           = "WD_FoldedIcon.psd";
	public const string UnfoldedIcon         = "WD_UnfoldedIcon.psd";
	public const string MinimizeIcon         = "WD_MinimizeIcon.psd";
	public const string MaximizeIcon         = "WD_MaximizeIcon.psd";
	public const string JoystickIcon         = "WD_JotstickIcon.psd";
	public const string ClockIcon            = "WD_ClockIcon.psd";
	public const string UpArrowHeadIcon      = "WD_UpArrowHeadIcon.psd";
	public const string DownArrowHeadIcon    = "WD_DownArrowHeadIcon.psd";
	public const string LeftArrowHeadIcon    = "WD_LeftArrowHeadIcon.psd";
	public const string RightArrowHeadIcon   = "WD_RightArrowHeadIcon.psd";
	public const string ModuleIcon           = "WD_ModuleIcon.png";
	public const string FunctionIcon         = "WD_FunctionIcon.png";
	public const string TransitionTriggerIcon= "WD_TransitionTriggerIcon.png";
	public const string HolderIcon           = "WD_ModuleIcon.png";
	
    // Textures
    public const string AALineTexture     = "WD_LineTexture.psd";
    public const string DefaultNodeTexture= "WD_DefaultNodeTexture.psd";
    public const string NodeMaskTexture   = "WD_NodeMaskTexture.psd";

    // Special node names.
    public const string UpdateNode     = "Update";
    public const string LateUpdateNode = "LateUpdate";
    public const string FixedUpdateNode= "FixedUpdate";
    public const string OnEntryNode    = "OnEntry";
    public const string OnExitNode     = "OnExit";
    public const string OnUpdateNode   = "OnUpdate";

    // Special port names.
    public const string EnablePort= "enable";

    // Reflection methods
    public const string AddChildMethod   = "AddChild";
    public const string RemoveChildMethod= "RemoveChild";

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
