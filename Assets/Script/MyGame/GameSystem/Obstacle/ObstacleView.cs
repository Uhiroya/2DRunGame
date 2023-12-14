using UnityEngine;

public interface IObstacleView
{
    void SetTheta(float theta);
    void Pause();
    void Resume();
}

public class ObstacleView : IObstacleView
{
    private readonly Animator _animator;
    private static readonly int XMovement = Animator.StringToHash("XMovement");

    public ObstacleView(Animator animator)
    {
        _animator = animator;
    }

    public void SetTheta(float theta)
    {
        if (_animator == null) return;
        _animator.SetFloat(XMovement, Mathf.Cos(theta + Mathf.PI / 2));
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
