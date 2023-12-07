using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public interface IPlayerView
{
    void OnInitialize();
    void OnWaiting();
    void OnWalk();
    UniTask OnDead();
}
public class PlayerView : IPlayerView
{
    float _deadAnimationTime;
    static readonly int _hashInitialize = Animator.StringToHash("Initialize");
    static readonly int _hashWaiting = Animator.StringToHash("Waiting");
    static readonly int _hashWalking = Animator.StringToHash("Walking");
    static readonly int _hashDead = Animator.StringToHash("Dead");
    Animator _animator;
    public PlayerView(float deadAnimationTime, Animator animator)
    {
        _deadAnimationTime = deadAnimationTime;
        _animator = animator;
    }
    public void OnInitialize()
        => _animator.SetTrigger(_hashInitialize);
    public void OnWaiting()
        => _animator.SetTrigger(_hashWaiting);
    public void OnWalk()
        => _animator.SetTrigger(_hashWalking);
    public async UniTask OnDead()
    {
        _animator.SetTrigger(_hashDead);
        await UniTask.Delay((int)(_deadAnimationTime * 1000)); 
    }
}
