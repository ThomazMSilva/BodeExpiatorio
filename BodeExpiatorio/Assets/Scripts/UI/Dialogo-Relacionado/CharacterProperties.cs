using UnityEngine;

[System.Serializable]
public class CharacterProperties
{
    public CharacterKey characterName;
    public AudioClip characterVoiceAudioClip;
    [Range(0f, 0.5f)] public float voicePitchVariation;   
    public Sprite
        characterHappyIMG,
        characterNeutralIMG,
        characterUpsetIMG;
}

[System.Serializable]
public enum CharacterKey
{
    Penitente,
    Padre,
    Outro
}

