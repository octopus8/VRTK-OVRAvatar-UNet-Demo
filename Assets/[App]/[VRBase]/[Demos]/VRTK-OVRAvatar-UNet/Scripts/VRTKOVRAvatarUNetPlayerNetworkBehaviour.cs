using UnityEngine;
using UnityEngine.Networking;
using VRTK;


/// <summary>
/// The network player used by the VRTK-OVRAvatar-UNet Demo.
/// </summary>
public class VRTKOVRAvatarUNetPlayerNetworkBehaviour : NetworkBehaviour
{
    #region SCRIPT INTERFACE VARIABLES

    [Tooltip("The remote avatar. This avatar is created when the network player is created.")]
    [SerializeField]
    GameObject remoteAvatar;


    #endregion





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// Initializes the network player.
    /// 
    /// </summary>
    void Start()
    {
        if (isLocalPlayer)
        {
            VRBVRTKOVRAvatar playerAvatar = VRTK_SharedMethods.FindEvenInactiveComponent<VRBVRTKOVRAvatar>();
            if (null != playerAvatar)
            {
                transform.SetParent(playerAvatar.gameObject.transform);
            }
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.position = Vector3.zero;
#if UNITY_ANDROID   // BAL Note: Only create the avatar if running on Android for now.
            GameObject remotePlayer = Instantiate(remoteAvatar, transform);
#endif
        }
    }


    #endregion
    

}