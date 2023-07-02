using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int instancecount;
    public TMPro.TMP_Text FPS;
    public TMPro.TMP_Text InstanceCounter;

    private void Awake()
    {
        Instance = this;

        DefaultWorldInitialization.Initialize("TopShotz", Application.isEditor);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    float updatecounter;

    public float updatecountertarget;

    // Update is called once per frame
    void Update()
    {
        updatecounter += Time.deltaTime;
        if(updatecounter>= updatecountertarget)
        {
            updatecounter = 0;
            FPS.text = (1.0f / Time.deltaTime).ToString("0.00");  
        }
        InstanceCounter.text = instancecount.ToString();
    }
}
