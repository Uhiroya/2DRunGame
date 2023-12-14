using UnityEngine;

public abstract class SingletonMonobehavior<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
            Initialize();
        }
    }

    /// <summary>Awakeのタイミングで実行したい処理を書く</summary>
    protected abstract void Initialize();
}
