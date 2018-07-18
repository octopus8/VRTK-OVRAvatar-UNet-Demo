using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;



/// <summary>
/// This class handles both local and internet network connections.
/// 
/// Note: This class is implemented as a VRBSingletonAsComponent.
/// </summary>
public class VRBNetworkManager : VRBSingletonAsComponent<VRBNetworkManager>
{


    #region PRIVATE VARIABLES

    /// <summary>
    /// The UNet network manager.
    /// </summary>
    VRBNetworkManagerImpl networkManager;


    /// <summary>
    /// Coroutine used for polling for internet networks.
    /// </summary>
    Coroutine pollForInternetNetworkCoroutine;

    /// <summary>
    /// Delegates called when the available networks have changed.
    /// </summary>
    public event Action<string> OnAvailableNetworksChanged = delegate { };

    /// <summary>
    /// Current networking state.
    /// </summary>
    NetworkingState currentNetworkingState = NetworkingState.idle;

    /// <summary>
    /// The network ID for the current internet connection.
    /// </summary>
    UnityEngine.Networking.Types.NetworkID internetNetworkID = UnityEngine.Networking.Types.NetworkID.Invalid;

    /// <summary>
    /// Flag indicating a request for the current list of internet networks is active.
    /// </summary>
    bool isObtainingInternetNetworks = false;

    #endregion





    /// <summary>
    /// Standard VRBSingletonAsComponent Instance method.
    /// </summary>
    public static VRBNetworkManager Instance
    {
        get
        {
            return ((VRBNetworkManager)_Instance);
        }

        set
        {
            _Instance = value;
        }
    }





    #region MONOBEHAVIOUR OVERRIDES


    /// <summary>
    /// Init method. Adds the NetworkManager component to the game object.
    /// </summary>
    private void Awake()
    {
        networkManager = gameObject.AddComponent<VRBNetworkManagerImpl>();
//        gameObject.AddComponent<NetworkManagerHUD>();
    }

    #endregion





    #region PUBLIC METHODS


    /// <summary>
    /// Sets the player prefab used by UNet.
    /// </summary>
    public void SetPlayerPrefab(GameObject playerPrefab)
    {
        networkManager.playerPrefab = playerPrefab;
    }





    /// <summary>
    /// Starts hosting a local network.
    /// </summary>
    public void StartHostingLocalNetwork()
    {
        // Handle the current network state.
        switch (currentNetworkingState)
        {
            case NetworkingState.hostLocal:
                // Already hosting a local network. Do nothing.
                return;

            case NetworkingState.hostInternet:
                QuitInternetNetwork();
                break;

            case NetworkingState.client:
                networkManager.StopClient();
                break;
        }


        // Start local host.
        networkManager.StartHost();
        currentNetworkingState = NetworkingState.hostLocal;
    }





    /// <summary>
    /// Starts hosting an internet network.
    /// </summary>
    public void StartHostingInternetNetwork()
    {
        // Handle the current network state.
        switch (currentNetworkingState)
        {
            case NetworkingState.hostInternet:
                // Already hosting an internet network. Do nothing.
                return;

            case NetworkingState.hostLocal:
                // Stop the local host.
                networkManager.StopHost();
                break;

            case NetworkingState.client:
                // Stop the client.
                networkManager.StopClient();
                break;
        }


        // If the match maker has not been started yet, start it.
        if (null == networkManager.matchMaker)
        {
            networkManager.StartMatchMaker();
        }


        // Create the match.
        // XXX need to set the name.
        string name = "inet match";// + (int)(UnityEngine.Random.value * 1000);
        networkManager.matchMaker.CreateMatch(name, 4, true, "", "", "", 0, 0, OnInternetMatchCreated);
    }





    /// <summary>
    /// Joins a local network.
    /// </summary>
    public void JoinLocalNetwork()
    {
        // Handle the current networking state.
        switch (currentNetworkingState)
        {
            case NetworkingState.client:
                return;

            case NetworkingState.hostLocal:
                // Stop the local host.
                networkManager.StopHost();
                break;

            case NetworkingState.hostInternet:
                QuitInternetNetwork();
                break;
        }


        // Start the client.
        networkManager.StartClient();
        currentNetworkingState = NetworkingState.client;
    }





    /// <summary>
    /// Joins an internet network.
    /// </summary>
    public void StartMonitoringAvailableInternetNetworks()
    {
        StartPollingForInternetNetworks();
    }





    /// <summary>
    /// Quits client. This works for both local and internet networks.
    /// </summary>
    public void QuitClient()
    {
        if (currentNetworkingState != NetworkingState.client)
        {
            return;
        }

        networkManager.StopClient();
        currentNetworkingState = NetworkingState.idle;
    }




    /// <summary>
    /// Quits hosting. This works for both local and internet networks.
    /// </summary>
    public void QuitHost()
    {
        if ((currentNetworkingState != NetworkingState.hostInternet) && (currentNetworkingState != NetworkingState.hostLocal))
        {
            return;
        }
        if (internetNetworkID != UnityEngine.Networking.Types.NetworkID.Invalid)
        {
            QuitInternetNetwork();
        }
        networkManager.StopHost();
        currentNetworkingState = NetworkingState.idle;
    }





    /// <summary>
    /// Stops polling for internet networks.
    /// </summary>
    public void StopPollingForInternetNetworks()
    {
        // If not polling, do nothing.
        if (null == pollForInternetNetworkCoroutine)
        {
            return;
        }


        // Stop the polling coroutine.
        StopCoroutine(pollForInternetNetworkCoroutine);
        pollForInternetNetworkCoroutine = null;
    }




    /// <summary>
    /// Called by VRBNetworkManagerImpl when the server disconnects.
    /// </summary>
    public void OnServerDisconnected(NetworkConnection conn)
    {
        switch (currentNetworkingState)
        {
            case NetworkingState.client:
                networkManager.StopClient();
                currentNetworkingState = NetworkingState.idle;
                break;
        }
    }





    /// <summary>
    /// Called by VRBNetworkManagerImpl when the server disconnects.
    /// </summary>
    public void OnClientDisconnect(NetworkConnection conn)
    {
        switch (currentNetworkingState)
        {
            case NetworkingState.client:
                currentNetworkingState = NetworkingState.idle;
                break;
        }
    }

    #endregion





    #region PRIVATE METHODS


    /// <summary>
    /// Starts polling for internet networks.
    /// </summary>
    void StartPollingForInternetNetworks()
    {
        // If already polling, do nothing.
        if (null != pollForInternetNetworkCoroutine)
        {
            return;
        }


        // Start a coroutine to get the internet networks available.
        pollForInternetNetworkCoroutine = StartCoroutine(PeriodicPollForInternetNetwork());
    }





    /// <summary>
    /// Callback called when an internet match is created.
    /// </summary>
    private void OnInternetMatchCreated(bool success, string extendedInfo, MatchInfo responseData)
    {
        // Store the internet network ID.
        internetNetworkID = responseData.networkId;


        // Start the host.
        networkManager.StartHost(responseData);
        currentNetworkingState = NetworkingState.hostInternet;
    }





    /// <summary>
    /// Coroutine used to periodically poll for available internet networks.
    /// </summary>
    IEnumerator PeriodicPollForInternetNetwork()
    {
        // If the match maker has not been started, then start it.
        if (null == networkManager.matchMaker)
        {
            networkManager.StartMatchMaker();
        }


        // Get the list of matches periodically.
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        while (true)
        {
            yield return wait;

            // If not already obtaining the list of available internet matches, obtain the list.
            // This flag is cleared in the callback.
            if (!isObtainingInternetNetworks)
            {
                isObtainingInternetNetworks = true;
                networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, HandleListInternetMatchesComplete);
            }
        }
    }






    /// <summary>
    /// Calls the delegates registered to receive notice that the list of internet networks has changed.
    /// 
    /// XXX Temporarly, it is joining the first network available.
    /// </summary>
    void HandleListInternetMatchesComplete(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        // XXX test: join the first network available.
        if (responseData.Count > 0)
        {
            // If hosting, stop the host.
            if ((currentNetworkingState == NetworkingState.hostInternet) || (currentNetworkingState == NetworkingState.hostLocal))
            {
                networkManager.StopHost();
            }

            networkManager.matchMaker.JoinMatch(responseData[0].networkId, "", "", "", 0, 0, HandleJoinInternetMatch);
            currentNetworkingState = NetworkingState.client;
        }


        // Call the delegates registered to receive notice that the list of internet networks has changed.
        //        OnAvailableMatchesChanged("testcrap");
        isObtainingInternetNetworks = false;
    }





    /// <summary>
    /// Callback called when an internet match as successfully been joined.
    /// </summary>
    private void HandleJoinInternetMatch(bool success, string extendedInfo, MatchInfo responseData)
    {
        // Start the client.
        StopCoroutine(pollForInternetNetworkCoroutine);
        pollForInternetNetworkCoroutine = null;
        networkManager.StopMatchMaker();
        networkManager.StartClient(responseData);
    }





    /// <summary>
    /// Quits the internet network.
    /// </summary>
    private void QuitInternetNetwork()
    {
        networkManager.matchMaker.DestroyMatch(internetNetworkID, 0, null);
        internetNetworkID = UnityEngine.Networking.Types.NetworkID.Invalid;
        // Stop the match maker.
        networkManager.StopMatchMaker();
    }


    #endregion





    #region DATA STRUCTURES


    /// <summary>
    /// The current networking state.
    /// </summary>
    enum NetworkingState
    {
        idle,
        hostLocal,
        hostInternet,
        client
    };


    #endregion


}