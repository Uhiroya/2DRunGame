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
        _model.Set(posX, posY);
    }

    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _model.Move(deltaTime, speed);
    }
    public void Hit(Collider2D player)
    {
        //インターフェースでの実装に差し替える。
        switch (_model.ItemType)
        {
            case ObstacleType.Item:
                FindObjectOfType<GamePresenter>()?.AddScore(_model.Score);
                break;
            case ObstacleType.Enemy:
                player.GetComponent<PlayerPresenter>()?.GameOver();
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
