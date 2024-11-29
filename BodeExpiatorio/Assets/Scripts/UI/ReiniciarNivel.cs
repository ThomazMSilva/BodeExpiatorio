using UnityEngine;

public class ReiniciarNivel : MonoBehaviour
{
    public VidaJogador vidaJogador;
    public JogadorReference jogador;

    public void OnReiniciarNivel()
    {
        if (vidaJogador == null) return;

        vidaJogador.DamageHealth("Reiniciou nivel"); //esse metodo, DamageHealth(), mata o jogador instantâneamente se n botar um valor de dano.
    }

    public void OnReturnToLastCheckpoint()
    {
        if (Confessionario.ultimoAtivo != null)
        {
            Transform respawnPoint = Confessionario.ultimoAtivo.GetRespawnPoint();
            jogador.SetPosition(respawnPoint.position);
            vidaJogador.DamageHealth("reiniciou nivel");
        }
        else
        {
            Debug.LogWarning("nenhum confessionario encontrado");
        }
    }
}