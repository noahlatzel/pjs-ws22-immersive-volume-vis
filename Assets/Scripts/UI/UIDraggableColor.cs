using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityVolumeRendering;

public class UIDraggableColor : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private GameObject colorView;
    private TransferFunctionPanel transferFuncManager;
    
    // Set in TransferFunctionPanel.cs
    public TFColourControlPoint controlPoint;
    public TransferFunction transferFunction;
    public int index;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        colorView = GameObject.Find("TransferFuncColor");
        transferFuncManager = GameObject.Find("TransferFunctionPanel").GetComponent<TransferFunctionPanel>();
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
        if (rectTransform.anchoredPosition.y != 0)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
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

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        transferFuncManager.selectedColourControlPointIndex = index;
    }
}
