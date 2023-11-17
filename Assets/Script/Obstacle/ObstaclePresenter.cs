using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ObstaclePresenter : MonoBehaviour
{
    [SerializeField] GameObject _explosionEffect;
    [SerializeField] ObstacleModel _model;

    public readonly ReactiveProperty<bool> IsHit = new(false) ;
    public void OnEnable()
    {
        IsHit.Value = false ;
    }
    public void SetObstacle(float posX, float posY)
    {
        //print($"{posX} {posY}");
        _model.SetX(posX);
        _model.SetY(posY);
    }

    public void ManualUpdate(float deltaTime, float speed)
    {
        _model.MoveX(deltaTime , speed);
        _model.MoveY(deltaTime , speed);
    }
    public void Hit(Collider2D player)
    {
        //インターフェースでの実装に差し替える。
        switch (_model.ItemType)
        {
            case ObstacleType.Item:
                FindObjectOfType<GamePresenter>().AddScore(_model.Score);
                break;
            case ObstacleType.Enemy:
                player.GetComponent<PlayerPresenter>().GameOver();
                Instantiate(_explosionEffect, transform.position, Quaternion.identity, this.transform.parent.transform);
                break;
            default: 
                break;
        }
        IsHit.Value = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Hit(other);

        }
    }

    public void OnDestroy()
    {
        IsHit.Dispose();
    }
}
