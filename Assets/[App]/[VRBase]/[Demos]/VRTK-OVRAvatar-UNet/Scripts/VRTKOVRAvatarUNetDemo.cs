using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using VRTK;


/// <summary>
/// This object contains core functionality for the VRTK-OVRAvatar-UNEt demo.
/// 
/// This is a demo project demonstrating the use of VRTK, OVRAvatar, and UNet together.
/// It currently only works on Gear VR, but since I used VRTK getting it to work on other
/// platforms shouldn’t be too hard. All the code written by me is in the [App] directory.
/// Other directories at the root of the Assets folder are imported packages.
/// The [App] directory contains the directories [Demos], [Dev], [System], and VRTK.
/// The [Demos] directory contains this demo. The [Dev] directory contains resources
/// created during development and are temporary. The [System] directory contains a
/// library of application independent resources. All other directories under [App]
/// contain resources pulled from imported resources and modified.
/// 
/// This app starts the user in a room with a menu in front. Users can move around using
/// the DPad of the controller. The player is represented by an Oculus Avatar. To the right
/// of the player is a second avatar; a twin avatar. This was done simply for test and demonstration purposes.
/// 
/// Using the right controller, the user can select either to host an internet game or a local game.
/// Once selected, a capsule will appear below the player.This was done simply to have a visual
/// indication of the match being created.The menu also changes to allow the user to quit hosting.
/// 
/// Other devices can join the match. Once a match is created, other devices will see the name of
/// the match in the list in the middle of the view.When a match is clicked, the user joins the match
/// and now visible among other players.Movement is synchronized across devices.The menu also changes
/// to allow the client to quit. Additionally, a user can pull a trigger to send a simple
/// message (the number 8) to the other devices.
/// 
/// </summary>
public class VRTKOVRAvatarUNetDemo : MonoBehaviour
{
    #region SCRIPT INTERFACE VARIABLES


    [Tooltip("The local player avatar. This is the avatar under the VRTK SDK GameObject.")]
    [SerializeField]
    OvrAvatar localPlayerAvatar;

    [Tooltip("The twin avatar prefab.")]
    [SerializeField]
    OvrAvatar twinAvatarPrefab;

    [Tooltip("The parent GameObject for the twin avatar.")]
    [SerializeField]
    Transform twinAvatarParent;

    [Tooltip("The Player created by UNet when a player connects or creates a network game.")]
    [SerializeField]
    GameObject networkPlayerPrefab;

    [Tooltip("Join button prefab.")]
    [SerializeField]
    VRTKOVRAvatarUNetJoinButton joinButtonPrefab;

    [Tooltip("Join button container.")]
    [SerializeField]
    Transform buttonContainerTransform;

    [Tooltip("The right hand controller.")]
    [SerializeField]
    VRTK_ControllerEvents rightHandController;

    [Tooltip("The menu canvas.")]
    [SerializeField]
    GameObject menuCanvas;

    [Tooltip("Host internet button.")]
    [SerializeField]
    GameObject hostInternetButton;

    [Tooltip("Host local button.")]
    [SerializeField]
    GameObject hostLocalButton;

    [Tooltip("Quit host button.")]
    [SerializeField]
    GameObject quitHostButton;

    [Tooltip("Quit client button.")]
    [SerializeField]
    GameObject quitClientButton;


    #endregion





    #region PRIVATE VARIABLES


    /// <summary>
    /// The twin avatar GameObject created.
    /// </summary>
    GameObject twinAvatar;


    #endregion





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// Performs initialization tasks.
    /// 
    /// The player prefab used by UNet is set. The Oculus user ID for the user is obtained, the twin avatar is created, and polling for network matches is started.
    /// </summary>
    private IEnumerator Start()
    {
        // Register delegate to be notified on changes to the available matches.
        VRBNetworkManager.OnAvailableMatchesChanged += AvailableMatchesList_OnAvailableMatchesChanged;
        VRBNetworkManager.OnClientConnectedAction += OnClientConnected;

        // Set the player prefab for UNet.
        VRBNetworkManager.Instance.SetPlayerPrefab(networkPlayerPrefab);

        // Get the Oculus user ID for the user. This needs to be done before the Start method of the OVRAvatar
        // for the twin avatar.
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        string userID = VRBOculusPlatform.Instance.GetOculusUserID();
        float startTime = Time.time;
        while ((null == userID) && (Time.time - startTime < 5.0f))
        {
            yield return wait;
            userID = VRBOculusPlatform.Instance.GetOculusUserID();
        }

        // Create the twin avatar.
        twinAvatar = Instantiate(twinAvatarPrefab, twinAvatarParent).gameObject;

        // Start polling for matches.
        VRBNetworkManager.Instance.StartPollingForInternetMatches();
    }





    /// <summary>
    /// Updates the player according to input.
    /// </summary>
    private void Update()
    {
        if (!rightHandController.buttonOnePressed)
        {
            Vector2 touchpadTouch = rightHandController.GetTouchpadAxis() * 0.01f;
            localPlayerAvatar.gameObject.transform.position += new Vector3(touchpadTouch.x, 0, touchpadTouch.y);
        }

        if (rightHandController.triggerPressed)
        {
            SendTestMessage();
        }
    }


    #endregion





    #region PUBLIC METHODS


    /// <summary>
    /// Callback called when the "Host Internet" button is pressed.
    /// 
    /// The internet match is created and the UI is updated.
    /// </summary>
    public void StartHostingInternetMatch()
    {
        VRBNetworkManager.Instance.StartHostingInternetMatch("test internet match");
        hostInternetButton.SetActive(false);
        hostLocalButton.SetActive(false);
        quitClientButton.SetActive(false);
        quitHostButton.SetActive(true);
        ClearButtons();
    }





    /// <summary>
    /// Callback called when the "Host LAN" button is pressed.
    /// 
    /// The local match is created and the UI is updated.
    /// </summary>
    public void StartHostingLANMatch()
    {
        VRBNetworkManager.Instance.StartHostingLANMatch();
        hostInternetButton.SetActive(false);
        hostLocalButton.SetActive(false);
        quitClientButton.SetActive(false);
        quitHostButton.SetActive(true);
        VRBNetworkManager.Instance.StopPollingForInternetMatches();
        ClearButtons();
    }





    /// <summary>
    /// Called by the Join button when it is pressed.
    /// 
    /// The match is joined and the UI is updated.
    /// </summary>
    public void StartClient(VRBMatchInfo matchInfo)
    {
        VRBNetworkManager.Instance.JoinMatch(matchInfo);
        VRBNetworkManager.Instance.StopPollingForInternetMatches();
        hostInternetButton.SetActive(false);
        hostLocalButton.SetActive(false);
        quitClientButton.SetActive(true);
        quitHostButton.SetActive(false);
        ClearButtons();
    }





    /// <summary>
    /// Called when the "Quit Client" button is pressed.
    /// 
    /// The client connection is ended and the UI is updated.
    /// </summary>
    public void QuitClient()
    {
        VRBNetworkManager.Instance.QuitClient();
        VRBNetworkManager.Instance.StartPollingForInternetMatches();
        hostInternetButton.SetActive(true);
        hostLocalButton.SetActive(true);
        quitClientButton.SetActive(false);
        quitHostButton.SetActive(false);
    }





    /// <summary>
    /// Called when the "Quit Host" button is pressed.
    /// 
    /// The host connection is ended and the UI is updated.
    /// </summary>
    public void QuitHost()
    {
        VRBNetworkManager.Instance.QuitHost();
        VRBNetworkManager.Instance.StartPollingForInternetMatches();
        hostInternetButton.SetActive(true);
        hostLocalButton.SetActive(true);
        quitClientButton.SetActive(false);
        quitHostButton.SetActive(false);
    }


    #endregion





    #region PRIVATE METHODS


    /// <summary>
    /// Delegate registered to be called when a client connects.
    /// 
    /// The client's message handlers are set.
    /// </summary>
    /// <param name="obj"></param>
    void OnClientConnected(NetworkConnection obj)
    {
        obj.RegisterHandler(MsgType.Highest + 1, OnNetMessage);
    }





    /// <summary>
    /// Called to send the test message.
    /// 
    /// A test message is sent to all connections.
    /// </summary>
    void SendTestMessage()
    {
        TestMessage testMessage = new TestMessage();
        testMessage.someval = 8;
        NetworkServer.SendToAll(MsgType.Highest + 1, testMessage);
    }





    /// <summary>
    /// Delegate registered to be notified on changes to the available matches.
    /// 
    /// The list of buttons is cleared and recreated.
    /// </summary>
    /// <param name="internetMatches">A list of available internet matches.</param>
    /// <param name="lanMatches">A list of available LAN matches.</param>
    void AvailableMatchesList_OnAvailableMatchesChanged(Dictionary<string, VRBMatchInfo> matches)
    {
        // Destroy the existing buttons.
        ClearButtons();

        // Create buttons for the current set of matches.
        foreach (var match in matches)
        {
            var button = Instantiate(joinButtonPrefab);
            button.Initialize(match.Value, buttonContainerTransform);
        }
    }




    /// <summary>
    /// Clears the list of available match buttons.
    /// </summary>
    void ClearButtons()
    {
        var buttons = GetComponentsInChildren<VRTKOVRAvatarUNetJoinButton>();
        foreach (var button in buttons)
        {
            Destroy(button.gameObject);
        }
    }



    /// <summary>
    /// Callback registred upon receiving the test message.
    /// </summary>
    /// <param name="netMsg"></param>
    void OnNetMessage(NetworkMessage netMsg)
    {
        TestMessage msg = netMsg.ReadMessage<TestMessage>();
        Debug.Log("message: "  + msg.someval);
    }


    #endregion





    #region DATA STRUCTURES


    /// <summary>
    /// A test message.
    /// </summary>
    public class TestMessage : MessageBase
    {
        public int someval;
    }


    #endregion


}
