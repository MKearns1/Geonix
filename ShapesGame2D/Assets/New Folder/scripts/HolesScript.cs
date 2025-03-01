using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HolesScript : MonoBehaviour
{
    public List<GameObject> Shapes;
    public GameObject ParticleEffectObj;
    public GameObject SoundObj;
    GeneralScript GeneralScriptObj;
    GameObject MovementScriptObj;
    GameObject InnerShape;
    Animator animator;
    bool Filled = false;

    public AudioClip Fillclip;
    public AudioClip Failclip;
    public AudioClip Spawnclip;
    public AudioClip Despawnclip;
    public string ShapeType;
    public Color ShapeColour;
    Light2D Light;

    public float LifeTime;
    float LifeLeft;
    float DetectRange = 0.1f;

    private bool spawnedsound=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GameObject.Find("GeneralScriptObj") != null)
        GeneralScriptObj = GameObject.Find("GeneralScriptObj").GetComponent<GeneralScript>();
        // MovementScriptObj = GameObject.Find("MovementScript");
        InnerShape = transform.GetChild(1).gameObject;
        animator = GetComponent<Animator>();
        animator.SetTrigger("Spawn");
        GameObject Sound = Instantiate(SoundObj, transform.position, Quaternion.identity);
        Sound.GetComponent<SoundScript>().clip = Spawnclip;
        Sound.GetComponent<SoundScript>().volume = 0.25f;

        GetComponent<SpriteRenderer>().color = ShapeColour;
        Light = GetComponentInChildren<Light2D>();
        Color.RGBToHSV(ShapeColour, out float h, out float s, out float v);
        ShapeColour = Color.HSVToRGB(h, 1, v);
        Light.color = ShapeColour;
        InnerShape.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(h,s,0.1f);
        LifeLeft = Random.Range(10,LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(GeneralScriptObj != null) { 
        Shapes = GeneralScriptObj.CurrentShapes;
            for (int i = 0; i < Shapes.Count; i++)
            {
                if (Shapes[i] != null)
                {
                    if (Shapes[i].GetComponent<ShapeScript>().ShapeType == "Circle") { DetectRange = 0.5f; }
                    if (Vector2.Distance(this.transform.position, Shapes[i].transform.position) < DetectRange)
                    {
                        if (ShapeType == Shapes[i].GetComponent<ShapeScript>().ShapeType)
                        {
                            Destroy(Shapes[i]);
                            GeneralScriptObj.Matched(i, transform.position + Vector3.up);
                            GameObject Particles = Instantiate(ParticleEffectObj, transform.position, Quaternion.identity);
                            Particles.GetComponent<ParticleScript>().ParticleColor = ShapeColour;
                            Particles.GetComponent<ParticleScript>().EmissionCount = 40;
                            GameObject Sound = Instantiate(SoundObj, transform.position, Quaternion.identity);
                            Sound.GetComponent<SoundScript>().clip = Fillclip;

                            InnerShape.SetActive(false);
                            animator.SetTrigger("Match");
                            GeneralScriptObj.Gridscript.StartCoroutine("Add", 1);
                            //Destroy(this.gameObject);
                        }
                        else
                        {
                            int x = Shapes[i].GetComponent<ShapeScript>().CurrentShapesindex;
                            Color othercolor = Shapes[i].GetComponent<SpriteRenderer>().color;
                            GeneralScriptObj.multiplier = 0;

                            GameObject Particles = Instantiate(ParticleEffectObj, transform.position, Quaternion.identity);
                            Particles.GetComponent<ParticleScript>().ParticleColor = Color.grey;
                            Particles.GetComponent<ParticleScript>().EmissionCount = 5;
                            GameObject Sound = Instantiate(SoundObj, transform.position, Quaternion.identity);
                            Sound.GetComponent<SoundScript>().clip = Failclip;

                            animator.SetTrigger("Eat");

                            if (GeneralScriptObj.Lives >= 1)
                            {
                                Destroy(Shapes[i]);
                                GeneralScriptObj.GenerateNewShape(x, Random.Range(0, 4));
                            }

                            else if (Shapes[i] != null) Destroy(Shapes[i]); GeneralScriptObj.Lives--;

                        }
                    }
                }
            }
        } 
        
    
        LifeLeft -= Time.deltaTime;

        if (LifeLeft < 0)
        {    
            if (!spawnedsound && animator != null)
            {
                animator.SetTrigger("Despawn");
                GameObject Sound = Instantiate(SoundObj, transform.position, Quaternion.identity);
                Sound.GetComponent<SoundScript>().clip = Despawnclip;
                Sound.GetComponent<SoundScript>().volume = 0.75f;
                spawnedsound = true;
            }
        }

    }


    public Color BlendColors(Color color1, Color color2)
    {
        // Add the RGB components together
        float r = Mathf.Round(Mathf.Clamp01(color1.r + color2.r));
        float g = Mathf.Round(Mathf.Clamp01(color1.g + color2.g));
        float b = Mathf.Round(Mathf.Clamp01(color1.b + color2.b));
        return new Color(r, g, b);
    }



    public void DestroyObject()
    {

        Destroy(gameObject);
    }
    
}
