using UnityEngine;

namespace Oasez.Extensions.Generics.Singleton
{
    /// <summary>
    /// Generic singleton class
    /// </summary>
    /// <typeparam name="T">typeof(Class)</typeparam>
    /// <typeparam name="A">Calltype for example: interface or the same class</typeparam>
    public abstract class GenericSingleton<T, A> : MonoBehaviour where T : Component, A
    {
        public static A Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).ToString() + " - Singleton");
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
        private static T _instance;

        protected virtual void Awake()
        {
            if (_instance == null || _instance == this)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else
            {
                Destroy(gameObject);
            }
        } 

    }
}