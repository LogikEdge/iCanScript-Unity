using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class iCS_Behaviour : MonoBehaviour {
    iCS_VisualScript[] allVisualScripts= new iCS_VisualScript[0];
    
	// Use this for initialization
	void Start () {
        allVisualScripts= GameObject.FindObjectsOfType(typeof(iCS_VisualScript)) as iCS_VisualScript[];
//        Debug.Log("# Visual Scripts in Scene: "+visualScriptInSceneCount);
	}
	
	// Update is called once per frame
	void Update () {
	    foreach(var vs in allVisualScripts) {
	        vs.RunMessage("Update");
	    }
	}
}
