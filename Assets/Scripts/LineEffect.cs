using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Animator))]
public class LineEffect : MonoBehaviour
{
    public static LineEffect instance;

    private LineRenderer _lineRenderer;

    private Animator _animator;

    private int _animationHashSuccess;

    public void OnSuccess() 
    {
        Vector3 position;
        Vector3 scale;
        if (LayerManager.instance.CurrentLayer.Expanded)
        {
            position = LayerManager.instance.CurrentLayer.ExpandTargetPosition;
            scale = LayerManager.instance.CurrentLayer.ExpandTargetScale;
        }
        else
        {
            position = LayerManager.instance.CurrentLayer.transform.position;
            scale = LayerManager.instance.CurrentLayer.transform.localScale;
        }

        transform.position = new Vector3(position.x, position.y - 0.5f, position.z);
        _lineRenderer.SetPosition(0, new Vector3(-scale.x / 2 - _lineRenderer.widthCurve[0].value / 4, scale.z / 2 + _lineRenderer.widthCurve[0].value / 4, 0f));
        _lineRenderer.SetPosition(1, new Vector3(scale.x / 2 + _lineRenderer.widthCurve[0].value / 4, scale.z / 2 + _lineRenderer.widthCurve[0].value / 4, 0f));
        _lineRenderer.SetPosition(2, new Vector3(scale.x / 2 + _lineRenderer.widthCurve[0].value / 4, -scale.z / 2 - _lineRenderer.widthCurve[0].value / 4, 0f));
        _lineRenderer.SetPosition(3, new Vector3(-scale.x / 2 - _lineRenderer.widthCurve[0].value / 4, -scale.z / 2 - _lineRenderer.widthCurve[0].value / 4, 0f));
        _animator.SetTrigger(_animationHashSuccess);
    }

    private void Awake()
    {
        instance = this;
        _lineRenderer = GetComponent<LineRenderer>();
        _animator = GetComponent<Animator>();
        _animationHashSuccess = Animator.StringToHash("Success");
    }
}
