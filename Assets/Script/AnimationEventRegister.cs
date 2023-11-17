using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventRegister : MonoBehaviour
{
    [SerializeField] UnityEvent StartAnimationEvent;
    [SerializeField] UnityEvent FinishAnimationEvent;

    public void InvokeStartEvent() => StartAnimationEvent?.Invoke();
    public void InvokeFinishEvent() => FinishAnimationEvent?.Invoke();
}
