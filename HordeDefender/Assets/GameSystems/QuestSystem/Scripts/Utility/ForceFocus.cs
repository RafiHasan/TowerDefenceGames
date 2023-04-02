using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceFocus : MonoBehaviour
{
    
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    GameObject pointer;
    void Start()
    {
        m_EventSystem = FindObjectOfType<EventSystem>();
        pointer = QuestManager.Instance.GetHeighlighter(transform);
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            GrapicalItemCheck();
            PhysicalItemCheck();
        }
    }

    private void PhysicalItemCheck()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                m_PointerEventData = new PointerEventData(m_EventSystem);
                m_PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                m_EventSystem.RaycastAll(m_PointerEventData, results);
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == gameObject)
                    {
                        ExecuteEvents.Execute(gameObject, m_PointerEventData, ExecuteEvents.pointerClickHandler);
                        ExecuteEvents.Execute(gameObject, m_PointerEventData, ExecuteEvents.selectHandler);
                        //GameEventManager.Instance.Navigate();
                    }
                }
            }
        }
    }

    private void GrapicalItemCheck()
    {
        GraphicRaycaster m_Raycaster;
        m_Raycaster = GetComponentInParent<GraphicRaycaster>();

        if (m_Raycaster == null)
            return;

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject==gameObject)
            {
                ExecuteEvents.Execute(gameObject, m_PointerEventData, ExecuteEvents.submitHandler);
                //GameEventManager.Instance.Navigate();
            }
        }
    }

    private void OnDisable()
    {
        Destroy(pointer);
        Destroy(this);
    }
    
}
