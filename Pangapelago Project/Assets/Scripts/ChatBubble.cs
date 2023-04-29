using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
     private SpriteRenderer backgroundSpriteRenderer;
     private SpriteRenderer iconSpriteRenderer;
     private TextMeshPro textMeshPro;

     private void Awake()
     {
         backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
         iconSpriteRenderer = transform.Find("Nameplate").GetComponent<SpriteRenderer>();
         textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
     }

     private void Start()
     {
         Setup("Hello World!");
     }
     private void Setup(string text)
     {
        textMeshPro.SetText(text);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(2f, 2f);
        backgroundSpriteRenderer.size = textSize + padding;

     }

 /*   public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        //types each characer 1 by 1
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if(index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
*/
}
