//{"ContentHash" : "EB01A1A897E939F2F94F2EEEA87475D0"}
/////////////////////////////////////////////////////////////////
//  iCS_Behaviour.cs
//
//  Generated by iCanScript v1.2.3
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


	void OnApplicationQuit()
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnApplicationQuit");
			}
		}
	}


	void OnTriggerEnter(Collider colliderInfo)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnTriggerEnter", colliderInfo);
			}
		}
	}


	void OnTriggerExit(Collider colliderInfo)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnTriggerExit", colliderInfo);
			}
		}
	}


	void OnTriggerStay(Collider colliderInfo)
	{
		if(allVisualScripts != null) {
			foreach(var vs in allVisualScripts) {
				vs.RunMessage("OnTriggerStay", colliderInfo);
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
