using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



/// <summary>
/// A custom implementation of the NetworkManager class.
/// 
/// This simply receives messages and passes them to the network manager.
/// </summary>
public class VRBNetworkManagerImpl : NetworkManager
{
    #region PRIVATE VARIABLES


    /// <summary>
    /// The network manager.
    /// </summary>
    VRBNetworkManager networkManager;


    #endregion





    #region MONOBEHAVIOUR OVERRIDES


    /// <summary>
    /// Caches a reference to the network manager.
    /// </summary>
    void Start()
    {
        networkManager = GetComponent<VRBNetworkManager>();    
    }





    /// <summary>
    /// Called when a client connects.
    /// 
    /// Passes the message to the network manager.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        networkManager.OnClientConnect(conn);
    }





    /// <summary>
    /// Called when a client disconnects.
    /// 
    /// Passes the message to the network manager.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        networkManager.OnClientDisconnect(conn);
    }


    #endregion


}
