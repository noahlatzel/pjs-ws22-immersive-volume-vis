using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreatePlot : MonoBehaviour
{
    //List of lists to store multiple sets of points, each representing a separate graph
    public List<List<Vector3>> pointsList;

    //Variable to keep track of the selected graph
    public int selectedGraphIndex = -1;

    private Vector3 playerPosition;

    void Start()
    {
        pointsList = JsonReader.Reader.giveVals();
        // Make sure that the pointsList is not empty
        if (pointsList == null || pointsList.Count == 0)
        {
            Debug.LogError("pointsList is empty");
            return;
        }
        
        Debug.Log("CreatePlot:"+pointsList[0][6]);

        // Iterate through each list of points
        for (int j = 0; j < pointsList.Count; j++)
        {
            GameObject newObj = new GameObject();
            newObj.transform.SetParent(gameObject.transform);
            // Create a new LineRenderer component
            LineRenderer lineRenderer = newObj.AddComponent<LineRenderer>();
            lineRenderer.widthMultiplier = 0.01f;
            lineRenderer.useWorldSpace = false;
            
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