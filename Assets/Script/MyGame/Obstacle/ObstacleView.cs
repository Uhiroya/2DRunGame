using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IObstacleView
{
    void SetAnimator(Animator animator);
    void SetXMovement(float xMovement);
    void Pause();
    void Resume();
}
public class ObstacleView :IObstacleView
{
    Animator _animator;
    public ObstacleView()
    {
    }
    public void SetAnimator(Animator animator)
    {
        _animator = animator;
    }
    public void SetXMovement(float xMovement)
    {
        if (_animator == null) return;
        _animator.SetFloat("XMovement", xMovement);
    }
    public void Pause()
    {
        if (_animator == null) return;
        _animator.speed = 0f;
    }

    public void Resume()
    {
        if (_animator == null) return;
        _animator.speed = 1f;
    }
}
