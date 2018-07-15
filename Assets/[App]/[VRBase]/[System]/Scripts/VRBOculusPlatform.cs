using UnityEngine;
using Oculus.Avatar;
using Oculus.Platform;
using Oculus.Platform.Models;
using System.Collections;
using System;

public class VRBOculusPlatform : VRBSingletonAsComponent<VRBOculusPlatform>
{

    string oculusUserID = null;

    internal string GetOculusUserID()
    {
        return oculusUserID;
    }


    /**
     * Standard VRBSingletonAsComponent Instance method.
     * 
     **/
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

    /**
     * This method does nothing. If init functionality needs to be added, it should be added here.
     * The main purpose of this method is to insure the Instance member is referenced.
     * 
     **/
    internal void Initialize()
    {
    }





    void Awake()
    {
        Oculus.Platform.Core.Initialize();
        Oculus.Platform.Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
        Oculus.Platform.Request.RunCallbacks();  //avoids race condition with OvrAvatar.cs Start().
    }

    private void GetLoggedInUserCallback(Message<User> message)
    {
        if (!message.IsError)
        {
            oculusUserID = message.Data.ID.ToString();
        }
    }



}
