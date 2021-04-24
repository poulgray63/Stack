using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    [SerializeField] private Fragment _fragmentPrefab;
    [SerializeField] private Layer _layerPrefab;
    [SerializeField] private Layer _foundation;
    [SerializeField] private Layer _lastLayer;
    private Layer _currentLayer;

    [SerializeField] [Range(1f, 10f)] private float _speed;
    [SerializeField] [Range(10f,200f)] private int _speedCurveStepCount;
    [SerializeField] private AnimationCurve _speedCurve;
    private float _speedCurveStepDelta;
    private int _currentSpeedCurveStep = 0;

    [SerializeField] [Range(0.1f, 1f)] private float _successThreshold;
    [SerializeField] [Range(5f, 10f)]  private float _spawnDistance;
    [SerializeField] [Range(1f, 10f)] private float _scaleSpeed;

    public List<Layer> layerList = new List<Layer>();
    public List<Fragment> fragmentList = new List<Fragment>();

    public Layer LastLayer { get { return _lastLayer; } }
    public Layer CurrentLayer { get { return _currentLayer; } }
    public float SpawnDistance { get { return _spawnDistance; } }
    public float Speed { get { return _speed; } }
    public float ScaleSpeed { get { return _scaleSpeed; } }

    public enum StackResult
    {
        PERFECT,
        SUCCESS,
        FAILURE
    }

    public int CurrentSpeedCurveStep 
    {
        set 
        {
            if (_currentSpeedCurveStep >= _speedCurveStepCount) 
            {
                _currentSpeedCurveStep = 0;
            }else _currentSpeedCurveStep = value;
        } 
        get { return _currentSpeedCurveStep; } 
    }

    public float GetSpeed()
    {
        CurrentSpeedCurveStep++;
        return _speed * _speedCurve.Evaluate(CurrentSpeedCurveStep * _speedCurveStepDelta);
    }

    public void OnStartGame() 
    {
        _currentLayer = _foundation;
    }

    public void OnRestartGame() 
    {
        _currentSpeedCurveStep = 0;
    }

    public void DestroyGarbage() 
    {
        foreach (Layer a in layerList)
        {
            if (a)
            {
                Destroy(a.gameObject);
            }
        }
        foreach (Fragment a in fragmentList)
        {
            if (a)
            {
                Destroy(a.gameObject);
            }
        }
        layerList.Clear();
        fragmentList.Clear();
    }

    public void CreateLayer()
    {
        _lastLayer = _currentLayer;

        Vector3 position = _lastLayer.transform.position + new Vector3(0f, 1f, 0f);
        Vector3 scale = _lastLayer.transform.localScale;

        if (_lastLayer.Expanded)
        {
            position = _lastLayer.ExpandTargetPosition + new Vector3(0f, 1f, 0f);
            scale = _lastLayer.ExpandTargetScale;
        }
        _currentLayer = Instantiate(_layerPrefab, position, _layerPrefab.transform.rotation);
        _currentLayer.renderer.material.color = ColorManager.instance.GetLayerColor();
        _currentLayer.transform.localScale = scale;
        if (GameManager.instance.Score % 2 == 0)
        {
            _currentLayer.MoveStartPosition = new Vector3
                (
                _currentLayer.transform.position.x - _spawnDistance, 
                _currentLayer.transform.position.y, 
                _currentLayer.transform.position.z
                );
            _currentLayer.MovingX = true;
        }
        else
        {
            _currentLayer.MoveStartPosition = new Vector3
                (
                _currentLayer.transform.position.x, 
                _currentLayer.transform.position.y, 
                _currentLayer.transform.position.z + _spawnDistance
                );
            _currentLayer.MovingX = false;
        }
        layerList.Add(_currentLayer);

    }

    public StackResult TryStack()
    {
        float temp;
        float scale;
        float distance = Vector2.Distance
            (
            new Vector2(_lastLayer.transform.position.x, _lastLayer.transform.position.z),
            new Vector2(_currentLayer.transform.position.x, _currentLayer.transform.position.z)
            );
        _currentLayer.Stop();
        if (distance <= _successThreshold)
        {
            _currentLayer.transform.position = new Vector3
                (
                _lastLayer.transform.position.x,
                _currentLayer.transform.position.y,
                _lastLayer.transform.position.z
                );
            return StackResult.PERFECT;
        }
        else
        {
            if (_currentLayer.MovingX)
            {
                temp = _currentLayer.transform.position.x;
                scale = Mathf.Clamp(_lastLayer.transform.localScale.x - distance, 0.0f, 5f);
                if (scale > 0.0f)
                {
                    _currentLayer.transform.localScale = new Vector3
                        (
                        scale,
                        _currentLayer.transform.localScale.y,
                        _currentLayer.transform.localScale.z
                        );

                    _currentLayer.transform.position = new Vector3
                        (
                        (_currentLayer.transform.position.x + _lastLayer.transform.position.x) / 2,
                        _currentLayer.transform.position.y,
                        _currentLayer.transform.position.z
                        );
                    CreateFragment(new Vector3
                        (
                        temp - (_currentLayer.transform.localScale.x / 2 * Mathf.Sign(_lastLayer.transform.position.x - _currentLayer.transform.position.x)),
                        _currentLayer.transform.position.y,
                        _currentLayer.transform.position.z
                        ), new Vector3
                        (
                        Mathf.Abs(_lastLayer.transform.localScale.x - _currentLayer.transform.localScale.x),
                        _currentLayer.transform.localScale.y,
                        _currentLayer.transform.localScale.z
                        ));
                    return StackResult.SUCCESS;

                }
                else return StackResult.FAILURE;
            }
            else
            {
                temp = _currentLayer.transform.position.z;
                scale = Mathf.Clamp(_lastLayer.transform.localScale.z - distance, 0.0f, 5f);
                if (scale > 0.0f)
                {
                    _currentLayer.transform.localScale = new Vector3
                        (
                        _currentLayer.transform.localScale.x,
                        _currentLayer.transform.localScale.y,
                        scale
                        );

                    _currentLayer.transform.position = new Vector3
                        (
                        _currentLayer.transform.position.x,
                        _currentLayer.transform.position.y,
                        (_currentLayer.transform.position.z + _lastLayer.transform.position.z) / 2
                        );
                    CreateFragment(new Vector3
                        (
                        _currentLayer.transform.position.x,
                        _currentLayer.transform.position.y,
                        temp + (_currentLayer.transform.localScale.z / 2 * Mathf.Sign(_currentLayer.transform.position.z - _lastLayer.transform.position.z))
                        ), new Vector3
                        (
                        _currentLayer.transform.localScale.x,
                       _currentLayer.transform.localScale.y,
                        Mathf.Abs(LastLayer.transform.localScale.z - _currentLayer.transform.localScale.z)
                        ));
                    return StackResult.SUCCESS;
                }
                else return StackResult.FAILURE;
            }
        }
    }

    private void Awake()
    {
        instance = this;
        _speedCurveStepDelta = 1f / _speedCurveStepCount;
    }

    private void CreateFragment(Vector3 position, Vector3 scale) 
    {
        Fragment fragment = Instantiate(_fragmentPrefab, position, _fragmentPrefab.transform.rotation);
        fragment.transform.localScale = scale;
        fragment.renderer.material.color = _currentLayer.renderer.material.color;
        fragmentList.Add(fragment);
    }
}
