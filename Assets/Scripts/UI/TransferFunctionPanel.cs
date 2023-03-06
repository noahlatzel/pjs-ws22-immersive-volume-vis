using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class TransferFunctionPanel : MonoBehaviour
{
    private GameObject selectedVolume;
    private GameObject dropdownMenu;
    private GameObject colorView;
    private GameObject alphaView;
    public GameObject alphaControlPointPrefab;
    public GameObject colourControlPointPrefab;
    private int activeAttribute;
    
    // Color picker
    public RectTransform texture;
    public Texture2D refSprite;
    public int selectedColourControlPointIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        dropdownMenu = GameObject.Find("DropdownAttributeSelector");
        selectedVolume = GameObject.Find("RenderedVolume");
        colorView = GameObject.Find("TransferFuncColor");
        alphaView = GameObject.Find("TransferFuncAlpha");
        
        // Set dropdown options
        for (int i = 0; i < selectedVolume.transform.childCount; i++)
        {
            dropdownMenu.GetComponent<TMP_Dropdown>().options.Add(
                new TMP_Dropdown.OptionData(selectedVolume.transform.GetChild(i).name));
        }
        
        dropdownMenu.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
        {
            DropDownValueChanged(dropdownMenu.GetComponent<TMP_Dropdown>().value);
        });
        
        // Initialize transfer function window
        DropDownValueChanged(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DropDownValueChanged(int value)
    {
        activeAttribute = value;
        TransferFunction transferFunction =
            selectedVolume.transform.GetChild(value).GetComponent<VolumeRenderedObject>().transferFunction;
        Texture2D transferFuncTex = transferFunction.GetTexture();
        colorView.GetComponent<RawImage>().texture = transferFuncTex;
        alphaView.GetComponent<RawImage>().texture = transferFuncTex;
        
        // Delete old control points (alpha)
        for (int i = 0; i < alphaView.transform.childCount; i++)
        {
            Destroy(alphaView.transform.GetChild(i).gameObject);
        }
        
        // Delete old control points (colour)
        for (int i = 0; i < colorView.transform.childCount; i++)
        {
            Destroy(colorView.transform.GetChild(i).gameObject);
        }
        
        // Create new control points (alpha)
        for (int i = 0; i < transferFunction.alphaControlPoints.Count; i++)
        {
            NewAlphaControlPointUI(transferFunction.alphaControlPoints[i], value, i);
        }
        
        // Create new control points (colour)
        for (int i = 0; i < transferFunction.colourControlPoints.Count; i++)
        {
            NewColourControlPointUI(transferFunction.colourControlPoints[i], value, i);
        }
    }

    void NewAlphaControlPointUI(TFAlphaControlPoint controlPoint, int index, int indexControlPoint)
    {
        // Instantiate new control point from Button prefab
        GameObject controlPointUI = Instantiate(alphaControlPointPrefab);
        
        // Get base dimensions
        float baseHeight = alphaView.GetComponent<RectTransform>().rect.height;
        float baseWidth = alphaView.GetComponent<RectTransform>().rect.width;
        
        // Configure control point
        RectTransform transformControlPoint = controlPointUI.GetComponent<RectTransform>();
        transformControlPoint.SetParent(alphaView.transform, false);
        transformControlPoint.name = "AlphaControlPoint";
        transformControlPoint.transform.localScale = new Vector3(1, 1, 1);
        transformControlPoint.anchoredPosition =
            new Vector2(baseWidth * controlPoint.dataValue, baseHeight * controlPoint.alphaValue);

        controlPointUI.GetComponent<UIDraggableAlpha>().controlPoint = controlPoint;
        controlPointUI.GetComponent<UIDraggableAlpha>().transferFunction = 
            selectedVolume.transform.GetChild(index).GetComponent<VolumeRenderedObject>().transferFunction;
        controlPointUI.GetComponent<UIDraggableAlpha>().index = indexControlPoint; 
    }
    
    void NewColourControlPointUI(TFColourControlPoint controlPoint, int index, int indexControlPoint)
    {
        // Instantiate new control point from Button prefab
        GameObject controlPointUI = Instantiate(colourControlPointPrefab);
        
        // Get base dimensions
        float baseWidth = colorView.GetComponent<RectTransform>().rect.width;
        
        // Configure control point
        RectTransform transformControlPoint = controlPointUI.GetComponent<RectTransform>();
        transformControlPoint.SetParent(colorView.transform, false);
        transformControlPoint.name = "ColourControlPoint";
        transformControlPoint.transform.localScale = new Vector3(1, 1, 1);
        transformControlPoint.anchoredPosition =
            new Vector2(baseWidth * controlPoint.dataValue, 0);
        controlPointUI.GetComponent<UIDraggableColor>().controlPoint = controlPoint;
        controlPointUI.GetComponent<UIDraggableColor>().transferFunction = 
            selectedVolume.transform.GetChild(index).GetComponent<VolumeRenderedObject>().transferFunction;
        controlPointUI.GetComponent<UIDraggableColor>().index = indexControlPoint;
    }

    public void AddColourControlPoint()
    {
        TransferFunction transferFunction =
            selectedVolume.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
        TFColourControlPoint controlPoint = new TFColourControlPoint(0.5f, Color.white);
        transferFunction.colourControlPoints.Add(controlPoint);
        NewColourControlPointUI(controlPoint, activeAttribute, transferFunction.colourControlPoints.Count - 1);
    }
    
    public void AddAlphaControlPoint()
    {
        TransferFunction transferFunction =
            selectedVolume.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
        TFAlphaControlPoint controlPoint = new TFAlphaControlPoint(0.5f, 0.5f);
        transferFunction.alphaControlPoints.Add(controlPoint);
        NewAlphaControlPointUI(controlPoint, activeAttribute, transferFunction.alphaControlPoints.Count - 1);
    }

    private void SetColor()
    {
        Vector3 imagePos = texture.position;
        float globalPosX = Input.mousePosition.x - imagePos.x;
        float globalPosY = Input.mousePosition.y - imagePos.y;

        int localPosX = (int)(globalPosX * (refSprite.width / texture.rect.width));
        int localPosY = (int)(globalPosY * (refSprite.height / texture.rect.height));

        Color c = refSprite.GetPixel(localPosX, localPosY);
        
        TransferFunction transferFunction =
            selectedVolume.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
        TFColourControlPoint oldCP = transferFunction.colourControlPoints[selectedColourControlPointIndex];
        transferFunction.colourControlPoints[selectedColourControlPointIndex] = new TFColourControlPoint(oldCP.dataValue, c);
        transferFunction.GenerateTexture();
    }

    public void OnClickColorPicker()
    {
        if (selectedColourControlPointIndex > -1)
        {
            SetColor();
        }
    }
}