using System.Collections.Generic;
using System.Linq;
using JsonReader;
using UnityEngine;
using UnityEngine.Rendering;

public class CreatePlot : MonoBehaviour
{
    //Variable to keep track of the selected graph
    public int selectedGraphIndex = -1;

    private Vector3 playerPosition;

    //List of lists to store multiple sets of points, each representing a separate graph
    public List<List<Vector3>> pointsList;

    private void Start()
    {
        pointsList = Reader.GiveDataList()[0];
        // Make sure that the pointsList is not empty
        if (pointsList == null || pointsList.Count == 0)
        {
            Debug.LogError("pointsList is empty");
            return;
        }

        Debug.Log("CreatePlot:" + pointsList[0][6]);

        // Iterate through each list of points
        for (var j = 0; j < pointsList.Count; j++)
        {
            var newObj = new GameObject();
            newObj.transform.SetParent(gameObject.transform);
            // Create a new LineRenderer component
            var lineRenderer = newObj.AddComponent<LineRenderer>();
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.useWorldSpace = false;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;

            // Set the LineRenderer's position count
            lineRenderer.positionCount = pointsList[j].Count;
            //Iterate through each point in the list
            lineRenderer.SetPositions(pointsList[j].ToArray());
            // for (int i = 0; i < pointsList[j].Count; i++)
            // {
            // //Set the position of each point on the LineRenderer to match the corresponding point in the list
            //     lineRenderer.SetPosition(i, pointsList[j][i]);
            // }
        }
    }

    //Method to get a specific point value from the graph
    public Vector2 GetPointValue(int graphIndex, int pointIndex)
    {
        return pointsList[graphIndex][pointIndex];
    }

    //Method to select a graph by its index
    public void SelectGraph(int index)
    {
        selectedGraphIndex = index;
    }

    //Method that takes the player's position as a parameter and returns the closest point on the selected graph to the player
    public Vector2 GetClosestPointOnSelectedGraph(Vector3 playerPosition)
    {
        //Check if a graph has been selected
        if (selectedGraphIndex == -1)
        {
            Debug.LogError("No graph is selected.");
            return Vector2.zero;
        }

        //Use LINQ to calculate the distance between each point on the selected graph and the player's position
        var distances = pointsList[selectedGraphIndex].Select(p => Vector3.Distance(p, playerPosition));
        //Find the minimum distance
        var closestPointIndex = distances.ToList().IndexOf(distances.Min());

        //Return the corresponding point
        return pointsList[selectedGraphIndex][closestPointIndex];
    }
}