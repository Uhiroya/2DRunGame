using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IPlayerView
{
    void OnWaiting();
    void OnWalk();
    void OnDead();
}
public class PlayerView :IPlayerView
{
    Animator _animator;
    public PlayerView(Animator animator)
    {
        _animator = animator;
    }
    public void OnWaiting()
        => _animator.SetTrigger("Waiting");
    public void OnWalk()
        => _animator.SetTrigger("Walking");
    public void OnDead()
        => _animator.SetTrigger("Dead");
}
