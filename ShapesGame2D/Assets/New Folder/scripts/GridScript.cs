using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public class GridScript : MonoBehaviour
{
    public GameObject gridShape;
    GeneralScript generalScript;
    public int rows;
    public int columns;




    private bool needsRebuild = false;

    private void Start()
    {
        generalScript = GameObject.Find("GeneralScriptObj").GetComponent<GeneralScript>();
    }

    void OnValidate()
    {
        // Mark the grid for rebuild
        needsRebuild = true;
    }

    void Update()
    {
        if (needsRebuild)
        {
            RebuildGrid();
            needsRebuild = false;
        }
        transform.position = new Vector3((columns) / -2, (rows) / -2, 0);

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

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                position.x = c;
                position.y = r;

                // Create a new grid element

                GameObject grid = Instantiate(gridShape, transform.position + position, Quaternion.identity);

                grid.GetComponentInChildren<Animator>().SetTrigger("Spawn");

                // Parent the grid element
                grid.transform.SetParent(transform, true);

            }
        }
    }

    void ClearGrid()
    {
        // Clear all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    public void AddLayer(int thickness)
    {

        if (gridShape == null)
        {
            Debug.LogWarning("Grid shape is not assigned!");
            return;
        }

        for (int T = 0; T < thickness; T++)
        {

            Vector3 pos = Vector3.zero;
            pos.y = rows;
            int i = 0;

            for (i = 0; i <= columns; i++)
            {
                pos.x = i;
                GameObject grid = Instantiate(gridShape, transform.position + pos, Quaternion.identity);
                grid.transform.GetChild(0).localScale = Vector3.zero;

                // grid.GetComponentInChildren<Animator>().SetTrigger("Spawn");

                // Parent the grid element
                grid.transform.SetParent(transform, true);
            }

            for (int x = 0; x < rows; x++)
            {
                //pos.x--;
                pos.y--;

                GameObject grid = Instantiate(gridShape, transform.position + pos, Quaternion.identity);
                grid.transform.GetChild(0).localScale = Vector3.zero;

                // grid.GetComponentInChildren<Animator>().SetTrigger("Spawn");

                // Parent the grid element
                grid.transform.SetParent(transform, true);
            }

            if (columns % 2 == 0)
            {
                for (int y = 0; y < transform.childCount; y++)
                {
                    Transform child = transform.GetChild(y);
                    Vector3 childPos = child.position;

                    // Check if the child is on the edges
                    if (childPos.x == -columns / 2 || childPos.x == columns / 2 ||  // Left or right edge
                        childPos.y == -rows / 2 || childPos.y == rows / 2)          // Top or bottom edge
                    {
                        Debug.Log(childPos);

                        // Trigger the animation
                        child.GetComponentInChildren<Animator>().SetTrigger("Spawn2");
                    }
                }
            }

            columns++;
            rows++;


            if (columns % 2 == 0)
            {
                for (int y = 0; y < transform.childCount; y++)
                {
                    Transform child = transform.GetChild(y);
                    Vector3 childPos = child.position;

                    // Check if the child is on the edges
                    if (childPos.x == (-columns / 2) + 1 || childPos.x == columns / 2 ||  // Left or right edge
                        childPos.y == (-rows / 2) + 1 || childPos.y == rows / 2)          // Top or bottom edge
                    {
                        Debug.Log(childPos);

                        // Trigger the animation
                        child.GetComponentInChildren<Animator>().SetTrigger("Spawn2");
                    }
                }
            }
            generalScript.GenerateBounds(columns, rows, ref generalScript.LowBound, ref generalScript.UpBound, ref generalScript.leftBound, ref generalScript.rightBound);
        }
    }


    IEnumerator Add(int thickness)
    {
        for (int j = 0; j < thickness; j++)
        {
            AddLayer(1);

            yield return new WaitForSeconds(0.5f);

        }
    }
}
