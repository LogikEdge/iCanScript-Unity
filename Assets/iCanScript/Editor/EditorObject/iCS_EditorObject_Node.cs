using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // Node specific attributes ---------------------------------------------
    public string MethodName {
		get { return EngineObject.MethodName; }
		set { EngineObject.MethodName= value; }
	}
    public int NbOfParams {
		get { return EngineObject.NbOfParams; }
		set { EngineObject.NbOfParams= value; }
	}
    public string IconGUID {
		get { return EngineObject.IconGUID; }
		set { EngineObject.IconGUID= value; }
	}
}
