using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieCounter : MonoBehaviour
{
    public static ZombieCounter Instance;

    private void Awake()
    {
        Instance = this;
    }


    public TMPro.TMP_InputField zombieAmmount;
    public TMPro.TMP_InputField Times;
    public TMPro.TMP_InputField Seed;
    public TMPro.TMP_Text FPS;
    public Toggle pltoggle;

    public int ammount;
    public float timar;
    public uint seed;
    public bool ShowPresentationLayer = false;

    // Start is called before the first frame update
    void Start()
    {
        zombieAmmount.text = ammount.ToString();
        Times.text = timar.ToString();
        Seed.text = seed.ToString();
        pltoggle.onValueChanged.AddListener((bool value) => { ShowPresentationLayer = value; });
    }
    int counter;
    float timecounter = 0;
    float fpsval = 0;
    // Update is called once per frame
    void Update()
    {
        ammount = int.Parse(zombieAmmount.text);
        timar = float.Parse(Times.text);
        seed = uint.Parse(Seed.text);
        

        if (timecounter>=1.0f)
        {
            fpsval = fpsval / counter;
            FPS.text = fpsval.ToString("0.00");
            counter = 0;
            fpsval = 0;
            timecounter = 0;
        }
        timecounter += Time.deltaTime;
        fpsval += (1.0f / Time.deltaTime);
        counter++;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
