using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

namespace UI
{
    public class TransferFunctionPanelPlot : MonoBehaviour
    {
        public GameObject volumeTransformer;
        private GameObject[] volumes;
        
        private GameObject selectedVolume1;
        private GameObject selectedVolume2;
        public GameObject volume1dropdown;
        public GameObject volume2dropdown;
        
        private GameObject dropdownMenu;

        private GameObject colorView;
        private GameObject alphaView;
        public GameObject alphaControlPointPrefab;
        public GameObject colourControlPointPrefab;
        private int activeAttribute;
        private TransferFunction secondaryTransferFunction;
    
        // Color picker
        public RectTransform texture;
        public Texture2D refSprite;
        public int selectedColourControlPointIndex = -1;

        // Start is called before the first frame update
        void Start()
        {
            volumes = new GameObject[volumeTransformer.transform.childCount];

            for (var i = 0; i < volumes.Length; i++) volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
            
            dropdownMenu = GameObject.Find("DropdownAttributeSelector");
            colorView = GameObject.Find("TransferFuncColor");
            alphaView = GameObject.Find("TransferFuncAlpha");

            selectedVolume1 = volumes[4];
            selectedVolume2 = volumes[1];
            
            dropdownMenu.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData("meteorite"));
            dropdownMenu.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData("pressure"));
            dropdownMenu.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData("temperature"));
            dropdownMenu.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData("water"));
            
            dropdownMenu.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
            {
                TfLayerChanged(dropdownMenu.GetComponent<TMP_Dropdown>().value);
            });
            
            volume1dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
            {
                VolumeDropdownChanged();
            });
            
            volume2dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
            {
                VolumeDropdownChanged();
            });
        
            // Initialize transfer function window
            TfLayerChanged(0);
        }

        void VolumeDropdownChanged()
        {
            int currValueDrop1 = volume1dropdown.GetComponent<TMP_Dropdown>().value -1;
            int currValueDrop2 = volume2dropdown.GetComponent<TMP_Dropdown>().value -1;

            if (currValueDrop1 > -1)
            {
                selectedVolume1 = volumes[currValueDrop1];
            }
            
            if (currValueDrop2 > -1)
            {
                selectedVolume2 = volumes[currValueDrop2];
            }
        }

        void TfLayerChanged(int value)
        {
            activeAttribute = value;

            TransferFunction transferFunction =
                selectedVolume1.transform.GetChild(value).GetComponent<VolumeRenderedObject>().transferFunction;
        
            // Set transfer function for other volume
            selectedVolume2.transform.GetChild(value).GetComponent<VolumeRenderedObject>().transferFunction = transferFunction;
        
            Texture2D transferFuncTex = transferFunction.GetTexture();
            alphaView.GetComponent<RawImage>().texture = transferFuncTex;
        
            // Create only color transfer function for color view
            secondaryTransferFunction = ScriptableObject.CreateInstance<TransferFunction>();
        
            // Create full alpha control point
            List<TFAlphaControlPoint> fullAlphaControlPointList = new List<TFAlphaControlPoint> { new TFAlphaControlPoint(0.0f, 1.0f),new TFAlphaControlPoint(0.5f, 1.0f) };
            secondaryTransferFunction.alphaControlPoints = fullAlphaControlPointList;
        
            // Copy color control points from primary transfer function
            secondaryTransferFunction.colourControlPoints =
                new List<TFColourControlPoint>(transferFunction.colourControlPoints);
        
            colorView.GetComponent<RawImage>().texture = secondaryTransferFunction.GetTexture();
        
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
                selectedVolume1.transform.GetChild(index).GetComponent<VolumeRenderedObject>().transferFunction;
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
                selectedVolume1.transform.GetChild(index).GetComponent<VolumeRenderedObject>().transferFunction;
            controlPointUI.GetComponent<UIDraggableColor>().secondaryTransferFunction = secondaryTransferFunction;
            controlPointUI.GetComponent<UIDraggableColor>().index = indexControlPoint;
        }

        public void AddColourControlPoint()
        {
            TransferFunction transferFunction =
                selectedVolume1.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
            TFColourControlPoint controlPoint = new TFColourControlPoint(0.5f, Color.white);
            transferFunction.colourControlPoints.Add(controlPoint);
            transferFunction.GenerateTexture();
        
            // Add to secondary transfer function
            secondaryTransferFunction.colourControlPoints.Add(controlPoint);
            secondaryTransferFunction.GenerateTexture();
        
            NewColourControlPointUI(controlPoint, activeAttribute, transferFunction.colourControlPoints.Count - 1);
        }
    
        public void AddAlphaControlPoint()
        {
            TransferFunction transferFunction =
                selectedVolume1.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
            TFAlphaControlPoint controlPoint = new TFAlphaControlPoint(0.5f, 0.5f);
            transferFunction.alphaControlPoints.Add(controlPoint);
            transferFunction.GenerateTexture();
            NewAlphaControlPointUI(controlPoint, activeAttribute, transferFunction.alphaControlPoints.Count - 1);
        }

        private void SetColor()
        {
            Vector3 imagePos = texture.position;
            //Vector3 inputPos = Input.mousePosition; // Desktop use
            Vector3 inputPos = GameObject.Find("Right Grab Ray").GetComponent<LineRenderer>().GetPosition(1); // For VR use only
            float globalPosX = inputPos.x - imagePos.x;
            float globalPosY = inputPos.y - imagePos.y;
            int localPosX = (int)(globalPosX * (refSprite.width / texture.rect.width) / 0.827); // factor considering stretch hard coded
            int localPosY = (int)(globalPosY * (refSprite.height / texture.rect.height) / 0.38);
            Color c = refSprite.GetPixel(localPosX, localPosY);
        
            TransferFunction transferFunction =
                selectedVolume1.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
            TFColourControlPoint oldCp = transferFunction.colourControlPoints[selectedColourControlPointIndex];
            transferFunction.colourControlPoints[selectedColourControlPointIndex] = new TFColourControlPoint(oldCp.dataValue, c);
        
            // Apply changes to secondary transfer function
            secondaryTransferFunction.colourControlPoints =
                new List<TFColourControlPoint>(transferFunction.colourControlPoints);
        
            secondaryTransferFunction.GenerateTexture();
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
}
