using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;


/// <summary>
/// An interface to UNet functionality. This class is implemented as a VRBSingletonAsComponent.
/// 
/// BAL Note: If a host has not been heard from in some time, it should time out. Connections are
/// recording the time of last broadcast, but nothing is being done with this currently.
/// 
/// </summary>
public class VRBNetworkManager : VRBSingletonAsComponent<VRBNetworkManager>
{
    #region PUBLIC STATIC MEMBERS


    /// <summary>
    /// Delegates called when match list has been updated. 
    /// </summary>
    public static event Action<Dictionary<string, VRBMatchInfo>> OnAvailableMatchesChanged = delegate { };

    /// <summary>
    /// Delegates called when a client connects. 
    /// </summary>
    public static event Action<NetworkConnection> OnClientConnectedAction = delegate { };

    /// <summary>
    /// Delegates called when a client disconnects.
    /// </summary>
    public static event Action OnClientDisconnectedAction = delegate { };


    #endregion





    #region PRIVATE VARIABLES


    /// <summary>
    /// A dictionary of available matches. For internet matches, the key is the network ID. For local matches
    /// the key is the IP address.
    /// </summary>
    Dictionary<string, VRBMatchInfo> matches = new Dictionary<string, VRBMatchInfo>();

    /// <summary>
    /// The network manager object. This is created at Awake.
    /// </summary>
    VRBNetworkManagerImpl networkManager;

    /// <summary>
    /// The network discovery object. This is created at Awake.
    /// </summary>
    VRBNetworkDiscovery networkDiscovery;

    /// <summary>
    /// Coroutine for polling for internet matches.
    /// </summary>
    Coroutine pollForInternetMatchesCorot = null;


    #endregion





    /// <summary>
    /// Standard VRBSingletonAsComponent Instance method.
    /// 
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





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// The network manager and network discovery objects are created.
    /// </summary>
    public void Awake()
    {
        networkManager = gameObject.AddComponent<VRBNetworkManagerImpl>();
        networkDiscovery = gameObject.AddComponent<VRBNetworkDiscovery>();
    }


    #endregion





    #region PUBLIC METHODS


    /// <summary>
    /// Sets the player prefab used by UNet.
    /// </summary>
    /// <param name="playerPrefab">The player prefab used by UNet.</param>
    public void SetPlayerPrefab(GameObject playerPrefab)
    {
        networkManager.playerPrefab = playerPrefab;
    }

    
    
    
    
    /// <summary>
    /// Called to start hosting a LAN match.
    /// </summary>
    public void StartHostingLANMatch()
    {
        if (null != pollForInternetMatchesCorot)
        {
            StopPollingForInternetMatches();
            networkDiscovery.StopClient();
        }
        networkDiscovery.StartHost();
        networkManager.StartHost();
    }





    /// <summary>
    /// Called to start hosting an Internet match.
    /// </summary>
    /// <param name="matchName"></param>
    public void StartHostingInternetMatch(string matchName)
    {
        if (null != pollForInternetMatchesCorot)
        {
            StopPollingForInternetMatches();
            networkDiscovery.StopClient();
        }
        networkManager.StartMatchMaker();
        networkManager.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnMatchCreated);
    }





    /// <summary>
    /// Called to quit hosting.
    /// </summary>
    public void QuitHost()
    {
        networkDiscovery.StopHost();
        networkManager.StopMatchMaker();
        networkManager.StopHost();
    }





    /// <summary>
    /// Called to quit client connection.
    /// </summary>
    public void QuitClient()
    {
        networkDiscovery.StopClient();
        networkManager.StopMatchMaker();
        networkManager.StopClient();
    }





    /// <summary>
    /// Called to join a match.
    /// </summary>
    /// <param name="matchInfo">The match to join.</param>
    public void JoinMatch(VRBMatchInfo matchInfo)
    {
        // If an IP address is specified, then it is a LAN match.
        // Start the LAN match.
        if (null != matchInfo.ipAddress)
        {
            networkDiscovery.StartClient();
            networkManager.networkAddress = matchInfo.ipAddress;
            networkManager.StartClient();
        }

        // Otherwise, it is an internet match.
        // Start the internet match.
        else
        {
            // If the match maker has not been started, start it so the match can be joined.
            if (networkManager.matchMaker == null)
            {
                networkManager.StartMatchMaker();
            }

            // Join the match.
            networkManager.matchMaker.JoinMatch(matchInfo.networkID, "", "", "", 0, 0, JoinInternetMatchCallback);
        }
    }





    /// <summary>
    /// Called to start polling for internet matches.
    /// </summary>
    public void StartPollingForInternetMatches()
    {
        if (null == pollForInternetMatchesCorot)
        {
            networkDiscovery.StartClient();
            matches.Clear();
            pollForInternetMatchesCorot = StartCoroutine(PollForInternetMatchesCorot());
        }
    }





    /// <summary>
    /// Called to stop polling for internet matches.
    /// </summary>
    public void StopPollingForInternetMatches()
    {
        if (null != pollForInternetMatchesCorot)
        {
            StopCoroutine(pollForInternetMatchesCorot);
            pollForInternetMatchesCorot = null;
        }
    }





    /// <summary>
    /// Called by the network manager when a client disconnects.
    /// </summary>
    /// <param name="conn"></param>
    public void OnClientDisconnect(NetworkConnection conn)
    {
        QuitClient();
        OnClientDisconnectedAction();
    }





    /// <summary>
    /// Called by the network manager when a client connects.
    /// </summary>
    /// <param name="conn"></param>
    public void OnClientConnect(NetworkConnection conn)
    {
        OnClientConnectedAction(conn);
    }

    
    
    
    
    /// <summary>
    /// Called by the network discovery object when a LAN broadcast is received.
    /// </summary>
    /// <param name="fromAddress">IP address of sender.</param>
    /// <param name="data">Data sent.</param>
    public void OnReceivedLANBroadcast(string fromAddress, string data)
    {
        // If the match already is in the list, then update the last broadcast time and return.
        if (matches.ContainsKey(fromAddress))
        {
            matches[fromAddress].lastBroadcast = Time.time;
            return;
        }

        // The match is not already in the list.
        // Create, init, and add the match.
        VRBMatchInfo matchInfo = new VRBMatchInfo();
        matchInfo.ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - fromAddress.LastIndexOf(":") - 1);
        matchInfo.name = "local";
        matchInfo.lastBroadcast = Time.time;
        matches.Add(fromAddress, matchInfo);

        // Notify registered delegates.
        OnAvailableMatchesChanged(matches);
    }

    
    #endregion





    #region PRIVATE METHODS


    /// <summary>
    /// Callback called when an internet match is succssfully created.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="responseData"></param>
    void OnMatchCreated(bool success, string extendedInfo, MatchInfo responseData)
    {
        networkManager.StartHost(responseData);
    }







    /// <summary>
    /// Callback called when an internet match is successfully joined.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="responseData"></param>
    void JoinInternetMatchCallback(bool success, string extendedInfo, MatchInfo responseData)
    {
        StopPollingForInternetMatches();
        networkManager.StartClient(responseData);
    }





    /// <summary>
    /// Callback called when the list of internet matches is obtained.
    /// 
    /// The current list of internet matches is updated. If a change is detected, delegates are notified.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="responseData"></param>
    void HandleListInternetMatchesComplete(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        // Get a list of current internet matches that are not in the response data.
        List<string> notFoundList = new List<string>();
        foreach (KeyValuePair<string, VRBMatchInfo> match in matches)
        {
            if (match.Value.networkID != NetworkID.Invalid)
            {
                bool found = false;
                foreach (MatchInfoSnapshot response in responseData)
                {
                    if (response.networkId.ToString() == match.Key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    notFoundList.Add(match.Key);
                }
            }
        }

        // If current internet matches were not found in the response data, then remove the missing matches.
        bool changed = false;
        if (notFoundList.Count > 0)
        {
            changed = true;
            foreach (var notFoundID in notFoundList)
            {
                matches.Remove(notFoundID);
            }
        }

        // Add any new matches found in the response data.
        foreach (MatchInfoSnapshot response in responseData)
        {
            string networkIDString = response.networkId.ToString();
            if (matches.ContainsKey(networkIDString))
            {
                continue;
            }

            VRBMatchInfo matchInfo = new VRBMatchInfo();
            matchInfo.networkID = response.networkId;
            matchInfo.name = response.name;
            matches.Add(networkIDString, matchInfo);
            changed = true;
        }

        // If the list changed, then notify delegates.
        if (changed)
        {
            OnAvailableMatchesChanged(matches);
        }
    }





    IEnumerator PollForInternetMatchesCorot()
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        while (true)
        {
            yield return wait;
            GetInternetMatches(HandleListInternetMatchesComplete);
        }
    }

    
    
    
    
    /// <summary>
    /// Requests a list of internet matches.
    /// </summary>
    /// <param name="handleGetMatchesComplete">Callback called when the list is obtained.</param>
    void GetInternetMatches(NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>> handleGetMatchesComplete)
    {
        // Make sure the matchmaker has been started.
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        // Get a list of internet matches from the match maker.
        networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, handleGetMatchesComplete);
    }


    #endregion


}


#region DATA STRUCTURES


/// <summary>
/// LAN connection info.
/// </summary>
public class VRBLANConnectionInfo
{
    /// <summary>
    /// The connection IP address.
    /// </summary>
    public string ipAddress;

    /// <summary>
    /// The connection name.
    /// </summary>
    public string name;





    /// <summary>
    /// Initializes variables.
    /// 
    /// The IP address is translated from an ipv6 to an ipv4 IP address.
    /// </summary>
    /// <param name="fromAddress"></param>
    /// <param name="data"></param>
    public VRBLANConnectionInfo(string fromAddress, string data)
    {
        ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - fromAddress.LastIndexOf(":") - 1);
        name = "local";
    }
}





/// <summary>
/// Match info.
/// </summary>
public class VRBMatchInfo
{
    public NetworkID networkID = NetworkID.Invalid;
    public string ipAddress;
    public string name;
    public float lastBroadcast;
}


#endregion