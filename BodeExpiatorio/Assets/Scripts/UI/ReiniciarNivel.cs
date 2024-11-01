using UnityEngine;

public class ReiniciarNivel : MonoBehaviour
{
    public VidaJogador vidaJogador; 

    public void OnReiniciarNivel()
    {
        if (vidaJogador == null) return;
    
        vidaJogador.DamageHealth("Reiniciou nivel"); //esse metodo, DamageHealth(), mata o jogador instant�neamente se n botar um valor de dano.
    }

    public void OnReturnToLastCheckpoint() => GameManager.Instance.LoadLastCheckpoint(); //Isso muda a cena pra cena do �ltimo confession�rio ativo
}
