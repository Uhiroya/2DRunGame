using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
    private static readonly int HashInitialize = Animator.StringToHash("Initialize");
    private static readonly int HashWaiting = Animator.StringToHash("Waiting");
    private static readonly int HashWalking = Animator.StringToHash("Walking");
    private static readonly int HashDead = Animator.StringToHash("Dead");
    private readonly Animator _animator;
    private readonly IAudioManager _audioManager;
    private readonly float _deadAnimationTime;

    public PlayerView(IAudioManager audioManager, float deadAnimationTime, Animator animator)
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
                OnFinishDeadAnimation?.Invoke();
                break;
        }
    }

    public void OnInitialize()
    {
        _animator.SetTrigger(HashInitialize);
    }

    public void OnWaiting()
    {
        _animator.SetTrigger(HashWaiting);
    }

    public void OnWalk()
    {
        _animator.SetTrigger(HashWalking);
    }

    public async UniTask OnDead()
    {
        _animator.SetTrigger(HashDead);
        await UniTask.Delay((int)(_deadAnimationTime * 1000));
    }
}
