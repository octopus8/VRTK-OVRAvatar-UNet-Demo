using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTKOVRAvatarUNetDemo : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;


	void Start ()
    {
        VRBNetworkManager.Instance.SetPlayerPrefab(playerPrefab);
	}



	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            VRBNetworkManager.Instance.StartHostingLocalNetwork();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            VRBNetworkManager.Instance.JoinLocalNetwork();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            VRBNetworkManager.Instance.StartHostingInternetNetwork();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            VRBNetworkManager.Instance.StartMonitoringAvailableInternetNetworks();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            VRBNetworkManager.Instance.QuitHost();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            VRBNetworkManager.Instance.QuitClient();
        }

    }
}
