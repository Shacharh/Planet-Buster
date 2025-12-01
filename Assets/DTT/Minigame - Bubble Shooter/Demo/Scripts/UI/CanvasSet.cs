using System;
using System.Collections;
using System.Collections.Generic;
using DTT.BubbleShooter.Demo;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Set the canvas component size so there is no blank spot on the side of the game area.
/// </summary>
public class CanvasSet : MonoBehaviour
{
    /// <summary>
    /// Bottom part of the canvas.
    /// </summary>
    [SerializeField] 
    [Tooltip("bottom component")]
    private RectTransform _bottom;
    
    /// <summary>
    /// Right part of the canvas.
    /// </summary>
    [SerializeField] 
    [Tooltip("right component")]
    private RectTransform _right;
    
    /// <summary>
    /// Left part of the canvas.
    /// </summary>
    [SerializeField] 
    [Tooltip("left component")]
    private RectTransform _left;

    /// <summary>
    /// Canvas that display the design element.
    /// </summary>
    private Canvas _canvas;
    
    /// <summary>
    /// layout element for the middle part of the background.
    /// </summary>
    private LayoutElement _layoutMiddle;
    
    /// <summary>
    /// initialize the component
    /// </summary>
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _layoutMiddle = _bottom.gameObject.GetComponent<LayoutElement>();
    }
    
    /// <summary>
    /// Update the component size according to the width of the game.
    /// </summary>
    /// <param name="width"> Width of the bubble game.</param>
    public float? UpdateCanvas(float width)
    {
        Vector2 uiOffset = new Vector2(_canvas.pixelRect.size.x / 2f, _canvas.pixelRect.size.y / 2f);
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(new Vector3(width*1.2f,0,0));
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * _canvas.pixelRect.width, ViewportPosition.y * _canvas.pixelRect.height);
        float bottomSize = (proportionalPosition.x - uiOffset.x);
        if (Screen.height > Screen.width)
        {
            if(bottomSize> _canvas.pixelRect.width) 
                return GetCanvasSIze();
            return null;
        }

        // If the size of the bottom part is greater than the size of the canvas, don't change it.
        if (bottomSize> _canvas.pixelRect.width)
        {
            _right.gameObject.SetActive(false);
            _left.gameObject.SetActive(false);
            _bottom.sizeDelta = new Vector2(Screen.width, _bottom.rect.height);
            return GetCanvasSIze();
        }

        //_bottom.sizeDelta = new Vector2(bottomSize, _bottom.sizeDelta.y);
        _layoutMiddle.minWidth = bottomSize;
        return null;
    }
    
    /// <summary>
    /// Return the size of the canvas.
    /// </summary>
    /// <returns>Size of the canvas.</returns>
    private float GetCanvasSIze() => Camera.main.orthographicSize * Camera.main.aspect * 1.5f;
    
}
