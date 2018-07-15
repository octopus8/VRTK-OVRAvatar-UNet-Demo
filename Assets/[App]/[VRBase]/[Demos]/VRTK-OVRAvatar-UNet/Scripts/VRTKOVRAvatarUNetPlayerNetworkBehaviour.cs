using UnityEngine;
using UnityEngine.Networking;
using VRTK;

public class VRTKOVRAvatarUNetPlayerNetworkBehaviour : NetworkBehaviour
{
    [SerializeField]
    float moveSpeed = 1.0f;


    private void Start()
    {
        VRTK_Logger.Info("Creating Player");
        Vector3 pos = Vector3.zero;
        pos.z += 5;
        transform.position = pos;
    }
}
