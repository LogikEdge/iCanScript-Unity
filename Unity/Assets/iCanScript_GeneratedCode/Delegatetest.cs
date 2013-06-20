using UnityEngine;
using System.Collections;

public class Delegatetest : MonoBehaviour {

	void OnMouseOver() {
		var iCanScript= GetComponent(typeof(iCS_BehaviourImp)) as iCS_BehaviourImp;
		if(iCanScript != null) {
			iCanScript.SayHello();
		}
	}
}
