using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string cenaDeJogoNome; 
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject MenuGrafico;
    [SerializeField] private GameObject MenuSom;
    [SerializeField] private GameObject MenuAcessibilidade;


    [SerializeField] private GameObject firstMenuOption;
    [SerializeField] private GameObject firstConfigOption;
    [SerializeField] private GameObject creditsReturnBTN;
    [SerializeField] private GameObject MenuGraficoBotao;
    [SerializeField] private GameObject MenuSomBotao;
    [SerializeField] private GameObject MenuAcessibilidadeBot�o;

   private void Update()
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
    }

    public void AbrirMenuGrafico()
    {

        painelOpcoes.SetActive(false);
        MenuGrafico.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(MenuGraficoBotao);
    }
    public void AbrirMenuSom()
    {

        painelOpcoes.SetActive(false);
        MenuSom.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(MenuSomBotao);
    }
    public void AbrirMenuAcessibilidade()
    {

        painelOpcoes.SetActive(false);
        MenuAcessibilidade.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(MenuAcessibilidadeBot�o);
    }
    public void AbrirOp��es()
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

    public void FecharOp��es()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
        //painelMenuInicial.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstMenuOption);
    }
    public void FecharGrafico()
    {
        MenuGrafico.SetActive(false);
        painelOpcoes.SetActive(true);
        //painelMenuInicial.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstConfigOption);
    }
    public void FecharSom()
    {
        MenuSom.SetActive(false);
        painelOpcoes.SetActive(true);
        //painelMenuInicial.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstConfigOption);
    }
    public void FecharAcessibilidade()
    {
        MenuAcessibilidade.SetActive(false);
        painelOpcoes.SetActive(true);
        //painelMenuInicial.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstConfigOption);
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
