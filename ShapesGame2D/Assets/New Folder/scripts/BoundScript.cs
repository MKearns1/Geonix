using UnityEngine;

public class BoundScript : MonoBehaviour
{
    public string side;
    GeneralScript general;
    Vector3 Position;
    Vector3 Scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        general = GameObject.Find("GeneralScriptObj").GetComponent<GeneralScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(general.UpBound);
    }


    public void UpdateRBBounds()
    {
        switch (side)
        {
            case "left":
                Position = new Vector3(general.leftBound - 1, -0.5f, 0);
                Scale = new Vector3(1.15f, general.GridWidth + 2, 1);

                break;
            case "right":
                Position = new Vector3(general.rightBound + 1, -0.5f, 0);
                Scale = new Vector3(1.15f, general.GridWidth+2, 1);
                break;
            case "top":
                Position = new Vector3(-0.5f, general.UpBound + 1, 0);
                Scale = new Vector3(general.GridHeight+2, 1.15f, 1);
                break;
            case "bottom":
                Position = new Vector3(-0.5f, general.LowBound - 1, 0);
                Scale = new Vector3(general.GridHeight+2, 1.15f, 1);
                break;
        }

        transform.position = Position;
        transform.localScale = Scale;
    }
}
