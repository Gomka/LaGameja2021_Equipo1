using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI npcName, dialogueText;

    private DialogueNode currentNode;

    private AudioSource audioSource;

    [SerializeField] private Animator animator;

    public bool isInteracting = false;

    private MovementController movement;

    private List<Coroutine> dialogueCoroutine;

    [SerializeField] private Image portrait;

    [SerializeField] private Button defaultButton, bOption1, bOption2, bOption3;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueCoroutine = new List<Coroutine>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if(!isInteracting){

            animator.SetBool("IsOpen", true);

            isInteracting=true;
            movement.StopMovement();
            movement.enabled = false;

            currentNode = dialogue.FirstNode;
            npcName.text = dialogue.NPCName;
            audioSource.clip = dialogue.NPCVoice;
            portrait.sprite = dialogue.Portrait;

            EnableButtons();

            dialogueCoroutine.Add(StartCoroutine(TypeSentence(currentNode.dialogueLine)));
        }
    }

    public void DisplayNextSentence(int index)
    {
        if(isInteracting)
        {
            if (currentNode == null || currentNode.Choices.Length == 0)
            {
                StartCoroutine(EndDialogue());
                return;
            }

            currentNode = currentNode.Choices[index].ChoiceNode;
            EnableButtons();

            dialogueCoroutine.Add(StartCoroutine(TypeSentence(currentNode.dialogueLine)));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            audioSource.pitch = Random.Range(-0.5f, 3);
            audioSource.Play();
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator EndDialogue()
    {
        if(isInteracting)
        {
            animator.SetBool("IsOpen", false);
            movement.enabled = true;
            currentNode = null;
            foreach (Coroutine co in dialogueCoroutine) {
                StopCoroutine(co);
            }
            dialogueCoroutine.Clear();
            yield return new WaitForSeconds(0.5f);
            dialogueText.text = "";
            isInteracting = false;
        }
    }

    private void EnableButtons()
    {
        defaultButton.gameObject.SetActive(true);
        defaultButton.Select();
        bOption1.gameObject.SetActive(false);
        bOption2.gameObject.SetActive(false);
        bOption3.gameObject.SetActive(false);
        
        if (currentNode.Choices.Length != 0)
        {
            defaultButton.gameObject.SetActive(false);

            bOption1.gameObject.SetActive(true);
            bOption1.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentNode.Choices[0].ChoicePreview;
            bOption1.Select();

            if (currentNode.Choices.Length > 1)
            {
                bOption2.gameObject.SetActive(true);
                bOption2.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentNode.Choices[1].ChoicePreview;
            }
            if (currentNode.Choices.Length > 2)
            {
                bOption3.gameObject.SetActive(true);
                bOption3.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentNode.Choices[2].ChoicePreview;
            }
        }
    }

    public void EndConversation()
    {
        StartCoroutine(EndDialogue());
    }

    private void Awake()
    {
        movement = FindObjectOfType<MovementController>();
    }
}
