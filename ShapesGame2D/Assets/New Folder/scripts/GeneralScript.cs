using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GeneralScript : MonoBehaviour
{
    public List<GameObject> Shapes;
    public GameObject BoundStartPos, BoundEndPos;
    public GameObject[] ShapeHoles;
    public List<GameObject> CurrentHoles;
    public GameObject MultiplierText;

    GameObject MovementScriptObj;
    public List<GameObject> ComponentsInPlay;
    GameObject VHSLines;
    public GridScript Gridscript;

    Camera MainCamera;
    GameObject HUDCanvas;

    Text DeathScreenText;

    public GameObject SoundObj;
    GameObject SoundPlayer;
    public AudioClip DeadMusic;

    public List<GameObject> CurrentShapes;
    KeyCode[] LeftInputs = { KeyCode.A, KeyCode.LeftArrow };
    KeyCode[] RightInputs = { KeyCode.D, KeyCode.RightArrow };
    KeyCode[] DownInputs = { KeyCode.S, KeyCode.DownArrow };
    KeyCode[] UpInputs = { KeyCode.W, KeyCode.UpArrow };

    public float Hue;
    public float Saturation;
    public float Value;
    public float BackgroundColourChangeRate;
    public float VHS_ScrollSpeed;
    float time=0;
    bool canPlaySound = true;
    public AudioClip clip;

    public float maxRotationAngle = 5f; // Maximum rotation angle in degrees
    public float smoothSpeed = 1f;     // Speed of smooth rotation change

    private Quaternion originalRotation;
    private float randomSeed;

    public int Lives;
    public int score;
    public int multiplier;

    public float GridWidth, GridHeight, leftBound, rightBound, UpBound, LowBound;

    public int GridGrowthAmount = 1;
    public int GridGrowthThreshold = 2;
    public int ShapesMatched;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Gridscript = GameObject.Find("GridObject").GetComponent<GridScript>();

        GridWidth = Gridscript.columns;
        GridHeight = Gridscript.rows;

        GenerateBounds(GridWidth, GridHeight, ref LowBound, ref UpBound, ref leftBound, ref rightBound);

        VHSLines = GameObject.Find("VHSLines");
        MovementScriptObj = GameObject.Find("MovementScript");
        GenerateNewShape(0, 0);
        GenerateNewShape(1,1);
        GenerateNewShapeHole(0);
        GenerateNewShapeHole(1);

        originalRotation = Camera.main.transform.rotation;
        randomSeed = Random.Range(0f, 100f);
        DeathScreenText = GameObject.Find("Text").GetComponent<Text>();DeathScreenText.enabled = false;

        MainCamera = Camera.main;
        HUDCanvas = GameObject.Find("Canvas");

    }

    // Update is called once per frame
    void Update()
    {
        GridWidth = Gridscript.columns;
        GridHeight = Gridscript.rows;

        Hue += BackgroundColourChangeRate * Time.deltaTime;
        if (Hue > 1f) Hue -= 1f;
        Camera.main.backgroundColor = Color.HSVToRGB(Hue, Saturation, 0.5f);
        GameObject.Find("Background").GetComponent<SpriteRenderer>().color = Color.HSVToRGB(Hue, Saturation, Value);
        GameObject.Find("Text").GetComponent<Text>().color = Color.HSVToRGB(Hue, Saturation, 0.8f);
        GameObject.Find("LivesText").GetComponent<Text>().text = "Lives: " + Mathf.Clamp(Lives,0,3).ToString();
        GameObject.Find("LivesText").GetComponent<Text>().color = Color.HSVToRGB(Hue, Saturation, 0.8f);
        GameObject.Find("ScoreText").GetComponent<Text>().color = Color.HSVToRGB(Hue, Saturation, 0.8f);
        GameObject.Find("ScoreText").GetComponent<Text>().text = "score: " + (score).ToString();

     
        Camera.main.orthographicSize =  Mathf.Lerp(Camera.main.orthographicSize, 0.5f * GridWidth + 2, .01f);

        //Camera.main.transform.position = PositionAverage() + Vector3.back*10;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Vector3.back + Vector3.back * 10, 0.001f);
       // VHSLines.transform.localPosition += Vector3.down * (VHS_ScrollSpeed);

       // if(VHSLines.transform.localPosition.y < 0) VHSLines.transform.localPosition = Vector3.up * 800;

        time += Time.deltaTime;


        float randomAngle = (Mathf.PerlinNoise(randomSeed, Time.time * smoothSpeed) - 0.5f) * 2f * maxRotationAngle;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(0f, 0f, randomAngle);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);


        for (int i = 0; i < CurrentShapes.Count; i++)
        {
            if (CurrentShapes[i] == null)
            {
                CurrentShapes.RemoveAt(i);

            }
            else
            {
                float clampedxpos = Mathf.Clamp(CurrentShapes[i].transform.position.x, leftBound, rightBound);
                float clampedypos = Mathf.Clamp(CurrentShapes[i].transform.position.y, LowBound, UpBound);

                CurrentShapes[i].transform.position = new Vector3(clampedxpos, clampedypos, 1);

               

            }
        }
        if (Mathf.Round(time) % 1 ==0 && canPlaySound)
        {
            canPlaySound = false;
            //PlaySound();
        }
        if (SoundPlayer == null)
        {
            canPlaySound=true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<AudioSource>().Play();
        }
        if (Input.GetMouseButtonDown(1)) { Gridscript.StartCoroutine("Add",2); }


        if (Lives < 1 && CurrentShapes.Count == 0)
        {
            DeathScreenText.enabled = true;
            GameObject.Find("Music").GetComponent<AudioSource>().clip = DeadMusic;
            if(GameObject.Find("Music").GetComponent<AudioSource>().isPlaying==false)
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        foreach (var shape in CurrentShapes)
        {
            if (shape == null) continue; // Skip null shapes

            bool foundMatch = false; // Reset for each shape

            foreach (var hole in CurrentHoles)
            {
                if (hole == null) continue; // Skip null holes

                // Check if the shape matches the hole
                if (hole.GetComponent<HolesScript>().ShapeType == shape.GetComponent<ShapeScript>().ShapeType)
                {
                    foundMatch = true;
                    break; // Exit inner loop since we found a match
                }
            }

            // If no matching hole was found, generate a new shape hole
            if (!foundMatch)
            {
                GenerateNewShapeHole(shape.GetComponent<ShapeScript>().ShapeTypeIndex);
            }
        }

      
     

    }

    public void GenerateNewShape(int index, int randomNum)
    {
       //Destroy(CurrentShapes[index]);

        CurrentShapes[index] = Instantiate(Shapes[randomNum], Vector3.down * 3, Quaternion.identity);
        ShapeScript script = CurrentShapes[index].GetComponent<ShapeScript>();
        script.Left = LeftInputs[index];
        script.Right = RightInputs[index];
        script.Down = DownInputs[index];
        script.Up = UpInputs[index];

        script.CurrentShapesindex = index;

        bool ShapeReplaced = false;

        for (int i = 0; i < ComponentsInPlay.Count; i++)
        {
            if (ComponentsInPlay[i] == null)
            {
                ComponentsInPlay[i] = CurrentShapes[index];
                ShapeReplaced = true;
                break;
            }
        }
        if (!ShapeReplaced) 
        {
        ComponentsInPlay.Add(CurrentShapes[index]);
        }
    }


    public void GenerateNewShapeHole(int index)
    {
        float randomXPos, randomYPos;
        Vector3 randomPos;
        GameObject NewHole;

        // Track all occupied positions
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        // Populate the HashSet with positions of existing shapes and holes
        foreach (GameObject shape in ComponentsInPlay)
        {
            if (shape != null)
            {
                occupiedPositions.Add(new Vector2(shape.transform.position.x, shape.transform.position.y));
            }
        }
        foreach (GameObject hole in CurrentHoles)
        {
            if (hole != null)
            {
                occupiedPositions.Add(new Vector2(hole.transform.position.x, hole.transform.position.y));
            }
        }

        // Find a random position that is not occupied
        do
        {
            randomXPos = Mathf.Round(Random.Range(leftBound, rightBound));
            randomYPos = Mathf.Round(Random.Range(LowBound, UpBound));
            randomPos = new Vector3(randomXPos, randomYPos, 0);
        }
        while (occupiedPositions.Contains(new Vector2(randomPos.x, randomPos.y))); // Repeat until an unoccupied position is found

        // Instantiate the new shape hole at the valid position
        NewHole = Instantiate(ShapeHoles[index], randomPos, Quaternion.identity);

        // Add the new shape hole to CurrentHoles, replacing null entries or adding to the list
        bool ShapeReplaced2 = false;
        for (int i = 0; i < CurrentHoles.Count; i++)
        {
            if (CurrentHoles[i] == null)
            {
                CurrentHoles[i] = NewHole;
                ShapeReplaced2 = true;
                break;
            }
        }
        if (!ShapeReplaced2)
        {
            CurrentHoles.Add(NewHole);
        }

        // Add the new shape hole to ComponentsInPlay, replacing null entries or adding to the list
        bool ShapeReplaced = false;
        for (int i = 0; i < ComponentsInPlay.Count; i++)
        {
            if (ComponentsInPlay[i] == null)
            {
                ComponentsInPlay[i] = NewHole;
                ShapeReplaced = true;
                break;
            }
        }
        if (!ShapeReplaced)
        {
            ComponentsInPlay.Add(NewHole);
        }
    }


    public void Matched(int index, Vector3 position)
    {
        int randomNum = Random.Range(0, ShapeHoles.Length);


        score += 10 * multiplier;
        multiplier += 1;

        GameObject newMultiplier = Instantiate(MultiplierText, Vector3.zero, Quaternion.identity);

        newMultiplier.transform.SetParent(HUDCanvas.gameObject.transform, false);
       
        newMultiplier.GetComponentInChildren<Animator>().SetTrigger("Spawn");

        TextPosToWorldPos(position, newMultiplier, HUDCanvas, MainCamera);

        Animator animator = newMultiplier.GetComponentInChildren<Animator>();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Layer 0

        float animationLength = stateInfo.length; // Length of the current animation

        newMultiplier.GetComponentInChildren<Text>().color = Color.HSVToRGB(Hue, .75f, 1f);


        newMultiplier.GetComponentInChildren<Text>().text = "x" + multiplier.ToString();

        Destroy(newMultiplier,animationLength);

        GenerateNewShapeHole(randomNum);
        GenerateNewShape(index, randomNum);
    }

    void setTextInvisible(GameObject gameobj)
    {
        gameobj.SetActive(false);
    }

    public void PlaySound()
    {
        SoundPlayer = Instantiate(SoundObj, transform.position, Quaternion.identity);
        Debug.Log("f");
    }


    Vector3 PositionAverage()
    {
        Vector3 average = Vector3.zero;
        foreach (GameObject i in ComponentsInPlay)
        {
            if (i != null)
            {
                average += i.transform.position;
            }
        }
        return average / ComponentsInPlay.Count; 
    }

    void TextPosToWorldPos(Vector3 Target, GameObject text, GameObject canvas, Camera camera)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(Target);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.GetComponent<Canvas>().worldCamera,
            out anchoredPosition
        );

        // Set the text's anchored position
        text.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

    }

    public void GenerateBounds(float Width, float Height, ref float lower, ref float upper, ref float left, ref float right )
    {
        if (Width % 2 != 0)
        {
            lower = Mathf.Floor(Height / -2) + 1;

            upper = Mathf.Floor((Height / 2));

            left = Mathf.Floor(Width / -2) + 1;

            right = Mathf.Floor(Width / 2);

            GameObject.Find("Background").transform.position = new Vector3(0,0,0);
            GameObject.Find("Background").transform.localScale = Vector3.one* (Width + 1);

           
        }
        else
        {
            lower = Mathf.Floor(Height / -2);

            upper = Mathf.Floor((Height / 2)-1);

            left = Mathf.Floor(Width / -2)  ;

            right = Mathf.Floor(Width / 2) - 1;

            GameObject.Find("Background").transform.position = new Vector3(-0.5f,-0.5f,1);
            GameObject.Find("Background").transform.localScale = Vector3.one * (Width+1);


        }

        foreach (GameObject RB_Bound in GameObject.FindGameObjectsWithTag("RB_Border"))
        {
            RB_Bound.GetComponent<BoundScript>().UpdateRBBounds();
        }
    }
}
