using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;



/**
 * 
 **/
public class VRBNetworkManager : VRBSingletonAsComponent<VRBNetworkManager>
{
    VRBNetworkManagerImpl networkManager;
    VRBNetworkDiscovery networkDiscovery;





    /**
     * Standard VRBSingletonAsComponent Instance method.
     * 
     **/
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



    public void Awake()
    {
        GameObject g = new GameObject("VRBNetworkManagerImpl", typeof(VRBNetworkManagerImpl));
        g.transform.SetParent(_Instance.transform);
        networkManager = g.GetComponent<VRBNetworkManagerImpl>();

        networkDiscovery = GetComponent<VRBNetworkDiscovery>();
    }







    internal void GetInternetMatches(NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>  handleGetMatchesComplete)
    {
        // Make sure the matchmaker has been started.
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        // 
        networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, handleGetMatchesComplete);
    }






    public void StartHostingLANMatch()
    {
        networkDiscovery.StartBroadcast();
        networkManager.StartHost();
    }




    /**
     * Called to start hosting an Internet match.
     * 
     * The matchmaker is started, then a match is created.  Once the match is created, OnMatchCreated is called.
     * 
     **/
    public void StartHostingInternetMatch(string matchName)
    {
        networkManager.StartMatchMaker();
        networkManager.matchMaker.CreateMatch(matchName, 4, true, "", "", "", 0, 0, OnMatchCreated);
    }






    /**
     * Callback called when a match is created.
     * 
     * The Host is started, and the match list is refreshed.
     * 
     **/
    private void OnMatchCreated(bool success, string extendedInfo, MatchInfo responseData)
    {
        networkManager.StartHost(responseData);
    }







    internal void JoinMatch(MatchInfoSnapshot match, VRBLANConnectionInfo lanMatch)
    {
        if (null != match)
        {
            if (networkManager.matchMaker == null)
            {
                networkManager.StartMatchMaker();
            }

            networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, HandleJoinMatch);
        }
        else
        {
            networkManager.networkAddress = lanMatch.ipAddress;
            networkManager.StartClient();
        }
    }



    private void HandleJoinMatch(bool success, string extendedInfo, MatchInfo responseData)
    {
        networkManager.StartClient(responseData);
    }



    internal void SetPlayerPrefab(GameObject playerPrefab)
    {
        networkManager.playerPrefab = playerPrefab;
    }




}
