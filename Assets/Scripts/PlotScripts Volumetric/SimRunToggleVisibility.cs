using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimRunToggleVisibility : MonoBehaviour
{
    public GameObject toggle0;
    public GameObject toggle1;
    public GameObject toggle2;
    public GameObject toggle3;
    public GameObject toggle4;
    public GameObject toggle5;
    public GameObject toggle6;

    private List<GameObject> toggles;

    public GameObject volumeTransformer;

    private Vector3[] togglePos;

    private GameObject[] volumes;

    // Start is called before the first frame update
    void Start()
    {
        volumes = new GameObject[volumeTransformer.transform.childCount];

        for (var i = 0; i < volumes.Length; i++) volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;

        togglePos = new Vector3[7];
        
        for (int i = 0; i < 7; i++)
        {
            togglePos[i] = new Vector3(450, -25 + (i * -100), 0);
        }

        toggles = new List<GameObject>();

        toggles.Add(toggle0);
        toggles.Add(toggle1);
        toggles.Add(toggle2);
        toggles.Add(toggle3);
        toggles.Add(toggle4);
        toggles.Add(toggle5);
        toggles.Add(toggle6);

        for (int i = 0; i < 7; i++)
        {
            TextMeshProUGUI currText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
            
            currText.SetText("Simulation " + volumes[i].name);

            if (volumes[i].activeSelf)
            {
                currText.color = Color.white;
            }
            else
            {
                currText.color = Color.grey;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}