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
    public ObstacleView(Animator animator = null)
    {
        _animator = animator;
        
    }
    public void Idle()
    {
        
    }
    public void OnRight()
    {
        
    }
    public void OnLeft()
    {
        
    }

}
