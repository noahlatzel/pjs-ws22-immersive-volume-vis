using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityVolumeRendering;

public class UIDraggableAlpha : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private GameObject alphaView;
    private UI.TransferFunctionPanelMainScene transferFuncManager;

    // Set in TransferFunctionPanel.cs
    public TFAlphaControlPoint controlPoint;
    public TransferFunction transferFunction;
    public int index;

    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        alphaView = GameObject.Find("TransferFuncAlpha");
        transferFuncManager = GameObject.Find("TransferFunctionPanel").GetComponent<TransferFunctionPanelMainScene>();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta; // For mouse use only
        rectTransform.position = GameObject.Find("Right Grab Ray").GetComponent<LineRenderer>().GetPosition(1); // For VR use only

        float maxHeight = alphaView.GetComponent<RectTransform>().rect.height;
        float maxWidth = alphaView.GetComponent<RectTransform>().rect.width;

        float myMaxWidth = GetComponent<RectTransform>().rect.width;
        float myMaxHeight = GetComponent<RectTransform>().rect.height;

        if (rectTransform.anchoredPosition.x < 0)
        {
            if (rectTransform.anchoredPosition.x < -60 && rectTransform.anchoredPosition.y < 50)
            {
                rectTransform.anchoredPosition = new Vector2(-60, rectTransform.anchoredPosition.y);
            }
            else if (rectTransform.anchoredPosition.y >= 50)
            {
                rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
            }
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

        controlPoint.alphaValue = rectTransform.anchoredPosition.y / (maxHeight - myMaxHeight);
        controlPoint.dataValue = rectTransform.anchoredPosition.x / (maxWidth- myMaxWidth);
        transferFunction.alphaControlPoints[index] = controlPoint;

        
        //transferFunction.GenerateTexture();
        transferFuncManager.GenerateTexture();
    }

    public void OnEndDrag(PointerEventData pointerEventData) {
        if (rectTransform.anchoredPosition.x < -30)
        {
            transferFunction.alphaControlPoints.RemoveAt(index);

            // Fix index of remaining control points
            for (int i = 0; i < alphaView.transform.childCount; i++)
            {
                if (alphaView.transform.GetChild(i).GetComponent<UIDraggableAlpha>().index > index)
                {
                    alphaView.transform.GetChild(i).GetComponent<UIDraggableAlpha>().index--;
                }
            }

            // Destroy gameObject
            Destroy(gameObject);
        }
        else if (rectTransform.anchoredPosition.x < 0) {
            rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
        }
    }
}
