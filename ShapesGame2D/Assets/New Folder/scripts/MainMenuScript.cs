using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    GameObject TitleText;
    GameObject StartText;
    GameObject HowToPlayText;
    GameObject BackButton;
    GameObject HowToPlayMenu;
    GameObject MainMenu;
    GameObject Background;
    GameObject Fade;
    GameObject BorderCanvas;
   

    GameObject[] UIElements;
    float Hue=1;
    float Saturation = 1;
    float BackgroundColourChangeRate = 0.15f;
    bool start =false;
    bool startNow = false;
    Color fadecolor = Color.black;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TitleText = GameObject.Find("Title");
        StartText = GameObject.Find("StartButton");
        HowToPlayText = GameObject.Find("HowToPlayButton");
        BackButton = GameObject.Find("BackButton");
        HowToPlayMenu = GameObject.Find("HowToPlayMenu");
        MainMenu = GameObject.Find("MainMenu");
        Background = GameObject.Find("BG");
        Fade = GameObject.Find("Fade");
        BorderCanvas = GameObject.Find("Canvas(Border)");
        
       // Background.GetComponent<RectTransform>().sizeDelta = Camera.main.;
        RectTransform rt = Background.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        RectTransform frt = Fade.GetComponent<RectTransform>();
        frt.sizeDelta = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        MainMenu.SetActive(true);
        HowToPlayMenu.SetActive(false);

        StartText.GetComponent<Button>().onClick.AddListener(OnStartPressed);
        HowToPlayText.GetComponent<Button>().onClick.AddListener(OnHowToPlayPressed);
        BackButton.GetComponent<Button>().onClick.AddListener(OnBackPressed);

        fadecolor.a = 0;
        Fade.GetComponent<Image>().color = fadecolor;

        for (int g=0; g < BorderCanvas.transform.childCount; g++)
        {
            RectTransform border = BorderCanvas.transform.GetChild(g).GetComponent<RectTransform>();
            if (BorderCanvas.transform.GetChild(g).name == "Border1") border.sizeDelta = new Vector2(Camera.main.pixelWidth, 200);
            if(BorderCanvas.transform.GetChild(g).name == "Border2") border.sizeDelta = new Vector2(200, Camera.main.pixelWidth);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Hue += BackgroundColourChangeRate * Time.deltaTime;
        if (Hue > 1f) Hue -= 1f;
        Camera.main.backgroundColor = Color.HSVToRGB(Hue, Saturation, 0.25f);
        TitleText.GetComponent<Text>().color = Color.HSVToRGB(Hue, .5f, 0.75f);
        StartText.GetComponentInChildren<Text>().color = Color.HSVToRGB(Hue, 0, 0.65f);
        HowToPlayText.GetComponentInChildren<Text>().color = Color.HSVToRGB(Hue, 0, 0.65f);
        Background.GetComponent<Image>().color = Color.HSVToRGB(Hue, Saturation, 0.15f);


        foreach(GameObject ss in GameObject.FindGameObjectsWithTag("Border"))
        {
            ss.GetComponent<Image>().color = Color.HSVToRGB(Hue, Saturation, 0.25f);
        }

        if(start)
        {
            fadecolor.a += 2 * Time.deltaTime;
            Fade.GetComponent<Image>().color =fadecolor;
        }
        if (fadecolor.a > 2)
        {
            SceneManager.LoadScene("SampleScene 2"); ;
        }
       // Debug.Log(SceneManager.GetActiveScene().name);

    }

    void OnStartPressed()
    {
        start = true;
        //SceneManager.LoadScene(0);
    }
    void OnHowToPlayPressed()
    {
        Debug.Log("sdsd");
        HowToPlayMenu.SetActive(true);
        MainMenu.SetActive(false);
    }

    void OnBackPressed()
    {
        HowToPlayMenu.SetActive(false);
        MainMenu.SetActive(true);

    }


    private void OnDestroy()
    {

       // foreach (GameObject go in UIElements)
        {
            // Unsubscribe from the onClick event to prevent memory leaks
          //  if (go != null)
            {
               // go.GetComponent<Button>().onClick.RemoveListener(OnButtonPressed);
            }
        }
    }

}
