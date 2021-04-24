using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Layer : MonoBehaviour
{
    [HideInInspector] public new Renderer renderer;

    [SerializeField] private bool _canMove;
    private float _speed;
    private bool _movingForward = true;
    private bool _movingX = true;
    private Vector3 _moveStartPosition;
    private Vector3 _moveTargetPosition;

    private bool _expanded = false;
    private bool _canScale;
    private Vector3 _expandTargetPosition = Vector3.zero;
    private Vector3 _expandTargetScale = Vector3.zero;

    public Vector3 MoveStartPosition
    {
        get { return _moveStartPosition; }
        set { _moveStartPosition = value; transform.position = value; }
    }
    public bool MovingX 
    {
        get { return _movingX; }
        set { _movingX = value; OnAxisChanged(value); }
    }
    public bool Expanded { get { return _expanded; } }
    public Vector3 MoveTargetPosition { get { return _moveTargetPosition; } }
    public Vector3 ExpandTargetPosition { get { return _expandTargetPosition; } }
    public Vector3 ExpandTargetScale { get { return _expandTargetScale; } }

    public void Stop()
    {
        _canMove = false;
    }

    public void Expand()
    {
        _expandTargetPosition = Vector3.MoveTowards(transform.position, new Vector3(0f, transform.position.y, 0f), 0.5f);
        _expandTargetScale = new Vector3(Mathf.Clamp(transform.localScale.x + 1f, 0.0f, 5.0f), 1f, Mathf.Clamp(transform.localScale.z + 1f, 0.0f, 5.0f));
        _canScale = true;
        _expanded = true;
    }

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        if (_canMove)
        {
            _speed = LayerManager.instance.GetSpeed();
        }
    }

    private void OnAxisChanged(bool xAxis)
    {
        switch (xAxis) 
        {
            case true:
                _moveTargetPosition = _moveStartPosition + new Vector3(LayerManager.instance.SpawnDistance*2, 0f, 0f);
                break;
            case false:
                _moveTargetPosition = _moveStartPosition + new Vector3(0f, 0f, -LayerManager.instance.SpawnDistance*2);
                break;
        }
    }

    private void Update()
    {
        if (_canScale) 
        {
            transform.position = Vector3.MoveTowards(transform.position, _expandTargetPosition, LayerManager.instance.ScaleSpeed * Time.deltaTime);
            transform.localScale = Vector3.MoveTowards(transform.localScale, _expandTargetScale, LayerManager.instance.ScaleSpeed * Time.deltaTime);
            if (transform.position == _expandTargetPosition && transform.localScale == _expandTargetScale) 
            {
                _canScale = false;
            }
        }
        if (_canMove)
        {
            if (MovingX)
            {
                if (_movingForward)
                {
                    if (transform.position != _moveTargetPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _moveTargetPosition, _speed * Time.deltaTime);
                    }
                    else _movingForward = !_movingForward;
                }
                else
                {
                    if (transform.position != _moveStartPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _moveStartPosition, _speed * Time.deltaTime);
                    }
                    else _movingForward = !_movingForward;
                }
            }
            else 
            {
                if (_movingForward)
                {
                    if (transform.position != _moveTargetPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _moveTargetPosition, _speed * Time.deltaTime);
                    }
                    else _movingForward = !_movingForward;
                }
                else 
                {
                    if (transform.position != _moveStartPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _moveStartPosition, _speed * Time.deltaTime);
                    }
                    else _movingForward = !_movingForward;
                }
            }
        }
    }
}
