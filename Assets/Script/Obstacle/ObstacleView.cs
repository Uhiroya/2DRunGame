using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IObstacleView
{
    void Idle();
    void OnRight();
    void OnLeft();
}
/// <summary>
/// Animatorは全ての障害物に持たせるようにする。！！
/// Viewには勝手に判断させない。
/// </summary>
public class ObstacleView :IObstacleView
{
    Animator _animator;
    bool _hasIdleState = false;
    bool _hasRightState = false;
    bool _hasLeftState = false;
    static readonly int _hashIdle = Animator.StringToHash("Idle");
    static readonly int _hashRight = Animator.StringToHash("Right");
    static readonly int _hashLeft = Animator.StringToHash("Left");
    public ObstacleView(Animator? animator = null)
    {
        _animator = animator;
        if (_animator == null) return;
        _hasIdleState = _animator.HasState(0, _hashIdle);
        _hasRightState = animator.HasState(0, _hashRight);
        _hasLeftState = animator.HasState(0, _hashLeft);
        Idle();
    }
    public void Idle()
    {
        if (_animator == null || _hasIdleState ) return;
    }
    public void OnRight()
    {
        if (_animator == null || _hasRightState) return;
    }
    public void OnLeft()
    {
        if (_animator == null || _hasLeftState) return;
    }

}
