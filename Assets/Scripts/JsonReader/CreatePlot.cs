using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace JsonReader
{
    public class CreatePlot : MonoBehaviour
    {
        [Tooltip("Show/Hide pressure plot.")] public bool pressure;

        [Tooltip("Show/Hide temperature plot.")]
        public bool temperature;

        [Tooltip("Show/Hide water plot.")] public bool water;

        [Tooltip("Show/Hide meteorite plot.")] public bool meteorite;

        [Tooltip("Choose 1D, 2D or 3D plot.")] public int dimensions;

        [Tooltip("Show/Hide simulation run 0 plot.")]
        public bool simRun0Vis;

        [Tooltip("Show/Hide simulation run 1 plot.")]
        public bool simRun1Vis;

        [Tooltip("Show/Hide simulation run 2 plot.")]
        public bool simRun2Vis;

        [Tooltip("Show/Hide simulation run 3 plot.")]
        public bool simRun3Vis;

        [Tooltip("Show/Hide simulation run 4 plot.")]
        public bool simRun4Vis;

        [Tooltip("Show/Hide simulation run 5 plot.")]
        public bool simRun5Vis;

        [Tooltip("Show/Hide simulation run 6 plot.")]
        public bool simRun6Vis;

        //Variable to keep track of the selected graph
        public int selectedGraphIndex = -1;

        public UnityEvent testEvent;

        public string[] layers = { "Pressure", "Temperature", "Water", "Meteorite" };

        private bool[] layerVisibilities = { true, true, true, true };

        private Vector3 playerPosition;


        //List of lists to store multiple sets of points, each representing a separate graph
        public List<List<List<Vector3>>> pointsList;
        private bool[] simRunVisibilities = { true, true, true, true, true, true, true };

        private void Start()
        {
            dimensions = 3;

            layerVisibilities = new[] { pressure, temperature, water, meteorite };

            simRunVisibilities = new[]
                { simRun0Vis, simRun1Vis, simRun2Vis, simRun3Vis, simRun4Vis, simRun5Vis, simRun6Vis };

            //Access first layer of plotData
            pointsList = Reader.GiveDataList();

            // Make sure that the pointsList is not empty
            if (pointsList == null || pointsList.Count == 0)
            {
                Debug.LogError("pointsList is empty");
                return;
            }

            for (var i = 0; i < pointsList.Count; i++)
            {
                var layerObj = gameObject.transform.Find(layers[i]);

                // Iterate through each list of points
                for (var j = 0; j < pointsList[i].Count; j++)
                {
                    // //Create a new GameObject for every line on the plot
                    // var newObj = new GameObject();
                    //
                    // //Put the new GameObject under this object in hierarchy
                    // newObj.transform.SetParent(gameObject.transform);
                    //
                    // //Rename the new GameObject according to the simulation run it belongs to
                    // newObj.transform.name = "SimulationRun_" + j;
                    //
                    // // Create a new LineRenderer component
                    // var lineRenderer = newObj.AddComponent<LineRenderer>();

                    var contObj = layerObj.transform.Find("SimRun_" + i + "_" + j);

                    var lineRenderer = contObj.GetComponent<LineRenderer>();

                    // Adjust styling of the LineRenderer
                    lineRenderer.widthMultiplier = 0.005f;
                    lineRenderer.shadowCastingMode = ShadowCastingMode.Off;

                    //Make LineRenderer's position adjustable
                    lineRenderer.useWorldSpace = false;

                    // Set the LineRenderer's position count
                    lineRenderer.positionCount = pointsList[i][j].Count;

                    //Set vertices of the LineRenderer according to plotData for current layer and simulation run
                    lineRenderer.SetPositions(pointsList[i][j].ToArray());
                }
            }

            SetVisibilities(layerVisibilities, simRunVisibilities);
        }

        public void SetVisibilities(bool[] layerVisibilitiesArr, bool[] simRunVisibilitiesArr)
        {
            if (layerVisibilitiesArr.Length != 4 || simRunVisibilitiesArr.Length != 7)
            {
                Debug.LogError("CreatePlot.SetVisibilities: Incorrect length at parameter arrays");
                return;
            }

            for (var i = 0; i < 4; i++)
            {
                var layerObj = gameObject.transform.Find(layers[i]).gameObject;

                for (var j = 0; j < 7; j++)
                {
                    var simRunObj = layerObj.transform.Find("SimRun_" + i + "_" + j).gameObject;

                    simRunObj.SetActive(simRunVisibilitiesArr[j]);
                }

                layerObj.SetActive(layerVisibilitiesArr[i]);
            }
        }

        public void SetVisibilityOfSingleRun(int runNum, bool val)
        {
            simRunVisibilities[runNum] = val;
            SetVisibilities(layerVisibilities, simRunVisibilities);
        }

        // public List<Vector3> SetPlotData(int dimension)
        // {
        //     
        // }


        // //Method to get a specific point value from the graph
        // public Vector3 GetPointValue(int graphIndex, int pointIndex)
        // {
        //     return pointsList[graphIndex][pointIndex];
        // }

        //Method to select a graph by its index
        public void SelectGraph(int index)
        {
            selectedGraphIndex = index;
        }

        public void testBool(bool test)
        {
            Debug.Log(test);
        }


        // //Method that takes the player's position as a parameter and returns the closest point on the selected graph to the player
        // public Vector3 GetClosestPointOnSelectedGraph(Vector3 playerPosition)
        // {
        //     //Check if a graph has been selected
        //     if (selectedGraphIndex == -1)
        //     {
        //         Debug.LogError("No graph is selected.");
        //         return Vector2.zero;
        //     }
        //
        //     //Use LINQ to calculate the distance between each point on the selected graph and the player's position
        //     var distances = pointsList[selectedGraphIndex].Select(p => Vector3.Distance(p, playerPosition));
        //     //Find the minimum distance
        //     var closestPointIndex = distances.ToList().IndexOf(distances.Min());
        //
        //     //Return the corresponding point
        //     return pointsList[selectedGraphIndex][closestPointIndex];
        // }
    }
}