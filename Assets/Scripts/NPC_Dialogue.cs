using UnityEngine;
using TMPro;

public class NPC_Dialogue : MonoBehaviour
{
    // Посилання на UI-елементи
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text promptText;
    public TMP_Text pageCounterText;

    // Зміст діалогу
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    // Внутрішній стан
    private bool playerIsInRange = false;
    private bool isDialogueActive = false;
    private int currentLineIndex = 0;

    void Start()
    {
        // Переконаємося, що UI прихований на старті
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (promptText != null) promptText.gameObject.SetActive(false);
        if (pageCounterText != null) pageCounterText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerIsInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isDialogueActive)
                {
                    StartDialogue();
                }
                else
                {
                    EndDialogue(); // Завершуємо діалог в будь-який момент
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) && isDialogueActive)
            {
                DisplayNextLine();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (!isDialogueActive && promptText != null)
            {
                promptText.text = "Розпочати діалог: Натисніть E";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            EndDialogue();
        }
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        currentLineIndex = 0;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (promptText != null)
        {
            promptText.text = "Закінчити діалог: Натисніть E";
        }

        if (pageCounterText != null) pageCounterText.gameObject.SetActive(true);

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            if (dialogueText != null)
            {
                dialogueText.text = dialogueLines[currentLineIndex];
            }
            if (pageCounterText != null)
            {
                pageCounterText.text = $"Наступна фраза на Enter: {currentLineIndex + 1}/{dialogueLines.Length}";
            }
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        if (pageCounterText != null)
        {
            pageCounterText.gameObject.SetActive(false);
        }
    }
}