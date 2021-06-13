using UnityEngine;

public abstract class AbstractScreen<T> : MonoBehaviour where T : Component {

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
                if (_instance == null) {
                    GameObject obj = new GameObject(typeof(T).ToString() + " - Singleton");
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    private static T _instance;

    protected virtual void Awake() {
        if (_instance == null || _instance == this) {
            _instance = this as T;
            gameObject.SetActive(false);
        } else {
            Destroy(gameObject);
        }
    }

    public void Show() {
        OnShow();
    }

    public void Hide() {
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }

}