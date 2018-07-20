using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// The network discorvery object used to announce and discover local matches.
/// </summary>
public class VRBNetworkDiscovery : NetworkDiscovery
{
    #region PRIVATE VARIABLES

    
    /// <summary>
    /// The network manager.
    /// </summary>
    VRBNetworkManager networkManager;


    #endregion





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// The network discovery script is initialized.
    /// </summary>
    private void Awake()
    {
        networkManager = GetComponent<VRBNetworkManager>();
        showGUI = false;
        useNetworkManager = true;
    }

    #endregion





    #region PUBLIC METHODS


    /// <summary>
    /// Called by the network manager to start hosting.
    /// </summary>
    public void StartHost()
    {
        Initialize();
        StartAsServer();
    }





    /// <summary>
    /// Called by the network manager to start hosting.
    /// </summary>
    public void StopHost()
    {
    }





    /// <summary>
    /// Called by the network manager to start hosting.
    /// </summary>
    public void StartClient()
    {
        Initialize();
        StartAsClient();
    }





    /// <summary>
    /// Called by the network manager to start hosting.
    /// </summary>
    public void StopClient()
    {
        StopBroadcast();
    }


    #endregion





    #region NETWORKDISCOVERY OVERRIDES


    /// <summary>
    /// Called when a broadcast from a local host is received.
    /// 
    /// The broadcast is passed to the network manager.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        networkManager.OnReceivedLANBroadcast(fromAddress, data);
    }


    #endregion


}
