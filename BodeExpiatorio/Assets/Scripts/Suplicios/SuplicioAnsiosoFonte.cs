using UnityEngine;

public class SuplicioAnsiosoFonte : MonoBehaviour
{
    [SerializeField] SuplicioAnsioso suplicioEfeito;
    [SerializeField] private bool canSpitOut = true;
    [SerializeField] private float spitDamage = 10;
    [SerializeField] private float spitForce = 15;
    private bool hasBindPlayer;
    private bool isPlayerInside;
    private Vector3 spitDirection;
    private Jogador _player;

    private void Start()
    {
        suplicioEfeito.OnChangedAttractingState += AttractingStateHasChanged;
        spitDirection.Set(transform.right.x, .5f, 0);
    }

    private void AttractingStateHasChanged()
    {
        if (!hasBindPlayer)
        { 
            if(isPlayerInside && suplicioEfeito.IsAttracting) BindPlayer();
            return; 
        }
        
        Debug.Log("Destravou o paer");
        _player.Movimento.SetPlayerBind(false);
        hasBindPlayer = false;
        
        if(canSpitOut)
            _player.ApplyDamageEffect(spitDamage, spitDirection * spitForce, 2, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        //POR QUE TA COLIDINDO 25 BILHOES DE VEZES POR FRAME.???????
        if (other.gameObject.CompareTag("Player")) 
        {
            isPlayerInside = true;

            if(_player == null) //por enqnt deixei assim. N funcionaria com multiplayer.
                _player = other.GetComponent<Jogador>();
        }

        if (!suplicioEfeito.IsAttracting) return;
        
        if (!hasBindPlayer) BindPlayer();
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) isPlayerInside = false;
    }

    private void BindPlayer()
    {
        _player.SetPosition(transform.position);
        _player.Movimento.SetPlayerBind(true);
        hasBindPlayer = true;
    }
}
