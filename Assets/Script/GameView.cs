using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

public interface IGameView
{
    UniTask TitleStart();
    public void ResetTitleUI();
    void ManualUpdate(float deltaTime);

    void SetUVSpeed(float speed);

    void SetScore(float currentScore);

    void ShowResultUI();

    UniTaskVoid ShowResultScore(float score);
}
public class GameView : IGameView
{
    float _titleAnimationTime;
    GameObject _titleObject;
    GameObject _titleTapObject;
    Vector3 _titlePosition;
    IBackGroundController _background;
    Text _scoreText;
    GameObject _resultUIGroup;
    Text _resultScoreText;
    float _resultAnimationTime;
    float _countUpTime;
   
    public GameView(float titleAnimationTime, GameObject titleObject , GameObject titleTapObject ,
        IBackGroundController background , Text scoreText , GameObject resultUIGroup
        ,Text resultScoreText, float resultAnimationTime ,float countUpTime)
    {
        _titleAnimationTime = titleAnimationTime;
        _titleObject = titleObject;
        _titleTapObject = titleTapObject;
        _background = background;
        _scoreText = scoreText; 
        _resultUIGroup = resultUIGroup;
        _resultScoreText = resultScoreText;
        _resultAnimationTime = resultAnimationTime;
        _countUpTime = countUpTime;
        _titlePosition = _titleObject.transform.position;
    }
    public async UniTask TitleStart()
    {
        _titleObject.SetActive(true);
        await UniTask.Delay((int)(_titleAnimationTime * 1000));
        _titleTapObject.SetActive(true);
    }
    public void ResetTitleUI()
    {
        _titleObject.SetActive(false);
        _titleObject.transform.position = _titlePosition;
        _titleTapObject.SetActive(false);
    }
    public void ManualUpdate(float deltaTime)
    {
        _background.ManualUpdate(deltaTime);
    }
    public void SetUVSpeed(float speed)
    {
        _background.SetUVSpeed(speed);
    }
    public void SetScore(float currentScore)
    {
        _scoreText.text = currentScore.ToString("00000000");
    }
    public void ShowResultUI()
    {
        _resultUIGroup.SetActive(true);
    }
    public async UniTaskVoid ShowResultScore(float score)
    {
        await UniTask.Delay((int)(_resultAnimationTime * 1000));
        //カウントアップ処理
        await DOVirtual.Float(
            0f,
            score,
            _countUpTime,
            value => _resultScoreText.text = value.ToString("00000000")
        );
    }
}
