using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    public class CreatePlot : MonoBehaviour
    {
        public int dimension;

        public bool[] simRunVisibilities = { true, true, true, true, true, true, true };
        public int visibleLayer = 0;

        //Names of all the layers as array
        private readonly string[] layers = { "Pressure", "Temperature", "Water", "Meteorite" };

        //List of lists to store multiple sets of points, each representing a separate graph
        public List<List<List<Vector3>>> pointsList;

        private void Start()
        {
            SetPlotData(dimension);

            SetVisibilities();
        }

        public void SetPlotData(int dimensionInternal)
        {
            //Load plot data
            pointsList = GivePlotData(dimensionInternal);

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
        
        public List<List<List<Vector3>>> GivePlotData(int dimensionInternal)
        {
            List<List<List<Vector3>>> plotData = Reader.GiveDataList(dimensionInternal);

            return plotData;
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
                
                if (i == visibleLayer)
                {
                    layerObj.SetActive(true);
                }
                else
                {
                    layerObj.SetActive(false);
                }
            }
        }

        public void SetVisibilityOfSingleRun(int runNum, bool val)
        {
            simRunVisibilities[runNum] = val;

            SetVisibilities();
            //SetVisibilities(layerVisibilities, simRunVisibilities);
        }

        public void MakeVisArr(int visibleLayerInternal, bool[] simRunVisibilitiesArr)
        {
            if ((visibleLayerInternal > 3 || visibleLayerInternal < 0) || simRunVisibilitiesArr.Length != 7)
            {
                Debug.LogError("CreatePlot.MakeVisArr: Incorrect length at parameter arrays");
                return;
            }

            visibleLayer = visibleLayerInternal;

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


        //Method that takes the player's position as a parameter and returns the closest point on the selected graph to the player
        // public Vector3 GetClosestPointOnSelectedGraph(Vector3 playerPosition, int selectedGraphIndex)
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