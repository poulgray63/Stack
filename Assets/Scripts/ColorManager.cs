using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    [SerializeField] private Material _background;

    private Color[] _current = new Color[2];
    private Color[] _lerpStartColor = new Color[2];
    private Color[] _target = new Color[2];
    private Color[] _secondaryTarget = new Color[2];

    [SerializeField] [Range(0.1f,5f)] private float _lerpDuration;
    private float _lerpTime = 0f;
    private bool _canLerp = false;

    private int _propertyIdTop;
    private int _propertyIdBottom;

    public static Color GetRandomColor()
    {
        Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        return color;
    }

    public static Color GetAverageColor(Color a, Color b)
    {
        return new Color((a.r + b.r) / 2, (a.g + b.g) / 2, (a.b + b.b) / 2);
    }

    public static Color ColorMoveTowards(Color from, Color to, float delta)
    {
        return new Color
            (
            Mathf.MoveTowards(from.r, to.r, delta),
            Mathf.MoveTowards(from.g, to.g, delta),
            Mathf.MoveTowards(from.b, to.b, delta)
            );
    }

    public void Lerp() 
    {
        _lerpStartColor[0] = _current[0];
        _lerpStartColor[1] = _current[1];
        _canLerp = true;
    }

    public void OnRestart() 
    {
        SetNewTarget();
        _secondaryTarget[0] = _target[0];
        _secondaryTarget[1] = _target[1];
        Lerp();
    }

    public Color GetLayerColor() 
    {
        return GetAverageColor(_secondaryTarget[0], _secondaryTarget[1]);
    }

    private void Awake()
    {
        instance = this;
        _propertyIdTop = Shader.PropertyToID("_Top");
        _propertyIdBottom = Shader.PropertyToID("_Bottom");

        ChangeBackground(GetRandomColor(), GetRandomColor());
        SetNewTarget();
    }

    private void Update()
    {
        if (_canLerp)
        {
            ChangeBackground(Color.Lerp(_lerpStartColor[0], _secondaryTarget[0], _lerpTime), Color.Lerp(_lerpStartColor[1], _secondaryTarget[1], _lerpTime));
            if (_lerpTime >= 1f)
            {
                _canLerp = false;
                _lerpTime = 0f;
                SetNewSecondaryTarget();
                if (_current[0] == _target[0] && _current[1] == _target[1])
                {
                    SetNewTarget();
                }
            }
            else 
            {
                _lerpTime += Time.deltaTime / _lerpDuration;
            }
        }
    }

    private void ChangeBackground(Color top, Color bottom) 
    {
        _current[0] = top;
        _current[1] = bottom;
        _background.SetColor(_propertyIdTop, top);
        _background.SetColor(_propertyIdBottom, bottom);
    }

    private void SetNewTarget() 
    {
        _target[0] = GetRandomColor();
        _target[1] = GetRandomColor();
        SetNewSecondaryTarget();
    }

    private void  SetNewSecondaryTarget() 
    {
        _secondaryTarget[0] = ColorMoveTowards(_current[0], _target[0], 0.1f);
        _secondaryTarget[1] = ColorMoveTowards(_current[1], _target[1], 0.1f);
    }
}

public static class ColorExtension
{
    public static Color GetInvertedColor(this Color color)
    {
        return new Color(1.0f - color.r, 1.0f - color.g, 1.0f - color.b);
    }
}
