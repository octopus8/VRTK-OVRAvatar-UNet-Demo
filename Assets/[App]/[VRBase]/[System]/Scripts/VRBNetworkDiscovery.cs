using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class VRBNetworkDiscovery : NetworkDiscovery
{
    float timeout = 5.0f;

    VRBGameList gameList;


    Dictionary<VRBLANConnectionInfo, float> lanAddresses = new Dictionary<VRBLANConnectionInfo, float>();




    void OnEnable()
    {
        gameList = GetComponent<VRBGameList>();

        Initialize();

        StartAsClient();

        StartCoroutine(CleanupExpiredEntries());
    }






    public void StartBroadcast()
    {
        StopBroadcast();
        Initialize();
        StartAsServer();

    }


    IEnumerator CleanupExpiredEntries()
    {
        WaitForSeconds wait = new WaitForSeconds(timeout);
        while (true)
        {
            bool changed = false;

            var keys = lanAddresses.Keys.ToList();
            foreach (var key in keys)
            {
                if (lanAddresses[key] <= Time.time)
                {
                    lanAddresses.Remove(key);
                    changed = true;
                }
            }
            if (changed)
            {
                UpdateMatchInfos();
            }

            yield return wait;
        }
    }




    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);

        VRBLANConnectionInfo info = new VRBLANConnectionInfo(fromAddress, data);
        if (lanAddresses.ContainsKey(info) == false)
        {
            lanAddresses.Add(info, Time.time + timeout);
            UpdateMatchInfos();
        }
        else
        {
            lanAddresses[info] = Time.time + timeout;
        }
    }



    private void UpdateMatchInfos()
    {
        gameList.HandleNewLANMatches(lanAddresses.Keys.ToList());
    }
}
