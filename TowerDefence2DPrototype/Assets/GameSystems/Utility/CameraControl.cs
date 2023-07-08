using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float zoomspeeed = 4;
    public float movespeeed = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.Z))
        {
            Camera.main.orthographicSize -= zoomspeeed * Time.deltaTime;
            if (Camera.main.orthographicSize < 5)
                Camera.main.orthographicSize = 5;
        }
        else if(Input.GetKey(KeyCode.X))
        {
            Camera.main.orthographicSize += zoomspeeed * Time.deltaTime;
            if (Camera.main.orthographicSize > 15)
                Camera.main.orthographicSize = 15;
        }

        

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.position += new Vector3(0, movespeeed, 0) * Time.deltaTime;
            if (Camera.main.transform.position.y > 12)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 12, Camera.main.transform.position.z);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.position -= new Vector3(0, movespeeed, 0) * Time.deltaTime;
            if (Camera.main.transform.position.y < -12)
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,  -12, Camera.main.transform.position.z);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.position -= new Vector3(movespeeed, 0, 0) * Time.deltaTime;
            if (Camera.main.transform.position.x < -8)
                Camera.main.transform.position = new Vector3(-8, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.position += new Vector3(movespeeed, 0,0) * Time.deltaTime;
            if (Camera.main.transform.position.x > 8)
                Camera.main.transform.position = new Vector3(8, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }


    }
}
