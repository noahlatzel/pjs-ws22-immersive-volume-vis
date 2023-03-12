using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityVolumeRendering;

public class UIDraggableColor : MonoBehaviour, IDragHandler, IPointerClickHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private GameObject colorView;
    private TransferFunctionPanel transferFuncManager;
    
    // Set in TransferFunctionPanel.cs
    public TFColourControlPoint controlPoint;
    public TransferFunction transferFunction;
    public TransferFunction secondaryTransferFunction;
    public int index;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        colorView = GameObject.Find("TransferFuncColor");
        transferFuncManager = GameObject.Find("TransferFunctionPanel").GetComponent<TransferFunctionPanel>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta; // For mouse use only
        rectTransform.position = GameObject.Find("Right Grab Ray").GetComponent<LineRenderer>().GetPosition(1); // For VR use only

        float maxHeight = colorView.GetComponent<RectTransform>().rect.height;
        float maxWidth = colorView.GetComponent<RectTransform>().rect.width;

        float myMaxWidth = GetComponent<RectTransform>().rect.width;
        float myMaxHeight = GetComponent<RectTransform>().rect.height;

        if (rectTransform.anchoredPosition.x <= 0)
        {
            if (rectTransform.anchoredPosition.x < -60)
            {
                rectTransform.anchoredPosition = new Vector2(-60, rectTransform.anchoredPosition.y);
            }
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
        controlPoint.colourValue = transferFunction.colourControlPoints[index].colourValue;
        
        transferFunction.colourControlPoints[index] = controlPoint;
        
        // Add changes to secondary transfer function
        secondaryTransferFunction.colourControlPoints =
            new List<TFColourControlPoint>(transferFunction.colourControlPoints);
        
        secondaryTransferFunction.GenerateTexture();
        transferFunction.GenerateTexture();
        
        transferFuncManager.selectedColourControlPointIndex = index;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        transferFuncManager.selectedColourControlPointIndex = index;
    }

    public void OnEndDrag(PointerEventData pointerEventData) {
        if (rectTransform.anchoredPosition.x < -30)
        {
            transferFunction.colourControlPoints.RemoveAt(index);

            // Fix index of remaining control points
            for (int i = 0; i < colorView.transform.childCount; i++)
            {
                if (colorView.transform.GetChild(i).GetComponent<UIDraggableColor>().index > index)
                {
                    colorView.transform.GetChild(i).GetComponent<UIDraggableColor>().index--;
                }
            }
            
            // Add changes to secondary transfer function
            secondaryTransferFunction.colourControlPoints =
                new List<TFColourControlPoint>(transferFunction.colourControlPoints);

            // Destroy gameObject
            Destroy(gameObject);
        }
        else if (rectTransform.anchoredPosition.x < 0)
        {
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }
    }
}
