using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCounter : MonoBehaviour
{
    public static ZombieCounter Instance;

    private void Awake()
    {
        Instance = this;
    }


    public TMPro.TMP_InputField zombieAmmount;
    public TMPro.TMP_InputField Time;
    public TMPro.TMP_InputField Seed;

    public int ammount;
    public float timar;
    public uint seed;

    // Start is called before the first frame update
    void Start()
    {
        zombieAmmount.text = ammount.ToString();
        Time.text = timar.ToString();
        Seed.text = seed.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ammount = int.Parse(zombieAmmount.text);
        timar = float.Parse(Time.text);
        seed = uint.Parse(Seed.text);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
