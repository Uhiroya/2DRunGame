using UnityEngine;
using UnityEngine.Events;

public class AnimationEventRegister : MonoBehaviour
{
    [SerializeField] private UnityEvent StartAnimationEvent;
    [SerializeField] private UnityEvent FinishAnimationEvent;

    public void InvokeStartEvent()
    {
        StartAnimationEvent?.Invoke();
    }

    public void InvokeFinishEvent()
    {
        FinishAnimationEvent?.Invoke();
    }
}
