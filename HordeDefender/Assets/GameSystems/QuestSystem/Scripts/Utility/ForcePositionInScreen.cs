using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePositionInScreen : MonoBehaviour
{
    [SerializeField] private Vector3 OriginalPosition;
    [SerializeField] Vector2 screenpos;
    [SerializeField] Vector2 screenpos2;
    [SerializeField] private Transform looker;
    Plane m_Plane;
    // Start is called before the first frame update
    void Start()
    {
        OriginalPosition = transform.position;
        m_Plane = new Plane(Vector3.up, OriginalPosition);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        screenpos=Camera.main.WorldToScreenPoint(OriginalPosition);

        


        screenpos2 = screenpos;

        if (screenpos.x<0 || screenpos.y<0 || screenpos.x>Screen.width||screenpos.y>Screen.height)
        {
            if (screenpos.x < 0)
            {
                screenpos.x = 0;
            }
            if (screenpos.y < 0)
            {
                screenpos.y = 0;
            }
            if (screenpos.x > Screen.width)
            {
                screenpos.x = Screen.width;
            }
            if (screenpos.y > Screen.height- 350)
            {
                screenpos.y = Screen.height-350;
            }
            Ray ray = Camera.main.ScreenPointToRay(screenpos);
            float enter = 0.0f;
            if (m_Plane.Raycast(ray, out enter))
            {
                transform.position = ray.GetPoint(enter);
            }
            /*transform.position = Camera.main.ScreenToWorldPoint(screenpos);
            transform.position = new Vector3(transform.position.x,OriginalPosition.y,transform.position.z);*/
            looker.LookAt(transform.position+(transform.position- OriginalPosition).normalized);
        }
        else
        {
            transform.position = OriginalPosition;
            looker.localRotation = Quaternion.Lerp(looker.localRotation, Quaternion.identity,Time.deltaTime*10);
        }
    }
}
