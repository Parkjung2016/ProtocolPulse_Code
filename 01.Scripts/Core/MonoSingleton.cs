using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    Debug.Log(typeof(T).Name + " could not be found");
                }
            }

            return _instance;
        }
    }
}