using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string cenaDeJogoNome; 
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject creditsScreen;

    [SerializeField] private GameObject firstMenuOption;
    [SerializeField] private GameObject firstConfigOption;
    [SerializeField] private GameObject creditsReturnBTN;

    public void AbrirOp��es()
    {
        //painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);

        EventSystem.current.SetSelectedGameObject(firstConfigOption);
    }

    public void Credits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        EventSystem.current.SetSelectedGameObject(creditsScreen.activeSelf ? creditsReturnBTN : firstMenuOption);
    }

    public void FecharOp��es()
    {
        painelOpcoes.SetActive(false);
        //painelMenuInicial.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(firstMenuOption);
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }

    public void Continue()
    {
        GameManager.Instance.Continue();
    }

    public void NewGame()
    {
        GameManager.Instance.NewGame();
    }

    public void BackToLastCheckpoint()
    {
        GameManager.Instance.LoadLastCheckpoint();
    }
}
