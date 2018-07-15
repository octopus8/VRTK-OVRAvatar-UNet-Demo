using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class MatchListPanel : MonoBehaviour
{
    [SerializeField]
    VRTKOVRAvatarUNetJoinButton joinButtonPrefab;


    private void Awake()
    {

        VRBGameList.OnAvailableMatchesChanged += AvailableMatchesList_OnAvailableMatchesChanged;
    }

    private void AvailableMatchesList_OnAvailableMatchesChanged(List<MatchInfoSnapshot> internetMatches, List<VRBLANConnectionInfo> lanMatches)
    {
        ClearExistingButtons();
        CreateNewJoinGameButtons(internetMatches, lanMatches);
    }


    private void ClearExistingButtons()
    {
        var buttons = GetComponentsInChildren<VRTKOVRAvatarUNetJoinButton>();
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }
    }

    private void CreateNewJoinGameButtons(List<MatchInfoSnapshot> internetMatches, List<VRBLANConnectionInfo> lanMatches)
    {
        foreach (var match in internetMatches)
        {
            var button = Instantiate(joinButtonPrefab);
            button.Initialize(match, null, transform);
        }
        foreach (var match in lanMatches)
        {
            var button = Instantiate(joinButtonPrefab);
            button.Initialize(null, match, transform);
        }
    }

}
