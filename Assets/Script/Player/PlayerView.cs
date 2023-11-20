using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IPlayerView
{
    void OnWaiting();
    void OnWalk();
    void OnDead();
}
public class PlayerView : IPlayerView
{
    static readonly int _hashWaiting = Animator.StringToHash("Waiting");
    static readonly int _hashWalking = Animator.StringToHash("Walking");
    static readonly int _hashDead = Animator.StringToHash("Dead");
    Animator _animator;
    public PlayerView(Animator animator)
    {
        _animator = animator;
    }
    public void OnWaiting()
        => _animator.SetTrigger(_hashWaiting);
    public void OnWalk()
        => _animator.SetTrigger(_hashWalking);
    public void OnDead()
        => _animator.SetTrigger(_hashDead);
}
