using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RandomTest : MonoBehaviour
{
    Unity.Mathematics.Random random;
    // Start is called before the first frame update
    void Start()
    {
        random = new Unity.Mathematics.Random(1);
        random.state = 100;
        
    }
    float counter = 0;

    int step = 10;
    string ranstr = "";
    // Update is called once per frame
    void Update()
    {
        if (Time.time < counter && step>0)
            return;
        step--;

        /*if (step==0)
        //Debug.Log(ranstr);*/
        ranstr += random.NextInt(0, 10);
        counter+=0.1f;
    }
}
