using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityVolumeRendering;

public class UIDraggableColor : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    private GameObject colorView;
    
    // Set in TransferFunctionPanel.cs
    public TFColourControlPoint controlPoint;
    public TransferFunction transferFunction;
    public int index;

    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        colorView = GameObject.Find("TransferFuncColor");
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
        
        float maxHeight = colorView.GetComponent<RectTransform>().rect.height;
        float maxWidth = colorView.GetComponent<RectTransform>().rect.width;

        float myMaxWidth = GetComponent<RectTransform>().rect.width;
        float myMaxHeight = GetComponent<RectTransform>().rect.height;

        if (rectTransform.anchoredPosition.x < 0)
        {
            rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
        }
        if (rectTransform.anchoredPosition.y < 0)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
        }
        if (rectTransform.anchoredPosition.y > maxHeight - myMaxHeight)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, maxHeight - myMaxHeight);
        }
        if (rectTransform.anchoredPosition.x > maxWidth - myMaxWidth)
        {
            rectTransform.anchoredPosition = new Vector2(maxWidth - myMaxWidth, rectTransform.anchoredPosition.y);
        }
        
        controlPoint.dataValue = rectTransform.anchoredPosition.x / (maxWidth- myMaxWidth);
        controlPoint.dataValue = rectTransform.anchoredPosition.x / (maxWidth- myMaxWidth);
        
        transferFunction.colourControlPoints[index] = controlPoint;
        
        transferFunction.GenerateTexture();
    }
}
