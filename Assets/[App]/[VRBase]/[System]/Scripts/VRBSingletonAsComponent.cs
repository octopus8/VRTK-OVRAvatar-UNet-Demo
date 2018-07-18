using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Parent class for Singleton classes.
/// 
/// Although not necessary, subclasses should implement an Instance method like
/// the one that follows.Otherwise the result from Instance will need to be cast.
/// 
/// 
/// public static VRBEntitlementCheck Instance
/// {
///     get
///     {
///         return ((VRBEntitlementCheck)_Instance);
///     }
/// 
///     set
///     {
///         _Instance = value;
///     }
/// }
/// </summary>
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


                // Attempt to load the Singleton as a resource.
                GameObject go;
                Object o = Resources.Load(typeof(T).Name, typeof(GameObject));


                // If the Singleton was obtained, then instantiate it.
                if (null != o)
                {
                    go = Instantiate(o) as GameObject;
                }

                // Otherwise, create a game object with the script attached.
                else
                {
                    go = new GameObject(typeof(T).Name, typeof(T));
                }


                // Cache a reference to the script.
                __Instance = go.GetComponent<T>();
                DontDestroyOnLoad(__Instance.gameObject);
            }


            // Return the created object.
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