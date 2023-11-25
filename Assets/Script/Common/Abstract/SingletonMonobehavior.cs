using UnityEngine;

public abstract class SingletonMonobehavior<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance => _instance;
    /// <summary>Awakeのタイミングで実行したい処理を書く</summary>
    protected abstract void Initialize();
    protected void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }
}
