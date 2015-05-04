using UnityEngine;
using System.Collections;

namespace SpaceShooter {
    public class Done_Mover : MonoBehaviour
    {
    	public float speed;

    	void Start ()
    	{
    		GetComponent<Rigidbody>().velocity = transform.forward * speed;
    	}
    }
    
}
