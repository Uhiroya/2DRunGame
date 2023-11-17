using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;

public class GamePresenter : MonoBehaviour
{
    [SerializeField] float _scoreRatePerSecond = 30f;
    [SerializeField] float _speedUpRate = 0.01f;
    [SerializeField] GameModel _model;
    [SerializeField] GameView _view;
    [SerializeField] PlayerPresenter _playerPresenter;
    [SerializeField] ObstacleGenerator _obstaclePresenter;
    public static float MapXMargin = Screen.width * 0.15f;
    private void Awake()
    {
        MapXMargin = Screen.width * 0.15f;
        Bind();
        Inisalize();
    }
    /// <summary>初期化を行う</summary>
    private void Inisalize()
    {
        _model.GameState.Value = GameFlowState.Inisialize;
    }

    /// <summary>
    /// バインド
    /// </summary>
    private void Bind()
    {
        _model.GameSpeed
            .Subscribe(
                x =>
                {
                    _view.SetUVSpeed(x);
                    _playerPresenter.SetSpeedRate(x);
                }
                )
            .AddTo(this);

        _model.Score
            .Subscribe(
                x => 
                    _view.SetScore(x)
                )
            .AddTo(this);

        _model.GameState
            .Subscribe(
                x => 
                {
                    switch (x)
                    {
                        case GameFlowState.InGame:
                            _playerPresenter.GameStart();
                            break;
                        case GameFlowState.Result:
                            _obstaclePresenter.Reset();
                            _view.ShowResultUI();
                            break;
                        default: 
                            break;
                    } 
                })
            .AddTo(this);

        //_obstaclePresenter.IsHit.Where(x => x == true)
        //    .Subscribe(
        //        x => 
        //        {
        //            _playerPresenter.GameOver(); 
        //        })
        //    .AddTo(this);
    }
    private void Update()
    {
        if ( _playerPresenter.PlayerState.Value == PlayerCondition.Alive)
        {
            _view.ManualUpdate(Time.deltaTime);
            _model.AddScore(_scoreRatePerSecond * Time.deltaTime);
            _model.AddSpeed(_speedUpRate * Time.deltaTime);
            _obstaclePresenter.ManualUpdate(Time.deltaTime, _model.GameSpeed.Value);
        }
    }
    public void AddScore(float score)
    {
        _model.AddScore(score);
    }
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ShowResultScore()
        => _view.ShowResultScore(_model.Score.Value);
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToTitle()
        => _model.GameState.Value = GameFlowState.Title;
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToInGame()
        => _model.GameState.Value = GameFlowState.InGame;
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToResult()
        => _model.GameState.Value = GameFlowState.Result;
}
