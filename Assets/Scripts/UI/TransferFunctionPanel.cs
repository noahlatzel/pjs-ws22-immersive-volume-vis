using System;
using System.Collections.Generic;
using System.Linq;
using ImportVolume;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

namespace UI
{
    public class TransferFunctionPanel : MonoBehaviour
    {
        private TMP_Dropdown Volume1Dropdown;
        private TMP_Dropdown Volume2Dropdown;
        
        public GameObject Volume1Drop;
        public GameObject Volume2Drop;
        
        private GameObject selectedVolume;
        private GameObject selectedVolume2;
        
        private GameObject[] volumes;
        public GameObject volumeTransformer;
        
        private GameObject dropdownMenu;
        private GameObject colorView;
        private GameObject alphaView;
        public GameObject alphaControlPointPrefab;
        public GameObject colourControlPointPrefab;
        private int activeAttribute;
        private TransferFunction secondaryTransferFunction;

        List<String[]> names = new List<String[]>();

        String[] tev = new[] { "temperature", "tev", "Temperature" };
        String[] prs = new[] { "pressure", "prs", "Pressure" };
        String[] v02 = new[] { "water", "v02", "Water" };
        String[] v03 = new[] { "meteorite", "v03", "Meteorite" };


        // Color picker
        public RectTransform texture;
        public Texture2D refSprite;
        public int selectedColourControlPointIndex = -1;

        // Start is called before the first frame update
        public void Start()
        {
            Volume1Dropdown = Volume1Drop.GetComponent<TMP_Dropdown>();
            Volume2Dropdown = Volume2Drop.GetComponent<TMP_Dropdown>();
            
            volumes = new GameObject[volumeTransformer.transform.childCount];

            for (var i = 0; i < volumes.Length; i++) volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;

            names.Add(tev);
            names.Add(prs);
            names.Add(v02);
            names.Add(v03);

            bool first = true;
            for (int i = 0; i < volumes.Length; i++)
            {
                if (volumes[i].transform.childCount > 0)
                {
                    if (first)
                    {
                        selectedVolume = volumes[i];
                        first = false;
                    }
                    else
                    {
                        selectedVolume2 = volumes[i];
                    }
                    Debug.Log("HERE");
                }
            }

            //selectedVolume = volumes[3];
            //selectedVolume2 = volumes[3];
            
            dropdownMenu = GameObject.Find("DropdownAttributeSelector");
            colorView = GameObject.Find("TransferFuncColor");
            alphaView = GameObject.Find("TransferFuncAlpha");

            // Clear option list
            dropdownMenu.GetComponent<TMP_Dropdown>().options = new List<TMP_Dropdown.OptionData>();
            
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
            
            Volume1Dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
            {
                volumeChanged();
            });
            
            Volume2Dropdown.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
            {
                volumeChanged();
            });
        
            // Initialize transfer function window
            // DropDownValueChanged(3);
        }

        void volumeChanged()
        {
            GameObject volume1;
            GameObject volume2;
            int currentSelectedVolume1 = Volume1Dropdown.value;
            int currentSelectedVolume2 = Volume2Dropdown.value;
            if (currentSelectedVolume1 > 0)
            {
                selectedVolume = volumes[currentSelectedVolume1 - 1];
            }
            if (currentSelectedVolume2 > 0)
            {
                selectedVolume2 = volumes[currentSelectedVolume2 - 1];
            }
            
        }

        void DropDownValueChanged(int value)
        {
            activeAttribute = value; 
            String activeAttributeName = dropdownMenu.GetComponent<TMP_Dropdown>().captionText.text;
            
            Debug.Log("DropDownValueChanged: Aktuelles Volumeattribute: " + selectedVolume.transform.GetChild(value).name + " Aktuelles Volume: " + selectedVolume.name);
            
            GameObject foundObject = new GameObject();
            bool found = false;
            foreach (String[] currVolNames in names)
            {
                if (currVolNames.Contains<String>(activeAttributeName))
                {
                    foreach (var currName in currVolNames)
                    {
                        if (selectedVolume.transform.Find(currName))
                        {
                            foundObject = selectedVolume.transform.Find(currName).gameObject;
                            found = true;

                            for (int i = 0; i < selectedVolume.transform.childCount; i++)
                            {
                                if (selectedVolume.transform.GetChild(i).gameObject.name.Equals(currName))
                                {
                                    activeAttribute = i;
                                }
                }
                        }
                    }
                }
            }

            if (!found)
            {
                Debug.Log("TransferFunctionPanel.DropDownValueChanged: Kein Layer mit dem Namen " + activeAttributeName + " gefunden.");
            }
            else
            {


                Debug.Log("TransferFunctionPanel.DropDownValueChanged: Child " + foundObject.name + " gefunden! ActiveAttribute: " + activeAttributeName);
                TransferFunction transferFunction =
                    foundObject.GetComponent<VolumeRenderedObject>().transferFunction;

                Texture2D transferFuncTex = transferFunction.GetTexture();
                alphaView.GetComponent<RawImage>().texture = transferFuncTex;

                // Create only color transfer function for color view
                secondaryTransferFunction = ScriptableObject.CreateInstance<TransferFunction>();

                // Create full alpha control point
                List<TFAlphaControlPoint> fullAlphaControlPointList = new List<TFAlphaControlPoint>
                    { new TFAlphaControlPoint(0.0f, 1.0f), new TFAlphaControlPoint(0.5f, 1.0f) };
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
                
                found = false;
                foundObject = new GameObject();

                foreach (String[] currVolNames in names)
                {
                    if (currVolNames.Contains<String>(activeAttributeName))
                    {
                        foreach (var currName in currVolNames)
                        {
                            if (selectedVolume.transform.Find(currName))
                            {
                                foundObject = selectedVolume.transform.Find(currName).gameObject;
                                found = true;
                            }
                        }
                    }
                }

                if (!found)
                {
                    Debug.Log("TransferFunctionPanel.DropDownValueChanged: Kein Layer im ersten Volume mit dem Namen " + activeAttributeName + " gefunden.");
                }
                else
                {
                    Debug.Log("TransferFunctionPanel.DropDownValueChanged: Child " + foundObject.name + " gesetzt! ActiveAttribute: " + activeAttributeName);

                    // Set transfer function for other volume
                    foundObject.GetComponent<VolumeRenderedObject>()
                        .transferFunction = transferFunction;
                }
                
                found = false;
                foundObject = new GameObject();

                foreach (String[] currVolNames in names)
                {
                    if (currVolNames.Contains<String>(activeAttributeName))
                    {
                        foreach (var currName in currVolNames)
                        {
                            if (selectedVolume2.transform.Find(currName))
                            {
                                foundObject = selectedVolume2.transform.Find(currName).gameObject;
                                found = true;
                            }
                        }
                    }
                }

                if (!found)
                {
                    Debug.Log("TransferFunctionPanel.DropDownValueChanged: Kein Layer im zweiten Volume mit dem Namen " + activeAttributeName + " gefunden.");
                }
                else
                {
                    Debug.Log("TransferFunctionPanel.DropDownValueChanged: Zweites Child " + foundObject.name + " gesetzt! ActiveAttribute: " + activeAttributeName);

                    // Set transfer function for other volume
                    foundObject.GetComponent<VolumeRenderedObject>()
                        .transferFunction = transferFunction;
                }
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


            
            String activeAttributeName = dropdownMenu.GetComponent<TMP_Dropdown>().captionText.text;

            Debug.Log("NewAlphaControlPointUI: Aktuelles Volumeattribute: " + selectedVolume.transform.GetChild(index).name + " Aktuelles Volume: " + selectedVolume.name);

            GameObject foundObject = new GameObject();
            bool found = false;
            foreach (String[] currVolNames in names)
            {
                if (currVolNames.Contains<String>(activeAttributeName))
                {
                    foreach (var currName in currVolNames)
                    {
                        if (selectedVolume.transform.Find(currName))
                        {
                            foundObject = selectedVolume.transform.Find(currName).gameObject;
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Debug.Log("TransferFunctionPanel.NewAlphaControlPointUI: Kein Layer mit dem Namen " + activeAttributeName + " gefunden.");
            }
            else
            {
                Debug.Log("TransferFunctionPanel.NewAlphaControlPointUI: Child " + foundObject.name + " gefunden! ActiveAttribute: " + activeAttributeName);

                controlPointUI.GetComponent<UIDraggableAlpha>().transferFunction =
                foundObject.GetComponent<VolumeRenderedObject>().transferFunction;
                controlPointUI.GetComponent<UIDraggableAlpha>().index = indexControlPoint;

            }
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
            controlPointUI.GetComponent<UIDraggableColorPlot>().controlPoint = controlPoint;

            String activeAttributeName = dropdownMenu.GetComponent<TMP_Dropdown>().captionText.text;

            Debug.Log("NewColourControlPointUI: Aktuelles Volumeattribute: " + selectedVolume.transform.GetChild(index).name + " Aktuelles Volume: " + selectedVolume.name);

            GameObject foundObject = new GameObject();
            bool found = false;



            foreach (String[] currVolNames in names)
            {
                if (currVolNames.Contains<String>(activeAttributeName))
                {
                    foreach (var currName in currVolNames)
                    {
                        if (selectedVolume.transform.Find(currName))
                        {
                            foundObject = selectedVolume.transform.Find(currName).gameObject;
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Debug.Log("TransferFunctionPanel.NewColourControlPointUI: Kein Layer mit dem Namen " + activeAttributeName + " gefunden.");
            }
            else
            {
                Debug.Log("TransferFunctionPanel.NewColourControlPointUI: Child " + foundObject.name + " gefunden! ActiveAttribute: " + activeAttributeName);


                controlPointUI.GetComponent<UIDraggableColorPlot>().transferFunction =
                foundObject.GetComponent<VolumeRenderedObject>().transferFunction;
                controlPointUI.GetComponent<UIDraggableColorPlot>().secondaryTransferFunction = secondaryTransferFunction;
                controlPointUI.GetComponent<UIDraggableColorPlot>().index = indexControlPoint;
            }
        }

        public void AddColourControlPoint()
        {
            String activeAttributeName = dropdownMenu.GetComponent<TMP_Dropdown>().captionText.text;

            Debug.Log("AddColourControlPoint: Aktuelles Volumeattribute: " + selectedVolume.transform.GetChild(activeAttribute).name + " Aktuelles Volume: " + selectedVolume.name);

            GameObject foundObject = new GameObject();
            bool found = false;
            foreach (String[] currVolNames in names)
            {
                if (currVolNames.Contains<String>(activeAttributeName))
                {
                    foreach (var currName in currVolNames)
                    {
                        if (selectedVolume.transform.Find(currName))
                        {
                            foundObject = selectedVolume.transform.Find(currName).gameObject;
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Debug.Log("TransferFunctionPanel.AddColourControlPoint: Kein Layer mit dem Namen " + activeAttributeName + " gefunden.");
            }
            else
            {
                Debug.Log("TransferFunctionPanel.AddColourControlPoint: Child " + foundObject.name + " gefunden! ActiveAttribute: " + activeAttributeName);


                TransferFunction transferFunction =
                foundObject.GetComponent<VolumeRenderedObject>().transferFunction;
                TFColourControlPoint controlPoint = new TFColourControlPoint(0.5f, Color.white);
                transferFunction.colourControlPoints.Add(controlPoint);
                transferFunction.GenerateTexture();

                // Add to secondary transfer function
                secondaryTransferFunction.colourControlPoints.Add(controlPoint);
                secondaryTransferFunction.GenerateTexture();

                NewColourControlPointUI(controlPoint, activeAttribute, transferFunction.colourControlPoints.Count - 1);
            }
        }
    
        public void AddAlphaControlPoint()
        {
            String activeAttributeName = dropdownMenu.GetComponent<TMP_Dropdown>().captionText.text;

            Debug.Log("AddColourControlPoint: Aktuelles Volumeattribute: " + selectedVolume.transform.GetChild(activeAttribute).name + " Aktuelles Volume: " + selectedVolume.name);

            GameObject foundObject = new GameObject();
            bool found = false;
            foreach (String[] currVolNames in names)
            {
                if (currVolNames.Contains<String>(activeAttributeName))
                {
                    foreach (var currName in currVolNames)
                    {
                        if (selectedVolume.transform.Find(currName))
                        {
                            foundObject = selectedVolume.transform.Find(currName).gameObject;
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Debug.Log("TransferFunctionPanel.AddColourControlPoint: Kein Layer mit dem Namen " + activeAttributeName + " gefunden.");
            }
            else
            {
                Debug.Log("TransferFunctionPanel.AddColourControlPoint: Child " + foundObject.name + " gefunden! ActiveAttribute: " + activeAttributeName);

                TransferFunction transferFunction =
                foundObject.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
                TFAlphaControlPoint controlPoint = new TFAlphaControlPoint(0.5f, 0.5f);
                transferFunction.alphaControlPoints.Add(controlPoint);
                transferFunction.GenerateTexture();
                NewAlphaControlPointUI(controlPoint, activeAttribute, transferFunction.alphaControlPoints.Count - 1);
            }
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
                selectedVolume.transform.GetChild(activeAttribute).GetComponent<VolumeRenderedObject>().transferFunction;
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


