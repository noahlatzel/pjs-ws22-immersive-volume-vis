using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityVolumeRendering;

public class UIDraggableAlpha : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    private GameObject alphaView;
    
    // Set in TransferFunctionPanel.cs
    public TFAlphaControlPoint controlPoint;
    public TransferFunction transferFunction;
    public int index;
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        alphaView = GameObject.Find("TransferFuncAlpha");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
        
        float maxHeight = alphaView.GetComponent<RectTransform>().rect.height;
        float maxWidth = alphaView.GetComponent<RectTransform>().rect.width;

        float myMaxWidth = GetComponent<RectTransform>().rect.width;
        float myMaxHeight = GetComponent<RectTransform>().rect.height;

        if (rectTransform.anchoredPosition.x < 0)
        {
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        if (rectTransform.anchoredPosition.y != 0)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
        }
        if (rectTransform.anchoredPosition.x > maxWidth - myMaxWidth)
        {
            rectTransform.anchoredPosition = new Vector2(maxWidth - myMaxWidth, 0);
        }

        controlPoint.alphaValue = rectTransform.anchoredPosition.y / (maxHeight - myMaxHeight);
        controlPoint.dataValue = rectTransform.anchoredPosition.x / (maxWidth- myMaxWidth);
        transferFunction.alphaControlPoints[index] = controlPoint;
        transferFunction.GenerateTexture();
    }
}
