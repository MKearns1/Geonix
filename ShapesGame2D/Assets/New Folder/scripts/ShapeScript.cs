using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShapeScript : MonoBehaviour
{
    public Vector3 Movement = Vector3.zero;

    float time;
    public float LifeTime;
    float LifeLeft;
    Vector3 SpawnPos = new Vector3(0,-8,0);
    public Vector3 TargetPos = Vector3.zero;
    public bool isMoving;
    public float MoveSpeed;
    public Animator animator;

    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Up;
    public KeyCode Down;

    public int CurrentShapesindex;
    public int ShapeTypeIndex;

    public string ShapeType;
    public Color ShapeColour;
    Light2D Light;
    float maxLightIntensity;

    public GameObject SoundObj;
    public GameObject ParticlesObj;
    public GeneralScript Generals;

    public AudioClip movementSound;

    float minSpeed = 0.1f;
    float maxSpeed = 5;

    public float PlayerSpeed;
    private MovementBehaviour movementBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float RandomXPos = Random.Range(-16,16);
        SpawnPos.x = RandomXPos;
        transform.position = SpawnPos;
        animator = GetComponent<Animator>();
        Generals = GameObject.Find("GeneralScriptObj").GetComponent<GeneralScript>();

        GetComponent<SpriteRenderer>().color = ShapeColour;
        Light = GetComponentInChildren<Light2D>();
        Color.RGBToHSV(ShapeColour, out float h, out float s, out float v);
        ShapeColour = Color.HSVToRGB(h, 1, v);
        Light.color = ShapeColour;

        maxLightIntensity = Light.intensity;
        LifeLeft = LifeTime;
        TargetPos = transform.position;
        
        switch(ShapeType)
        {
            case "Square":
                movementBehaviour = gameObject.AddComponent<LinearMovement>();
                break;
            case "Circle":
                movementBehaviour = gameObject.AddComponent<RollingMovement>();
                //Physics2D.IgnoreLayerCollision(this.gameObject.layer,)
               // GetComponent<Rigidbody2D>().
                break;
            case "Triangle":
                movementBehaviour = gameObject.AddComponent<SnakeMovement>();
                movementBehaviour.SetStartDirection(this);
                break;
            case "Hexagon":
                movementBehaviour = gameObject.AddComponent<LinearMovement>();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement = Vector3.zero;

        time += Time.deltaTime;

        LifeLeft -= Time.deltaTime;
        //transform.localScale = Vector3.one * (LifeLeft/LifeTime);

        GetComponentInChildren<TextMeshPro>().text = Mathf.Round(LifeLeft).ToString();
        Light.intensity = LifeLeft * (maxLightIntensity / LifeTime);

        animator.speed = Mathf.Clamp(LifeTime / LifeLeft, minSpeed, maxSpeed);

        if (LifeLeft < 0 )
        {
            if (Generals.Lives >= 1) Generals.GenerateNewShape(CurrentShapesindex, Random.Range(0, 4));
            Generals.Lives--;
            GameObject Particles = Instantiate(ParticlesObj, transform.position, Quaternion.identity);
            Particles.GetComponent<ParticleScript>().ParticleColor = ShapeColour;
            Particles.GetComponent<ParticleScript>().EmissionCount = 20;
            Destroy(gameObject);
        }

        //if (Input.GetKeyDown(Left) && transform.position.x > -15)
        //{

        //    Movement += Vector3.left * PlayerSpeed; PlaySound();
        //    animator.SetTrigger("Move");
        //    movementBehaviour.Move(this, Vector3.left);

        //}
        //if (Input.GetKeyDown(Right) && transform.position.x < 14)
        //{
        //    Movement += Vector3.right * PlayerSpeed; PlaySound();
        //    animator.SetTrigger("Move");


        //}
        //if (Input.GetKeyDown(Down) && transform.position.y > -7)
        //{
        //    Movement += Vector3.down * PlayerSpeed; PlaySound();
        //    animator.SetTrigger("Move");


        //}
        //if (Input.GetKeyDown(Up) && transform.position.y < 7)
        //{
        //    Movement += Vector3.up * PlayerSpeed; PlaySound();
        //    animator.SetTrigger("Move");


        //}

        movementBehaviour.Move(this);

       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {

        }
    }


    public void PlaySound()
    {
        GameObject sound = Instantiate(SoundObj, transform.position, Quaternion.identity);
        sound.GetComponent<SoundScript>().clip = movementSound;
        sound.GetComponent<SoundScript>().volume = 0.05f;
    }
}
