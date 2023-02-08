using JsonReader;
using UnityEngine;
using UnityEngine.UI;

public class SimRunVisToggles : MonoBehaviour
{
    [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

    [Tooltip("Assign number of simulation run to be set.")]
    public int assignedRun;

    private CreatePlot createPlot;
    private bool[] simRunVisArr;
    private Toggle thisToggle;

    // Start is called before the first frame update
    private void Start()
    {
        createPlot = plotGameObject.GetComponent<CreatePlot>();
        thisToggle = gameObject.GetComponent<Toggle>();


        simRunVisArr = createPlot.simRunVisibilities;

        thisToggle.isOn = simRunVisArr[assignedRun];
    }

    // Update is called once per frame
    private void Update()
    {
        simRunVisArr = createPlot.simRunVisibilities;
        if (thisToggle.isOn != simRunVisArr[assignedRun]) thisToggle.isOn = simRunVisArr[assignedRun];
    }

    public void ToggleAction()
    {
        simRunVisArr[assignedRun] = thisToggle.isOn;

        createPlot.SetVisibilityOfSingleRun(assignedRun, simRunVisArr[assignedRun]);
    }
}