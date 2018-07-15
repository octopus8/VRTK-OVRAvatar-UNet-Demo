using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Parent class for Singleton classes.
 * 
 **************************************************************************
 * Although not necessary, subclasses should implement an Instance method like
 * the one that follows. Otherwise the result from Instance will need to be casted.
 * 
 * Standard VRBSingletonAsComponent Instance method:
 
public static VRBEntitlementCheck Instance
{
    get
    {
        return ((VRBEntitlementCheck)_Instance);
    }

    set
    {
        _Instance = value;
    }
}
 **************************************************************************
 **/
public class VRBSingletonAsComponent<T> : MonoBehaviour where T : VRBSingletonAsComponent<T>
{
    private static T __Instance;
    private bool _alive = true;

    protected static VRBSingletonAsComponent<T> _Instance
    {
        get
        {
            if (!__Instance)
            {
                T[] managers = GameObject.FindObjectsOfType(typeof(T)) as T[];
                if (managers != null)
                {
                    if (managers.Length == 1)
                    {
                        __Instance = managers[0];
                        return __Instance;
                    }
                    else if (managers.Length > 1)
                    {
                        Debug.LogError("You have more than one " + typeof(T).Name + " in the scene. You only need 1, it's a singleton!");
                        for (int i = 0; i < managers.Length; ++i)
                        {
                            T manager = managers[i];
                            Destroy(manager.gameObject);
                        }
                    }
                }
                GameObject go;
                Object o = Resources.Load(typeof(T).Name, typeof(GameObject));
                if (null != o)
                {
                    go = Instantiate(o) as GameObject;
                }
                else
                {
                    go = new GameObject(typeof(T).Name, typeof(T));
                }
                __Instance = go.GetComponent<T>();
                DontDestroyOnLoad(__Instance.gameObject);
            }
            return __Instance;
        }

        set
        {
            __Instance = value as T;
        }
    }




    public static bool IsAlive
    {
        get
        {
            if (__Instance == null)
                return false;
            return __Instance._alive;
        }
    }




    public void Destroy()
    {
        Destroy(gameObject);
    }



    void OnDestroy()
    {
        _alive = false;
    }



    void OnApplicationQuit()
    {
        _alive = false;
    }

}