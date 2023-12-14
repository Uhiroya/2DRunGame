using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public interface IPlayerView
{
    event Action OnFinishDeadAnimation;
    UniTaskVoid OnPlayerConditionChanged(PlayerCondition playerCondition);
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
    IAudioManager _audioManager;
    public PlayerView(IAudioManager audioManager ,float deadAnimationTime, Animator animator)
    {
        _audioManager = audioManager;
        _deadAnimationTime = deadAnimationTime;
        _animator = animator;
    }
    public event Action OnFinishDeadAnimation;
    public async UniTaskVoid OnPlayerConditionChanged(PlayerCondition playerCondition)
    {
        switch (playerCondition)
        {
            case PlayerCondition.Initialize:
                OnInitialize();
                break;
            case PlayerCondition.Pause:
                OnWaiting();
                break;
            case PlayerCondition.Alive:
                OnWalk();
                break;
            case PlayerCondition.GetItem:
                _audioManager.PlaySE(GameSE.HitItem);
                break;
            case PlayerCondition.Dying:
                _audioManager.PlaySE(GameSE.HitEnemy);
                await OnDead();
                OnFinishDeadAnimation.Invoke();
                break;
            default:
                break;
        }
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
