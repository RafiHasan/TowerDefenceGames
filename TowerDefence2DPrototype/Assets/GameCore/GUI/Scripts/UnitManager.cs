using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public List<Sprite> spriteList;
    public List<string> unitnames;
    public GameObject unitPrefab;
    public List<UnitCard> unitCards;
    public Transform content;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        foreach(UnitCard unitCard in unitCards)
        {
            Destroy(unitCard.gameObject);
        }
        unitCards.Clear();
    }


    public void AddUnit(StatsComponent statsComponent,Color color)
    {
        UnitCard card=Instantiate(unitPrefab,content).GetComponent<UnitCard>();
        card.sprite = spriteList[unitCards.Count];
        card.Name = unitnames[unitCards.Count];
        card.color = color;
        card.stats = statsComponent;
        unitCards.Add(card);
    }
}
