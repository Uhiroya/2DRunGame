using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IPlayerView
{
    public void OnWaiting();
    public void OnWalk();
    public void OnDead();

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
