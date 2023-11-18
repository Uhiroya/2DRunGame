using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public interface IObstaclePresenter
{

}
public class ObstaclePresenter 
{
    Collider2D _col;
    ObstacleModel _model;
    public ObstacleType ObstacleType => _model.ItemType;
    public ObstaclePresenter(ObstacleModel model)
    {
        _model = model;
    }
    //public readonly ReactiveProperty<ObstacleType> IsHit = new(ObstacleType.None) ;
    public readonly ReactiveProperty<Vector2> Position = new();
    public void Create(Collider2D col ,Transform transform)
    {
        //IsHit.Value = ObstacleType.None;
        _model.PositionX
            .Subscribe(x => { Position.Value = new Vector2(x, Position.Value.y); });
        _model.PositionY
            .Subscribe(y => { Position.Value = new Vector2(Position.Value.x , y);});
        _col.OnTriggerEnter2DAsObservable()
            .Where(col => col.tag == "Player")
            .Subscribe(col => Hit(col));
        _col = col;
        _model.Create(transform);
    }

    public void SetObstacle(float posX, float posY)
    {
        _model.Set(posX, posY);
    }

    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _model.Move(deltaTime, speed);
    }
    public void Hit(Collider2D player)
    {
        //IsHit.Value = _model.ItemType;
        Debug.Log("aaaa");
    }
    public float GetScore()
    {
        return _model.Score;
    }

    public void OnDestroy()
    {
        //IsHit.Dispose();
    }


}
