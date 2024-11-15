using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CharacterContainer : ScriptableObject
{
    public List<CharacterProperties> characters = new();

    public CharacterProperties GetCharacter(CharacterKey key)
    {
        CharacterProperties character = characters.Find(obj => obj.characterName == key);

        return character ?? characters[0];
    }
}
