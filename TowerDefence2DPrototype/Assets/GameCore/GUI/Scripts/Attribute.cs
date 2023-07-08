using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute : MonoBehaviour
{
    public int Index;
    public UnitCard ParentCard;

    public TMPro.TMP_Text Name;
    public TMPro.TMP_InputField InputField;

    // Start is called before the first frame update
    void Start()
    {
        Name.text = ParentCard.stats.Stats[Index].ID.ToString();
        InputField.text = ParentCard.stats.Stats[Index].value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ParentCard.stats.Stats[Index].value = float.Parse(InputField.text);
    }
}
