using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

namespace PlotScripts_Volumetric
{
    public class CreatePlot : MonoBehaviour
    {
        public int dimension;

        public int selectedRun;

        public bool[] simRunVisibilities = { true, true, true, true, true, true, true };
        public int visibleLayer;

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

                    var lineRenderer = contObj.GetComponent<VolumetricLineStripBehavior>();

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
            var plotData = Reader.GiveDataList(dimensionInternal);

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
                    layerObj.SetActive(true);
                else
                    layerObj.SetActive(false);
            }
        }

        public void SetVisibilityOfSingleRun(int runNum, bool val)
        {
            simRunVisibilities[runNum] = val;

            SetVisibilities();
        }

        public void MakeVisArr(int visibleLayerInternal, bool[] simRunVisibilitiesArr)
        {
            if (visibleLayerInternal > 3 || visibleLayerInternal < 0 || simRunVisibilitiesArr.Length != 7)
            {
                Debug.LogError("CreatePlot.MakeVisArr: Incorrect length at parameter arrays");
                return;
            }

            visibleLayer = visibleLayerInternal;

            simRunVisibilities = simRunVisibilitiesArr;
        }


        //Method that takes the player's position and a currently selected run as parameters and gives the closest point on the selected run as a Vector3
        public Vector3 GetClosestPointOnSelectedGraph(Vector3 playerPosition, int selectedRunIndex)
        {
            //Check if a run has been selected
            if (selectedRunIndex == -1)
            {
                Debug.LogError("No graph is selected.");
                return Vector2.zero;
            }

            var minDist = float.MaxValue;
            var smallestDistindex = -1;

            for (var i = 0; i < pointsList[visibleLayer][selectedRunIndex].Count; i++)
            {
                var currDist = Vector3.Distance(pointsList[visibleLayer][selectedRunIndex][i], playerPosition);

                if (currDist < minDist)
                {
                    smallestDistindex = i;
                    minDist = currDist;
                }
            }


            //Return the corresponding point
            return pointsList[visibleLayer][selectedRunIndex][smallestDistindex];
        }
    }
}