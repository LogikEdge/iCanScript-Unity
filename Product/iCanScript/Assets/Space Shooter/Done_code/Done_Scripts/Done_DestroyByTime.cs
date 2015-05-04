using UnityEngine;
using System.Collections;

namespace SpaceShooter {
    public class Done_DestroyByTime : MonoBehaviour
    {
    	public float lifetime;

    	void Start ()
    	{
    		Destroy (gameObject, lifetime);
    	}
    }
    
}
