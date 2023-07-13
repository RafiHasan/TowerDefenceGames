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
        
        if(ParentCard.stats.Stats[Index].ID==StatID.SLOWPERCENTAGE)
        {
            InputField.text = ((1-ParentCard.stats.Stats[Index].value)*100).ToString();
        }
        else if (ParentCard.stats.Stats[Index].ID == StatID.DAMAGEPERSECOND)
        {
            InputField.text = (-ParentCard.stats.Stats[Index].value).ToString();
        }
        else
        {
            InputField.text = ParentCard.stats.Stats[Index].value.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

        try
        {
            if (ParentCard.stats.Stats[Index].ID == StatID.SLOWPERCENTAGE)
            {
                ParentCard.stats.Stats[Index].value = (1- float.Parse(InputField.text) / 100);
                if (ParentCard.stats.Stats[Index].value < 0)
                {
                    ParentCard.stats.Stats[Index].value = 0;
                    InputField.text = ((1 - ParentCard.stats.Stats[Index].value) * 100).ToString();
                }
                if (ParentCard.stats.Stats[Index].value > 1.0f)
                {
                    ParentCard.stats.Stats[Index].value = 1.0f;
                    InputField.text = ((1 - ParentCard.stats.Stats[Index].value) * 100).ToString();
                }

            }
            else if (ParentCard.stats.Stats[Index].ID == StatID.DAMAGEPERSECOND)
            {
                ParentCard.stats.Stats[Index].value = -float.Parse(InputField.text);
            }
            else
            {
                ParentCard.stats.Stats[Index].value = float.Parse(InputField.text);
            }
        }
        catch
        {
            
        }
    }
}
