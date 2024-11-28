using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterExpression
{
    happy,
    neutral,
    upset
}

public enum CharacterPosition
{
    left,
    center,
    right
}

[System.Serializable]
public class Dialogue
{
    [TextArea] public string text;
    public float typingDelaySpeed;
    [field: SerializeField] public EventReference DialogueEventReference;
    public bool hasBackground;
    public CharacterExpression characterMood;
    public CharacterKey characterKey;
    public CharacterPosition characterPosition;
}

public class DialogueBehaviour : MonoBehaviour
{
    [SerializeField] CharacterContainer characterManager;

    public List<Dialogue> dialogueList = new();

    private int currentIndex;

    private bool isTextFinished;

    private TextMeshProUGUI textMeshPro;
    private UnityEngine.UI.Image characterA_IMG;
    private UnityEngine.UI.Image characterB_IMG;
    private ImageFade imageFade;
    private ImageFade imageFadeB;
    private AudioSource audioSource;
    public bool disablesSelfOTF;
    public bool hidesImageOnDisable;
    public UnityEvent OnTextFinished;
    public bool LevelUpAfter;


    private void Awake()
    {
        CanvasReference reference = FindAnyObjectByType<CanvasReference>();
        textMeshPro = reference.tmp;
        characterA_IMG = reference.image;
        characterB_IMG = reference.imageB;
        imageFade = characterA_IMG.GetComponent<ImageFade>();
        imageFadeB = characterB_IMG.GetComponent<ImageFade>();
        audioSource = reference.audioSource;
    }

    private void OnEnable()
    {
        TextMeshProHandler.OnTextClickedEvent += ChangeText;

        textMeshPro.gameObject.SetActive(true);
        characterA_IMG.gameObject.SetActive(true);
        characterB_IMG.gameObject.SetActive(true);

        currentIndex = 0;
        //Debug.Log($"Chamou {gameObject.name}");
        StartTypingCurrentDialogue();

        //if (LevelUpAfter) OnTextFinished.AddListener(GameManager.Instance.LevelManager.IncreaseLevel);
    }

    private void OnDisable()
    {
        TextMeshProHandler.OnTextClickedEvent -= ChangeText;

        //textMeshPro.gameObject.SetActive(false);
        textMeshPro.text = "";


        if (hidesImageOnDisable)
        {
            imageFade.DisableImage();
            imageFadeB.DisableImage();
        }

        //if (LevelUpAfter) OnTextFinished.RemoveListener(GameManager.Instance.LevelManager.IncreaseLevel);
    }

    public void ChangeText()
    {
        StopAllCoroutines();

        if (!isTextFinished)
        {
            textMeshPro.text = (dialogueList[currentIndex].hasBackground ? "<font=\"LiberationSans SDF\"> <mark=#000000 padding=10,20,5,5>" : "") + dialogueList[currentIndex].text;
            isTextFinished = true;
        }

        else
        {
            textMeshPro.text = "";

            if (currentIndex + 1 < dialogueList.Count)
            {
                currentIndex++;

                StartTypingCurrentDialogue();
            }
            else
            {
                FinishDialogue(disablesSelfOTF);
            }


        }
    }

    public void FinishDialogue(bool b)
    {
        OnTextFinished?.Invoke();
        currentAudioEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (b) gameObject.SetActive(false);
    }

    private IEnumerator TypeText
        (string newText,
        float typingInterval = 0.1f, 
        bool hasBackground = false, 
        CharacterKey characterKey = CharacterKey.PenitentePadrao,
        CharacterExpression characterMood = CharacterExpression.happy, 
        CharacterPosition characterPosition = CharacterPosition.left
        )
    {
        isTextFinished = false;
        textMeshPro.text = hasBackground ? "<font=\"Girassol Regular SDF\"> <mark=#000000 padding=10,20,5,5>" : "";
        WaitForSeconds interval = new(typingInterval);

        ChangeSprite(characterKey, characterMood, characterPosition);

        //audioEventReference.start();

        for (int i = 0; i < newText.Length; i++)
        {
            if (newText[i] == '<')
            {
                textMeshPro.text += FullTag(newText, ref i);
            }
            else
            {
                /*if (newText[i] != ' ')
                    PlaySound(characterKey);*/
                textMeshPro.text += newText[i];
            }

            yield return interval;
        }
        isTextFinished = true;

        yield return null;
    }

    EventInstance currentAudioEventInstance;

    private void StartTypingCurrentDialogue()
    {
        //Debug.Log($"Comecou a digitar uma fala de {gameObject.name}");
        Dialogue currentDialogue = dialogueList[currentIndex];

        if (!currentDialogue.DialogueEventReference.IsNull) 
        {
            if (currentAudioEventInstance.isValid())
                currentAudioEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentAudioEventInstance = AudioManager.Instance.CreateEventInstance(currentDialogue.DialogueEventReference);
            currentAudioEventInstance.start();
        }

        characterA_IMG.color = currentDialogue.characterPosition != CharacterPosition.left ? Color.gray : Color.white; ;

        characterB_IMG.color = currentDialogue.characterPosition != CharacterPosition.left ? Color.white : Color.gray;

        StartCoroutine(
            TypeText
            (
                currentDialogue.text,
                currentDialogue.typingDelaySpeed,
                currentDialogue.hasBackground,
                currentDialogue.characterKey,
                currentDialogue.characterMood,
                currentDialogue.characterPosition
            )
        );
    }

    private void PlaySound(CharacterKey charKey)
    {
        CharacterProperties characterToSpeak = characterManager.GetCharacter(charKey);

        //audioSource.clip = currentCharacter.characterVoiceAudioClip;
        audioSource.pitch = 1 + Random.Range(-characterToSpeak.voicePitchVariation, characterToSpeak.voicePitchVariation);
        //Debug.Log($"Played {characterToSpeak.characterVoiceAudioClip}");
        audioSource.PlayOneShot(characterToSpeak.characterVoiceAudioClip);
    }

    string FullTag(string text, ref int index)
    {
        string fullTag = "";

        while (index < text.Length)
        {
            fullTag += text[index];

            if (text[index] == '>')
                return fullTag;

            index++;
        }

        return "";
    }

    private void ChangeSprite(CharacterKey charKey, CharacterExpression charMood, CharacterPosition charPos)
    {
        CharacterProperties characterToChange = characterManager.GetCharacter(charKey);

        //var charAColor = characterA_IMG.color;
        //var charBColor = characterB_IMG.color;

        if (charPos == CharacterPosition.left) 
        { 
            switch (charMood)
            {
                case CharacterExpression.happy:
                    characterA_IMG.sprite = characterToChange.characterHappyIMG;
                    break;
                case CharacterExpression.neutral:
                    characterA_IMG.sprite = characterToChange.characterNeutralIMG;
                    break;
                case CharacterExpression.upset:
                    characterA_IMG.sprite = characterToChange.characterUpsetIMG;
                    break;

                default: break;
            }
            //charAColor.a = 255;
            //charBColor.a = 50;
        }
        else
        {
            switch (charMood)
            {
                case CharacterExpression.happy:
                    characterB_IMG.sprite = characterToChange.characterHappyIMG;
                    break;
                case CharacterExpression.neutral:
                    characterB_IMG.sprite = characterToChange.characterNeutralIMG;
                    break;
                case CharacterExpression.upset:
                    characterB_IMG.sprite = characterToChange.characterUpsetIMG;
                    break;

                default: break;
            }
            //charAColor.a = 50;
            //charBColor.a = 255;

        }
        //characterA_IMG.color = charAColor;
        //characterB_IMG.color = charBColor;
    }
}
