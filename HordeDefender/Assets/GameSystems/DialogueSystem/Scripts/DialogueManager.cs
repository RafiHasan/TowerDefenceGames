using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public List<Dialogue> DialogueList = new List<Dialogue>();
    private int _currentDialogueIndex = 0;

    public event System.Action OnConversationStart;
    public event System.Action OnConversationEnd;
    public event System.Action OnConversationSkip;

    public event System.Action<Dialogue> OnDialogueStart;
    public event System.Action OnDialogueCompleted;
    
    public event System.Action OnDialogueSkip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Inisialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Inisialize()
    {
        OnConversationStart += _ConversationStart;
        OnConversationEnd += _ConversationEnd;
        OnConversationSkip += _ConversationEnd;

        OnDialogueStart += _DialogueStart;
        OnDialogueCompleted += _DialogueNext;
        OnDialogueSkip += _DialogueNext;
    }

    public void AddDialogue(Dialogue dialogue)
    {
        DialogueList.Add(dialogue);
    }

    public void AddConversation(Conversation conversation)
    {
        DialogueList.AddRange(conversation.dialogues);
    }

    public void StartConversation()
    {
        OnConversationStart?.Invoke();
    }

    public void EndConversation()
    {
        OnConversationEnd?.Invoke();
    }

    public void SkipConversation()
    {
        OnConversationSkip?.Invoke();
    }

    public void NextDialogue()
    {
        OnDialogueCompleted?.Invoke();
    }

    public void SkipDialogue()
    {
        OnDialogueSkip?.Invoke();
    }

    private void _ConversationStart()
    {
        Debug.Log("Conversation Started");
        _currentDialogueIndex = 0;
        if (DialogueList.Count > _currentDialogueIndex)
        {
            Dialogue dialogue = DialogueList[_currentDialogueIndex];
            OnDialogueStart?.Invoke(dialogue);
        }
        
    }

    private void _ConversationEnd()
    {
        Debug.Log("Conversation Ended");
        DialogueList.Clear();
        _currentDialogueIndex = 0;
    }

    private void _DialogueStart(Dialogue dialogue)
    {
        Debug.Log(dialogue.Name + ": " + dialogue.Sentence);
    }

    private void _DialogueNext()
    {
        Debug.Log("Dialogue Ended");
        _currentDialogueIndex++;
        if (DialogueList.Count > _currentDialogueIndex)
        {
            Dialogue dialogue = DialogueList[_currentDialogueIndex];
            OnDialogueStart?.Invoke(dialogue);
        }
        else
        {
            EndConversation();
        }
    }
}
