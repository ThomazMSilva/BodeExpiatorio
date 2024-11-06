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

    /*private void Update()
    {

        if (painelMenuInicial.activeSelf && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstMenuOption);
        }
        else if (painelOpcoes.activeSelf && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstConfigOption);
        }
        else if (creditsScreen.activeSelf && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(creditsReturnBTN);
        }
    }*/

    public void AbrirOpções()
    {

        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstConfigOption);
    }

    public void Credits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        EventSystem.current.SetSelectedGameObject(creditsScreen.activeSelf ? creditsReturnBTN : firstMenuOption);
    }

    public void FecharOpções()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
        //painelMenuInicial.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

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
