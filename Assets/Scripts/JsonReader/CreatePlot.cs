using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace JsonReader
{
    public class CreatePlot : MonoBehaviour
    {
        [Tooltip("Show/Hide pressure plot.")] public bool pressure;

        [Tooltip("Show/Hide temperature plot.")] public bool temperature;

        [Tooltip("Show/Hide water plot.")] public bool water;

        [Tooltip("Show/Hide meteorite plot.")] public bool meteorite;
        
        [Tooltip("Choose 1D, 2D or 3D plot.")] public int dimensions;

        [Tooltip("Show/Hide simulation run 0 plot.")] public bool simRun0Vis;
        
        [Tooltip("Show/Hide simulation run 1 plot.")] public bool simRun1Vis;
        
        [Tooltip("Show/Hide simulation run 2 plot.")] public bool simRun2Vis;
        
        [Tooltip("Show/Hide simulation run 3 plot.")] public bool simRun3Vis;
        
        [Tooltip("Show/Hide simulation run 4 plot.")] public bool simRun4Vis;
        
        [Tooltip("Show/Hide simulation run 5 plot.")] public bool simRun5Vis;
        
        [Tooltip("Show/Hide simulation run 6 plot.")] public bool simRun6Vis;

        //Variable to keep track of the selected graph
        public int selectedGraphIndex = -1;

        private Vector3 playerPosition;

        public string[] layers = new[] { "Pressure", "Temperature", "Water", "Meteorite" };

        //List of lists to store multiple sets of points, each representing a separate graph
        public List<List<List<Vector3>>> pointsList;

        private void Start()
        {
            dimensions = 3;

            bool[] layerVisibilities = new[] {pressure, temperature, water, meteorite };

            bool[] simRunVisibilities = new[]
                { simRun0Vis, simRun1Vis, simRun2Vis, simRun3Vis, simRun4Vis, simRun5Vis, simRun6Vis };
            
            //Access first layer of plotData
            pointsList = Reader.GiveDataList();

            // Make sure that the pointsList is not empty
            if (pointsList == null || pointsList.Count == 0)
            {
                Debug.LogError("pointsList is empty");
                return;
            }

            for (int i = 0; i < pointsList.Count; i++)
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

        public void SetVisibilities(bool[] layerVisibilities, bool[] simRunVisibilities)
        {
            if ((layerVisibilities.Length != 4) || (simRunVisibilities.Length != 7))
            {
                Debug.LogError("CreatePlot.SetVisibilities: Incorrect length at parameter arrays");
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                var layerObj = gameObject.transform.Find(layers[i]).gameObject;
                
                for (int j = 0; j < 7; j++)
                {
                    var simRunObj = layerObj.transform.Find("SimRun_" + i + "_" + j).gameObject;
                    
                    simRunObj.SetActive(simRunVisibilities[j]);
                }
                
                layerObj.SetActive(layerVisibilities[i]);
            }
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