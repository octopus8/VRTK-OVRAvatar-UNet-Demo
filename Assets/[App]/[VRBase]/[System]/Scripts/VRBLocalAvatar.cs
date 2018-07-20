using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class is to be attached to all local avatars.
/// 
/// This class will set the Oculus Avatar user ID appropriately.
/// </summary>
public class VRBLocalAvatar : MonoBehaviour
{
    #region PRIVATE VARIABLES


    /// <summary>
    /// The Oculus avatar.
    /// </summary>
    OvrAvatar avatar;


    #endregion





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// Initializes the Oculus avatar.
    /// </summary>
    void Awake()
    {
        // Cache a reference to the avatar.
        avatar = GetComponent<OvrAvatar>();

        // Set the Oculus user ID.
        string userID = VRBOculusPlatform.Instance.GetOculusUserID();
        if (null != userID)
        {
            avatar.oculusUserID = userID;
        }
    }


    #endregion


}
