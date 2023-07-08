using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    public GameObject AttributePrefab;
    public StatsComponent stats;
    public Transform content;
    public Sprite sprite;
    public Image image;
    public string Name;
    public TMPro.TMP_Text UnitName;

    public Color color;
    public TMPro.TMP_InputField R;
    public TMPro.TMP_InputField G;
    public TMPro.TMP_InputField B;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0;i<stats.Stats.Count;i++)
        {
            Attribute attribute=Instantiate(AttributePrefab,content).GetComponent<Attribute>();
            attribute.Index = i;
            attribute.ParentCard = this;
        }

        image.sprite = sprite;
        UnitName.text = Name;
        R.text = ((int)(color.r * 255)).ToString();
        G.text = ((int)(color.g * 255)).ToString();
        B.text = ((int)(color.b * 255)).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        color = new Color(int.Parse(R.text)/255f, int.Parse(G.text) / 255f, int.Parse(B.text) / 255f);
        image.color = color;
    }
}
