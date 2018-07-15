using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using System;

public class VRBGameList : MonoBehaviour
{

    // Delegates called when match list has been updated.
    public static event Action<List<MatchInfoSnapshot>, List<VRBLANConnectionInfo>> OnAvailableMatchesChanged = delegate { };

    // Current list of matches.
    static List<MatchInfoSnapshot> internetMatches = new List<MatchInfoSnapshot>();
    static List<VRBLANConnectionInfo> lanMatches = new List<VRBLANConnectionInfo>();



    // The next time to refresh the match list.
    float nextMatchListRefreshTime = 0;


    // The rate at which to query the server for the current match list.
    float matchListRefreshIntervalSeconds = 1.0f;


    VRBNetworkManager networkManager;


    // Use this for initialization
    void Start()
    {
        networkManager = GetComponent<VRBNetworkManager>();
    }




    // Update is called once per frame
    void Update()
    {
        // If it is time to refresh the match list, do so.
        if ((Time.time >= nextMatchListRefreshTime))
        {
            nextMatchListRefreshTime = Time.time + matchListRefreshIntervalSeconds;
            RefreshInternetMatches();
        }
    }




    void RefreshInternetMatches()
    {
        // Set the next match list refresh time.
        nextMatchListRefreshTime = Time.time + matchListRefreshIntervalSeconds;


        // 
        networkManager.GetInternetMatches(HandleListInternetMatchesComplete);
    }


    /**
     * Callback called when XXX.
     * 
     * 
     **/
    void HandleListInternetMatchesComplete(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
    {
        HandleNewInternetMatchList(responseData);
    }



    /**
     * Called when the match list has been updated.
     * 
     **/
    void HandleNewInternetMatchList(List<MatchInfoSnapshot> matchList)
    {
        // Cache the match list.
        internetMatches = matchList;

        // Call the delegates passing the new match list.
        OnAvailableMatchesChanged(internetMatches, lanMatches);
    }



    internal void HandleNewLANMatches(List<VRBLANConnectionInfo> list)
    {
        lanMatches = list;

        // Call the delegates passing the new match list.
        OnAvailableMatchesChanged(internetMatches, lanMatches);
    }
}
