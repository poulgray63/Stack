using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Animator))]
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] [Range(0.1f, 10f)] private float _speedIncreaseHeight;
    private float _speed;
    private bool _canMove = false;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private Animator animator;

    public void IncreaseHeight() 
    {
        Move(transform.position + Vector3.up, _speedIncreaseHeight);
    }

    public void OnFailure()
    {
        animator.SetTrigger("ZoomOut");
    }

    public void OnRestartGame() 
    {
        animator.SetTrigger("Restart");
    }
    public void OnStartRestartAnimationEvent() 
    {
        GameManager.instance.canStartNewGame = false;
    }
    public void OnMiddleRestartAnimationEvent() 
    {
        LayerManager.instance.DestroyGarbage();
        ColorManager.instance.OnRestart();
        GameManager.instance.canStartNewGame = true;
        MoveToStartPosition();
    }

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        _startPosition = transform.position;
        _targetPosition = transform.position;
    }

    private void MoveToStartPosition() 
    {
        transform.position = _startPosition;
    }

    private void Move(Vector3 targetPosition, float speed)
    {
        _targetPosition = targetPosition;
        _speed = speed;
        _canMove = true;
    }

    private void Update()
    {
        if (_canMove) 
        {
            if (transform.position == _targetPosition)
            {
                _canMove = false;
            }
            else 
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
            }
        }
    }
}
