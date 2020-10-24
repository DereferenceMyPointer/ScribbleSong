using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    private Queue<string> sentences;
    public static DialogueManager Instance;
    public bool isOpen;
    public string defaultName = "Speaking";
    public float textSpeed;

    public PlayerMovement controller;
    public Animator animator;
    public TextMeshProUGUI nameSpace;
    public TextMeshProUGUI bodySpace;
    public Animator blackWash;
    public Animator deathText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        sentences = new Queue<string>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isOpen && Input.GetButtonDown("Cancel"))
        {
            EndDialogue();
        } else if (isOpen && (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1")))
        {
            DisplayNext();
        }
    }

    public void StartDialogue(Dialogue d)
    {
        controller.enabled = false;
        controller.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        isOpen = true;
        animator.SetBool("Open", true);
        sentences.Clear();
        foreach (string sentence in d.sentences)
        {
            sentences.Enqueue(sentence);
        }
        nameSpace.text = d.name;
        DisplayNext();
    }

    public void DisplayNext()
    {
        StopAllCoroutines();
        if (sentences.Count > 0)
        {
            bodySpace.text = "";
            StartCoroutine(WriteSentences(sentences.Dequeue()));
        } else
        {
            EndDialogue();
        }
    }

    public IEnumerator WriteSentences(string sentence)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            bodySpace.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void EndDialogue()
    {
        controller.enabled = true;
        isOpen = false;
        animator.SetBool("Open", false);
        sentences.Clear();
    }

    public void FadeToBlack(float seconds)
    {
        blackWash.SetBool("Darken", true);
        StartCoroutine(LightenTimeout(seconds));
    }

    public IEnumerator LightenTimeout(float t)
    {
        yield return new WaitForSeconds(t);
        blackWash.SetBool("Darken", false);
    }

    public void DisplayDeathText()
    {
        deathText.SetBool("Visible", true);
        StartCoroutine(DisableDeathText());
    }

    public IEnumerator DisableDeathText()
    {
        yield return new WaitForEndOfFrame();
        deathText.SetBool("Visible", false);
    }

}
