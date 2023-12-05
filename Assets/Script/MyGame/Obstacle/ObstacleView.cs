using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IObstacleView
{
    void SetAnimator(Animator animator);
    void SetXMovement(float xMovement);
}
/// <summary>
/// Animatorは全ての障害物に持たせるようにする。！！
/// Viewには勝手に判断させない。
/// </summary>
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

}
