using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PlotScripts_Volumetric
{
    public class CreatePlot : MonoBehaviour
    {
        public int dimension;

        // //Variable to keep track of the selected graph
        // public int selectedGraphIndex = -1;

        public bool[] simRunVisibilities = { true, true, true, true, true, true, true };
        public bool[] layerVisibilities = { true, false, false, false };

        //Names of all the layers as array
        private readonly string[] layers = { "Pressure", "Temperature", "Water", "Meteorite" };

        private Vector3 playerPosition;

        //List of lists to store multiple sets of points, each representing a separate graph
        public List<List<List<Vector3>>> pointsList;

        private void Start()
        {
            // MakeVisArr();

            SetPlotData(dimension);

            SetVisibilities();
        }

        public void SetPlotData(int dimension_internal)
        {
            //Load plot data
            pointsList = GivePlotData(dimension_internal);

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

                    var lineRenderer = contObj.GetComponent<VolumetricLines.VolumetricLineStripBehavior>();

                    // Adjust styling of the LineRenderer
                    lineRenderer.LineWidth = 0.005f;
                    lineRenderer.LightSaberFactor = 1.0f;

                    //Set vertices of the LineRenderer according to plotData for current layer and simulation run
                    lineRenderer.UpdateLineVertices(pointsList[i][j].ToArray());
                }
            }
        }
        
        public List<List<List<Vector3>>> GivePlotData(int dimension_internal)
        {
            List<List<List<Vector3>>> plotData = Reader.GiveDataList(dimension_internal);

            return plotData;
        }
        
        public void SetVisibilities(bool[] layerVisibilitiesArr, bool[] simRunVisibilitiesArr)
        {
            if (layerVisibilitiesArr.Length != 4 || simRunVisibilitiesArr.Length != 7)
            {
                Debug.LogError("CreatePlot.SetVisibilities: Incorrect length at parameter arrays");
                return;
            }

            layerVisibilities = layerVisibilitiesArr;
            simRunVisibilities = simRunVisibilitiesArr;
            
            SetVisibilities();
        }

        public void SetVisibilities()
        {
            for (var i = 0; i < 4; i++)
            {
                var layerObj = gameObject.transform.Find(layers[i]).gameObject;

                for (var j = 0; j < 7; j++)
                {
                    var simRunObj = layerObj.transform.Find("SimRun_" + i + "_" + j).gameObject;

                    simRunObj.SetActive(simRunVisibilities[j]);
                }

                layerObj.SetActive(layerVisibilities[i]);
            }
        }

        public void SetVisibilityOfSingleRun(int runNum, bool val)
        {
            simRunVisibilities[runNum] = val;

            SetVisibilities();
            //SetVisibilities(layerVisibilities, simRunVisibilities);
        }

        // public void MakeVisArr()
        // {
        //     layerVisibilities = new[] { pressure, temperature, water, meteorite };
        //     
        //     simRunVisibilities = new[]
        //         { simRun0Vis, simRun1Vis, simRun2Vis, simRun3Vis, simRun4Vis, simRun5Vis, simRun6Vis };
        // }

        public void MakeVisArr(bool[] layerVisibilitiesArr, bool[] simRunVisibilitiesArr)
        {
            if (layerVisibilitiesArr.Length != 4 || simRunVisibilitiesArr.Length != 7)
            {
                Debug.LogError("CreatePlot.MakeVisArr: Incorrect length at parameter arrays");
                return;
            }

            layerVisibilities = layerVisibilitiesArr;

            simRunVisibilities = simRunVisibilitiesArr;
        }


        // //Method to get a specific point value from the graph
        // public Vector3 GetPointValue(int graphIndex, int pointIndex)
        // {
        //     return pointsList[graphIndex][pointIndex];
        // }

        // //Method to select a graph by its index
        // public void SelectGraph(int index)
        // {
        //     selectedGraphIndex = index;
        // }


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