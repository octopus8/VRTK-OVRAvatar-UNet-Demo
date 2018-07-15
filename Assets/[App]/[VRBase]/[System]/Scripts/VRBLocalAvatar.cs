using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBLocalAvatar : MonoBehaviour
{

    OvrAvatar avatar;


    // Use this for initialization
    void Awake()
    {
        avatar = GetComponent<OvrAvatar>();
        string userID = VRBOculusPlatform.Instance.GetOculusUserID();
        if (null != userID)
        {
            avatar.oculusUserID = userID;
        }
        else
        {
            avatar.oculusUserID = "295109307540267";
        }
    }



}
