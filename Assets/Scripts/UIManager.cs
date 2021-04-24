using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Animator animatorScreenStart;
    public Animator animatorScreenGame;
    public CanvasGroup screenGame;

    public Text textScore;
    public Animator animatorTextScore;

    public Animator animatorHighscoreGroup;
    public Text textHighscore;
    public GameObject textNewRecord;
    public GameObject iconHighscore;

    private int _propertyIdShow;
    private int _propertyIdHide;

    public void OnStartGame() 
    {
        animatorScreenStart.SetTrigger(_propertyIdHide);
        animatorScreenGame.SetTrigger(_propertyIdShow);
    }

    public void OnRestartGame() 
    {
        animatorTextScore.SetTrigger(_propertyIdHide);
        animatorHighscoreGroup.SetTrigger(_propertyIdHide);
        animatorScreenGame.SetTrigger(_propertyIdHide);
        animatorScreenStart.SetTrigger(_propertyIdShow);
    }

    public void OnFailure() 
    {
        animatorHighscoreGroup.SetTrigger(_propertyIdShow);
        if (GameManager.instance.Score < 1) 
        {
            animatorTextScore.SetTrigger(_propertyIdShow);
        }
    }

    private void Awake()
    {
        instance = this;
        _propertyIdShow = Animator.StringToHash("Show");
        _propertyIdHide = Animator.StringToHash("Hide");
    }
}
