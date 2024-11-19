using UnityEngine;

public class SuplicioAnsiosoFonte : MonoBehaviour
{
    [SerializeField] SuplicioAnsioso suplicioEfeito;
    [SerializeField] private bool canSpitOut = true;
    [SerializeField] private float spitDamage = 10;
    [SerializeField] private float spitForce = 15;
    [SerializeField] private bool hasBindPlayer;
    [SerializeField] private bool isPlayerInside;
    private Vector3 spitDirection;
    private Jogador _player;

    private void Start()
    {
        suplicioEfeito.OnChangedAttractingState += AttractingStateHasChanged;
        spitDirection.Set(transform.right.x, .5f, 0);

        _player = FindAnyObjectByType<Jogador>();
        _player.Vida.OnPlayerDeath += SetPlayerOutside;
    }

    private void AttractingStateHasChanged()
    {
        if (!hasBindPlayer)
        {
            if (isPlayerInside && suplicioEfeito.IsAttracting) BindPlayer();

            return; 
        }
        
        _player.Movimento.SetPlayerBind(false);
        hasBindPlayer = false;

        if (canSpitOut)
        {
            _player.ApplyDamageEffect(spitDamage, spitDirection * spitForce, 2, "Portal do Desejo");
            isPlayerInside = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //POR QUE TA COLIDINDO 25 BILHOES DE VEZES POR FRAME.???????
        if (other.gameObject.CompareTag("Player"))
        { 
            isPlayerInside = true;
        
            if (!suplicioEfeito.IsAttracting) return;

            if (!hasBindPlayer) BindPlayer();
        }
        
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) SetPlayerOutside();
    }

    private void SetPlayerOutside() => isPlayerInside = false;

    private void BindPlayer()
    {
        //_player.SetPosition(transform.position);
        _player.Movimento.SetPlayerBind(true);
        hasBindPlayer = true;
    }
}
