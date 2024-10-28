using UnityEngine;
using System.Reflection;

public class ReiniciarNivel : MonoBehaviour
{
    public VidaJogador vidaJogador; 

    public void OnReiniciarNivel()
    {
        if (vidaJogador != null)
        {
            
            var currentHealthProperty = vidaJogador.GetType().GetProperty("CurrentHealth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (currentHealthProperty != null && currentHealthProperty.CanWrite)
            {
                currentHealthProperty.SetValue(vidaJogador, 0f); 
            }

            
        }
    }
}
