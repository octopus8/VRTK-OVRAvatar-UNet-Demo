using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUNet : MonoBehaviour
{
    [Tooltip("Player prefab used by UNet when players connect.")]
    [SerializeField]
    GameObject playerPrefab;


    public void Start()
    {
        // Set the player prefab for UNet.
        VRBNetworkManager.Instance.SetPlayerPrefab(playerPrefab);
    }





    public void StartHostingInternetMatch()
    {
        VRBNetworkManager.Instance.StartHostingInternetMatch("test internet match");
    }






    public void StartHostingLANMatch()
    {
        VRBNetworkManager.Instance.StartHostingLANMatch();
    }




}
