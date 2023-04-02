using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueVisualizer : MonoBehaviour
{
    public GameObject DialogueBox;
    public TMPro.TMP_Text NameText;
    public TMPro.TMP_Text SentenceText;

    private void Awake()
    {
        DialogueBox.SetActive(false);
        
    }

    private void Start()
    {
        DialogueManager.Instance.OnConversationStart += _ConversationStart;
        DialogueManager.Instance.OnConversationEnd += _ConversationEnd;
        DialogueManager.Instance.OnConversationSkip += _ConversationEnd;

        DialogueManager.Instance.OnDialogueStart += _DialogueStart;
    }

    private void _ConversationStart()
    {
        DialogueBox.SetActive(true);
    }

    private void _ConversationEnd()
    {
        DialogueBox.SetActive(false);
        Debug.Log("Dialogue Visualizer: Conversation Ended");
    }

    private void _DialogueStart(Dialogue dialogue)
    {
        NameText.text = dialogue.Name;
        SentenceText.text = dialogue.Sentence;
    }
}
