using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private SceneAsset cenaDeJogo; 
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;

  
    private string GetSceneName(SceneAsset sceneAsset)
    {
        return sceneAsset.name;
    }

    public void jogar()
    {

        SceneManager.LoadScene(GetSceneName(cenaDeJogo));
    }

    public void AbrirOp��es()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOp��es()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }
}
