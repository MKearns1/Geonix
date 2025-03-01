using Unity.VisualScripting;
using UnityEngine;

public class GenerateShapeHoles : MonoBehaviour
{
    public GameObject BoundStartPos, BoundEndPos;
    public GameObject[] ShapeHoles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomXPos, randomYPos;
        randomXPos = Random.Range(BoundStartPos.transform.position.x, BoundEndPos.transform.position.x);
        randomYPos = Random.Range(BoundStartPos.transform.position.y, BoundEndPos.transform.position.y);

        randomXPos = Mathf.Round(randomXPos);
        randomYPos = Mathf.Round(randomYPos);

        //Instantiate(ShapeHoles[], new Vector3(randomXPos, randomYPos), Quaternion.identity);

        //ShapeHoles.transform.position = new Vector3(randomXPos, randomYPos);
    }

    // Update is called once per frame
    void Update()
    {
      
    }


    public void GenerateNewShapeHole()
     {
        float randomXPos, randomYPos;
        randomXPos = Random.Range(BoundStartPos.transform.position.x, BoundEndPos.transform.position.x);
        randomYPos = Random.Range(BoundStartPos.transform.position.y, BoundEndPos.transform.position.y);

        randomXPos = Mathf.Round(randomXPos);
        randomYPos = Mathf.Round(randomYPos);

        int randomNum = Random.Range(0,ShapeHoles.Length);

        Instantiate(ShapeHoles[randomNum], new Vector3(randomXPos, randomYPos), Quaternion.identity);
     }
}
