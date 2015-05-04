using UnityEngine;
using System.Collections;

namespace SpaceShooter {
    public class Done_DestroyByBoundary : MonoBehaviour
    {
    	void OnTriggerExit (Collider other) 
    	{
    		Destroy(other.gameObject);
    	}
    }    
}
