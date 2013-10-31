//{"ContentHash" : "26570609E959332BB1F1674BE6A9641F"}
/////////////////////////////////////////////////////////////////
//  iCS_Behaviour.cs
//
//  Generated by iCanScript v0.9.5
/////////////////////////////////////////////////////////////////

using UnityEngine;

[AddComponentMenu("")]
public sealed class iCS_Behaviour : MonoBehaviour {
	iCS_VisualScript[]   allVisualScripts= null;

	void Start()
	{
		allVisualScripts= gameObject.GetComponents<iCS_VisualScript>();
	}

	void Update()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("Update");
			}
		}
	}


	void LateUpdate()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("LateUpdate");
			}
		}
	}


	void FixedUpdate()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("FixedUpdate");
			}
		}
	}



	void Reset()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("Reset");
			}
		}
	}


	void OnMouseEnter()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseEnter");
			}
		}
	}


	void OnMouseOver()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseOver");
			}
		}
	}


	void OnMouseExit()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseExit");
			}
		}
	}


	void OnMouseDown()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseDown");
			}
		}
	}


	void OnMouseUp()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseUp");
			}
		}
	}


	void OnMouseUpAsButton()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseUpAsButton");
			}
		}
	}


	void OnMouseDrag()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMouseDrag");
			}
		}
	}


	void OnBecameVisible()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnBecameVisible");
			}
		}
	}


	void OnBecameInvisible()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnBecameInvisible");
			}
		}
	}


	void OnEnable()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnEnable");
			}
		}
	}


	void OnDisable()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnDisable");
			}
		}
	}


	void OnDestroy()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnDestroy");
			}
		}
	}


	void OnPreCull()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnPreCull");
			}
		}
	}


	void OnPreRender()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnPreRender");
			}
		}
	}


	void OnPostRender()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnPostRender");
			}
		}
	}


	void OnRenderObject()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnRenderObject");
			}
		}
	}


	void OnWillRenderObject()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnWillRenderObject");
			}
		}
	}


	void OnGUI()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnGUI");
			}
		}
	}


	void OnDrawGizmosSelected()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnDrawGizmosSelected");
			}
		}
	}


	void OnDrawGizmos()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnDrawGizmos");
			}
		}
	}


	void OnServerInitialized()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnServerInitialized");
			}
		}
	}


	void OnConnectedToServer()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnConnectedToServer");
			}
		}
	}


	void OnAnimatorMove()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnAnimatorMove");
			}
		}
	}


	void OnApplicationQuit()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnApplicationQuit");
			}
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnTriggerEnter", other);
			}
		}
	}


	void OnTriggerExit(Collider other)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnTriggerExit", other);
			}
		}
	}


	void OnTriggerStay(Collider other)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnTriggerStay", other);
			}
		}
	}


	void OnCollisionEnter(Collision collisionInfo)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnCollisionEnter", collisionInfo);
			}
		}
	}


	void OnCollisionExit(Collision collisionInfo)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnCollisionExit", collisionInfo);
			}
		}
	}


	void OnCollisionStay(Collision collisionInfo)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnCollisionStay", collisionInfo);
			}
		}
	}


	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnControllerColliderHit", hit);
			}
		}
	}


	void OnJointBreak(float breakForce)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnJointBreak", breakForce);
			}
		}
	}


	void ParticleCollision(GameObject gameObject)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("ParticleCollision", gameObject);
			}
		}
	}


	void OnLevelWasLoaded(int level)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnLevelWasLoaded", level);
			}
		}
	}


	void OnApplicationPause(bool pause)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnApplicationPause", pause);
			}
		}
	}


	void OnApplicationFocus(bool focus)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnApplicationFocus", focus);
			}
		}
	}


	void OnPlayerConnected(NetworkPlayer player)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnPlayerConnected", player);
			}
		}
	}


	void OnPlayerDisconnected(NetworkPlayer player)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnPlayerDisconnected", player);
			}
		}
	}


	void OnDisconnectedFromServer(NetworkDisconnection mode)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnDisconnectedFromServer", mode);
			}
		}
	}


	void OnFailedToConnect(NetworkConnectionError error)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnFailedToConnect", error);
			}
		}
	}


	void OnFailedToConnectToMasterServer(NetworkConnectionError error)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnFailedToConnectToMasterServer", error);
			}
		}
	}


	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnMasterServerEvent", msEvent);
			}
		}
	}


	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnNetworkInstantiate", info);
			}
		}
	}


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnSerializeNetworkView", stream, info);
			}
		}
	}


	void OnAnimatorIK(int layerIndex)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnAnimatorIK", layerIndex);
			}
		}
	}


	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnRenderImage", source, destination);
			}
		}
	}


	void OnAudioFilterRead(float[] data, int channels)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnAudioFilterRead", data, channels);
			}
		}
	}


}
