using UnityEngine;
using Oculus.Avatar;
using Oculus.Platform;
using Oculus.Platform.Models;
using System.Collections;
using System;



/// <summary>
/// This class provides support for the Oculus platform. This class is implemented
/// as a VRBSingletonAsComponent.
/// </summary>
public class VRBOculusPlatform : VRBSingletonAsComponent<VRBOculusPlatform>
{
    #region PRIVATE MEMBERS


    /// <summary>
    /// The Oculus user ID for the local user.
    /// </summary>
    string oculusUserID = null;


    #endregion





    /// <summary>
    /// Standard VRBSingletonAsComponent Instance method.
    /// </summary>
    public static VRBOculusPlatform Instance
    {
        get
        {
            return ((VRBOculusPlatform)_Instance);
        }

        set
        {
            _Instance = value;
        }
    }





    #region MONOBEHAVIOUR METHODS


    /// <summary>
    /// The Oculus platform is initialized, and the logged in user is obtained.
    /// </summary>
    void Awake()
    {
        Oculus.Platform.Core.Initialize();
        Oculus.Platform.Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
        Oculus.Platform.Request.RunCallbacks();  //avoids race condition with OvrAvatar.cs Start().
    }


    #endregion




    /// <summary>
    /// This method does nothing. If init functionality needs to be added, it should be added here.
    /// The main purpose of this method is to insure the Instance member is referenced.
    /// </summary>
    public void Initialize()
    {
    }

    



    /// <summary>
    /// Get the Oculus user ID.
    /// </summary>
    /// <returns></returns>
    public string GetOculusUserID()
    {
        return oculusUserID;
    }





    #region PRIVATE METHODS


    /// <summary>
    /// Called when the logged in user is obtained.
    /// </summary>
    /// <param name="message"></param>
    void GetLoggedInUserCallback(Message<User> message)
    {
        if (!message.IsError)
        {
            oculusUserID = message.Data.ID.ToString();
        }
    }


    #endregion


}
