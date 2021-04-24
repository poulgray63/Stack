using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool canStartNewGame = true;
    private Statement _currentStatement = Statement.Start;

    [SerializeField] [Range(2, 10)] private int _successThreshold;
    private int _score = 0;
    private int _highScore = 0;
    private int _inRow = 0;

    public enum Statement
    {
        Start,
        Game,
        Failure
    }

    public int Score 
    {
        get { return _score; }
        private set { _score = value; OnScoreChanged(); }
    }

    public int InRow 
    {
        get { return _inRow; }
        set { _inRow = value; OnInRowChanged(); }
    }

    public int Highscore 
    {
        get { return _highScore; }
        set { _highScore = value; OnHighscoreChanged(); }
    }

    public void StartGame() 
    {
        if (canStartNewGame)
        {
            _currentStatement = Statement.Game;
            Score = 0;
            LayerManager.instance.OnStartGame();
            LayerManager.instance.CreateLayer();
            UIManager.instance.OnStartGame();
        }
    }

    public void RestartGame() 
    {
        _currentStatement = Statement.Start;
        UIManager.instance.OnRestartGame();
        CameraManager.instance.OnRestartGame();
        LayerManager.instance.OnRestartGame();
    }

    public void Tap() 
    {
        switch (_currentStatement)
        {
            case Statement.Game:
                Stack();
                break;
            case Statement.Failure:
                RestartGame();
                break;
        }
    }

    private void Awake()
    {
        instance = this;
        Highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    private void Stack() 
    {
        ColorManager.instance.Lerp();
        switch (LayerManager.instance.TryStack())
        {
            case LayerManager.StackResult.PERFECT:
                Score++;
                InRow++;
                LineEffect.instance.OnSuccess();
                SoundManager.instance.OnSuccess();
                LayerManager.instance.CreateLayer();
                CameraManager.instance.IncreaseHeight();
                break;
            case LayerManager.StackResult.SUCCESS:
                Score++;
                InRow = 0;
                SoundManager.instance.ResetPitch();
                LayerManager.instance.CreateLayer();
                CameraManager.instance.IncreaseHeight();
                break;
            case LayerManager.StackResult.FAILURE:
                LayerManager.instance.CurrentLayer.GetComponent<Rigidbody>().isKinematic = false;
                SoundManager.instance.ResetPitch();
                if (_score > _highScore)
                {
                    UIManager.instance.textHighscore.gameObject.SetActive(false);
                    UIManager.instance.textNewRecord.SetActive(true);
                    UIManager.instance.iconHighscore.SetActive(false);

                    Highscore = _score;
                }
                else
                {
                    UIManager.instance.textHighscore.gameObject.SetActive(true);
                    UIManager.instance.textNewRecord.SetActive(false);
                    UIManager.instance.iconHighscore.SetActive(true);
                }
                UIManager.instance.OnFailure();
                CameraManager.instance.OnFailure();
                _currentStatement = Statement.Failure;
                break;
        }
    }

    private void OnScoreChanged() 
    {
        if (_score == 1) 
        {
            UIManager.instance.animatorTextScore.SetTrigger("Show");
        }
        UIManager.instance.textScore.text = _score.ToString();
    }

    private void OnInRowChanged() 
    {
        if (_inRow >= _successThreshold) 
        {
            LayerManager.instance.CurrentLayer.Expand();
        }
    }

    private void OnHighscoreChanged()
    {
        PlayerPrefs.SetInt("Highscore", _highScore);
        UIManager.instance.textHighscore.text = _highScore.ToString();
    }
}
