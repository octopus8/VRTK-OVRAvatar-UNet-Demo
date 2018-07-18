using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// The VRBase implementation of the NetworkManager.
/// </summary>
public class VRBNetworkManagerImpl : NetworkManager
{


    /// <summary>
    /// The VRBNetworkManager that creates this script.
    /// </summary>
    VRBNetworkManager networkManager;





    /// <summary>
    /// Called on script awake.
    /// 
    /// Caches a reference to the VRBNetworkManager object.
    /// </summary>
    private void Awake()
    {
        networkManager = GetComponent<VRBNetworkManager>();
    }





    /// <summary>
    /// Called when the server disconnects.
    /// 
    /// Passes the event to the VRBNetworkManager.
    /// </summary>
    /// <param name="conn">The network connection the server disconnected on.</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        networkManager.OnServerDisconnected(conn);
    }





    /// <summary>
    /// Called when the client disconnects.
    /// 
    /// Passes the event to the VRBNetworkManager.
    /// </summary>
    /// <param name="conn">The network connection the client disconnected on.</param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        networkManager.OnClientDisconnect(conn);
    }


}
