using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector3[] MovementDirection = { Vector3.zero,Vector3.zero };

    public GameObject[] Shapes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MovementDirection[0] += Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MovementDirection[0] += Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MovementDirection[0] += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MovementDirection[0] += Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovementDirection[1] += Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovementDirection[1] += Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovementDirection[1] += Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovementDirection[1] += Vector3.right;
        }
    }

    private void FixedUpdate()
    {


        //Debug.Log(MovementDirection.ToString());
        Shapes[0].transform.position = MovementDirection[0];
        Shapes[1].transform.position = MovementDirection[1];
    }

}
