using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Parent class for Singleton classes that need MonoBehaviour support.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VRBSingletonAsComponent<T> : MonoBehaviour where T : VRBSingletonAsComponent<T>
{
    #region PRIVATE MEMBERS


    /// <summary>
    /// The instance of the Singleton object.
    /// </summary>
    private static T __Instance;

    /// <summary>
    /// Flag indicating whether or not the script is alive.
    /// </summary>
    private bool _alive = true;


    #endregion





    /// <summary>
    /// Access to the instance. 
    /// </summary>
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

                // This is taken out. It can be used for obtaining the Singleton from the Resources.
                if (false)
                {
                    Object o = Resources.Load(typeof(T).Name, typeof(GameObject));
                    if (null != o)
                    {
                        go = Instantiate(o) as GameObject;
                    }
                    else
                    {
                        go = new GameObject(typeof(T).Name, typeof(T));
                    }
                }
                else
                {
                    go = new GameObject(typeof(T).Name, typeof(T));
                }
                __Instance = go.GetComponent<T>();

                // BAL Note: The following line should be considered as an option.
//                DontDestroyOnLoad(__Instance.gameObject);
            }
            return __Instance;
        }

        set
        {
            __Instance = value as T;
        }
    }





    /// <summary>
    /// Returns the alive status.
    /// </summary>
    public static bool IsAlive
    {
        get
        {
            if (__Instance == null)
                return false;
            return __Instance._alive;
        }
    }





    /// <summary>
    /// Destroys the game object along with the script.
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }





    /// <summary>
    /// Sets the alive flag as false.
    /// </summary>
    void OnDestroy()
    {
        _alive = false;
    }





    /// <summary>
    /// Sets the alive flag a false.
    /// </summary>
    void OnApplicationQuit()
    {
        _alive = false;
    }


}