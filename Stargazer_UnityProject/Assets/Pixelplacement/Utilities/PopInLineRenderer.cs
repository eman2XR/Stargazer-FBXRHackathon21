using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PopInLineRenderer : MonoBehaviour
{
    //Public Variables:
    public float duration = .35f;
    public float delay;
    
    //Private Variables:
    private LineRenderer _lineRenderer;
    private float _initialStartWidth;
    private float _initialEndWidth;
    
    //Startup:
    private void Awake()
    {
        //refs:
        _lineRenderer = GetComponent<LineRenderer>();
        
        //sets:
        _initialStartWidth = _lineRenderer.startWidth;
        _initialEndWidth = _lineRenderer.endWidth;
    }

    private void OnEnable()
    {
        Tween.Value(0f, 1f, UpdateLineRenderer, duration, delay, Tween.EaseOutBack);
    }

    //Callbacks:
    private void UpdateLineRenderer(float value)
    {
        _lineRenderer.startWidth = _initialStartWidth * value;
        _lineRenderer.endWidth = _initialEndWidth * value;
    }
}
