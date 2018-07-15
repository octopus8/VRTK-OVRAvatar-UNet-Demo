using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using VRTK;


// 
// <summary>
// XXX
// </summary>
// <param name="XXX">XXX</param>


/// <summary>
/// The main demo script.
/// </summary>
public class VRTKOVRAvatarUNetDemo : MonoBehaviour
{
    [Tooltip("The player avatar.")]
    [SerializeField]
    OvrAvatar avatarLocal;

    [Tooltip("The avatar prefab. This is used to create the twin avatar.")]
    [SerializeField]
    OvrAvatar avatarPrefab;

    [Tooltip("The parent GameObject for the twin avatar.")]
    [SerializeField]
    Transform avatarParent;

    [SerializeField]
    GameObject playerPrefab;


    [SerializeField]
    VRTKOVRAvatarUNetJoinButton joinButtonPrefab;

    [SerializeField]
    Transform buttonContainerTransform;

    [SerializeField]
    VRTK_ControllerEvents rightHandController;

    [SerializeField]
    VRTK_ControllerEvents leftHandController;

    GameObject mirrorAvatar;


    private IEnumerator Start()
    {
        // Set the player prefab for UNet.
        VRBNetworkManager.Instance.SetPlayerPrefab(playerPrefab);

        WaitForSeconds wait = new WaitForSeconds(1.0f);
        string userID = VRBOculusPlatform.Instance.GetOculusUserID();
        while (null == userID)
        {
            yield return wait;
            userID = VRBOculusPlatform.Instance.GetOculusUserID();
        }

        mirrorAvatar = Instantiate(avatarPrefab, avatarParent).gameObject;
    }






    public void StartHostingInternetMatch()
    {
        VRBNetworkManager.Instance.StartHostingInternetMatch("test internet match");
    }






    public void StartHostingLANMatch()
    {
        VRBNetworkManager.Instance.StartHostingLANMatch();
    }




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
            button.Initialize(match, null, buttonContainerTransform);
        }
        foreach (var match in lanMatches)
        {
            var button = Instantiate(joinButtonPrefab);
            button.Initialize(null, match, buttonContainerTransform);
        }
    }

    GameObject networkPlayer = null;

    bool testval = false;
    private void Update()
    {
        if (networkPlayer == null)
        {
            NetworkBehaviour[] networkBehaviours = VRTK_SharedMethods.FindEvenInactiveComponents<NetworkBehaviour>();
            foreach (NetworkBehaviour b in networkBehaviours)
            {
                if (b.isLocalPlayer)
                {
                    networkPlayer = b.gameObject;
                    break;
                }
            }
        }
        if (null != networkPlayer)
        {
            if (!rightHandController.buttonOnePressed)
            {
                Vector2 touchpadTouch = rightHandController.GetTouchpadAxis() * 0.01f;
                networkPlayer.transform.position += new Vector3(touchpadTouch.x, 0, touchpadTouch.y);
            }

        }

        if (leftHandController.AnyButtonPressed())
        {
            VRTK_Logger.Info("XXX left hand input detected.");

        }


        if (rightHandController.touchpadTouched)
        {
            
            VRTK_Logger.Info("rctrl: " + rightHandController.GetControllerType().ToString() + ", lctrl: " + leftHandController.GetControllerType().ToString());
        }



        /*
                if (!vRTK_ControllerEvents.buttonOnePressed)
                {
                    Vector2 touchpadTouch = vRTK_ControllerEvents.GetTouchpadAxis() * 0.01f;
                    avatarLocal.transform.position += new Vector3(touchpadTouch.x, 0, touchpadTouch.y);
                }
        */
        /*
                if (!testval && vRTK_ControllerEvents.touchpadTouched)
                {
                    testval = true;


                    //            Instantiate(avatar);

                    if (null != avatarPrefab)
                    {
                        VRTK_Logger.Info("avatar pos: (" + avatarLocal.gameObject.transform.position.x + ", " + avatarLocal.gameObject.transform.position.y + ", " + avatarLocal.gameObject.transform.position.z + ")");
                        Vector3 p = mirrorAvatar.transform.position;
                        VRTK_Logger.Info("mirror avatar: " + p.x + ", " + p.y + ", " + p.z + ")");
                        p = VRTK_SDK_Bridge.GetHeadsetCamera().position;
                        VRTK_Logger.Info("cam pos: " + p.x + ", " + p.y + ", " + p.z + ")");
                    }
                }
        */
    }


}
