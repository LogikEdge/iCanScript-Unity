using UnityEngine;
using System.Collections;

public class DestroyBoundary : MonoBehaviour {

	void OnTriggerExit(Collider other) 
	{
		Destroy(other.gameObject);
	}
}
