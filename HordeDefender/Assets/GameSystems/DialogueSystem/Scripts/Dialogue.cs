using System;

    
[Serializable]
public class Dialogue
{
    public string Name;
    public string Sentence;
    public Dialogue(string name, string sentence)
    {
        Name = name;
        Sentence = sentence;
    }
}