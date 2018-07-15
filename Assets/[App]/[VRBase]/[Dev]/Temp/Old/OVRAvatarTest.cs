using Oculus.Platform;
using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRAvatarTest : MonoBehaviour
{
    [SerializeField]
    OvrAvatar avatar;



    private IEnumerator Start()
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        string userID = VRBOculusPlatform.Instance.GetOculusUserID();
        while (null == userID)
        {
            yield return wait;
            userID = VRBOculusPlatform.Instance.GetOculusUserID();
        }

        Instantiate(avatar);
    }



}
