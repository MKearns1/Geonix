using UnityEngine;
using System.Collections.Generic;

public class MovingGridBG : MonoBehaviour
{
    public GameObject gridShape;
    public GameObject[] Shapes;
    public List<GameObject> columnList;

    public int rows;
    public int columns;

    private bool needsRebuild = false;

    void Start()
    {
        RebuildGrid();
    }

    void Update()
    {
        // Move each column down
        for (int i = 0; i < columnList.Count; i++)
        {
            GameObject column = columnList[i];
            int dir=1;
            if (column != null)
            {
                if(i % 2 == 0) { dir = -1; }
                else { dir = 1; }
                // Move the column downward
                Vector3 currentPosition = column.transform.position;
                currentPosition += Vector3.down * 2 * Time.deltaTime * dir; // Adjust speed as needed
                column.transform.position = currentPosition;

                if (column.transform.localPosition.y < 10)
                {
                    column.transform.position = new Vector3(currentPosition.x, 0, currentPosition.z);
                }
                else if (column.transform.localPosition.y > 18)
                {
                    column.transform.position = new Vector3(currentPosition.x, 0, currentPosition.z);
                }
            }
        }
    }

    void RebuildGrid()
    {
        ClearGrid();
        CreateGrid();
    }

    void CreateGrid()
    {
        if (gridShape == null)
        {
            Debug.LogWarning("Grid shape is not assigned!");
            return;
        }

        Vector3 position = Vector3.zero;

        for (int c = 0; c < columns; c++)
        {
            // Create a new column
            GameObject newColumn = new GameObject("Column" + (c + 1).ToString());
            newColumn.transform.SetParent(transform, true);

            columnList.Add(newColumn);

            // Set the column's initial position
            newColumn.transform.position = new Vector3(c * 2, 0, 0); // Spread columns horizontally

            for (int r = 0; r < rows; r++)
            {
                position.y = r * 2; // Spacing between rows
                position.x = c * 2; // Align with column

                GameObject gridObj;
                // Create a grid element
                //if (Random.Range(0, 30) == 1 ) 
                //{
                //    //gridObj = Instantiate(Shapes[Random.Range(0,Shapes.Length)], transform.position + position, Quaternion.identity);
                //    gridObj = Instantiate(gridShape, transform.position + position, Quaternion.identity);
                //}
               // else { 
                    Color gridcol = Color.black; gridcol.a = 0.3f; 
                    gridObj = Instantiate(gridShape, transform.position + position, Quaternion.identity); gridObj.GetComponent<SpriteRenderer>().color = gridcol; 
                //}

                gridObj.transform.localScale *= 2;

                // Parent the grid element to the column
                gridObj.transform.SetParent(newColumn.transform, true);
            }
        }
    }

    void ClearGrid()
    {
        // Destroy all children of this GameObject
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        columnList.Clear(); // Clear the list to avoid referencing destroyed objects
    }
}
