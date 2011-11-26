using UnityEngine;
using System.Collections;

public class UK_EditorStrings {
    // Icons
	public const string FoldedIcon           = "UK_FoldedIcon.psd";
	public const string UnfoldedIcon         = "UK_UnfoldedIcon.psd";
	public const string MinimizeIcon         = "UK_MinimizeIcon.psd";
	public const string MaximizeIcon         = "UK_MaximizeIcon.psd";
	public const string JoystickIcon         = "UK_JotstickIcon.psd";
	public const string ClockIcon            = "UK_ClockIcon.psd";
	public const string UpArrowHeadIcon      = "UK_UpArrowHeadIcon.psd";
	public const string DownArrowHeadIcon    = "UK_DownArrowHeadIcon.psd";
	public const string LeftArrowHeadIcon    = "UK_LeftArrowHeadIcon.psd";
	public const string RightArrowHeadIcon   = "UK_RightArrowHeadIcon.psd";
	public const string ModuleIcon           = "UK_ModuleIcon.png";
	public const string MethodIcon           = "UK_FunctionIcon.png";
	public const string TransitionTriggerIcon= "UK_TransitionTriggerIcon.png";
	public const string RandomIcon           = "UK_RandomIcon.png";
	
    // Textures
    public const string AALineTexture     = "UK_LineTexture.psd";
    public const string DefaultNodeTexture= "UK_DefaultNodeTexture.psd";
    public const string NodeMaskTexture   = "UK_NodeMaskTexture.psd";

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
