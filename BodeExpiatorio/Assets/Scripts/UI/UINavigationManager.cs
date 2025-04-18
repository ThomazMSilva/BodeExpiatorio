﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UINavigationManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    private Stack<GameObject> panelStack = new();
    public Stack<GameObject> UIPanelsStack { get { return panelStack; } }

    //private void Awake() => panelStack = new();

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && panelStack.Count > 0)
        {
            SelectDefaultButton(panelStack.Peek());
        }
    }

    public void OpenPanel(GameObject newPanel)
    {
        if (panelStack.Count > 0)
        {
            var topPanel = panelStack.Peek();
            
            if(topPanel != mainMenuScreen) 
                topPanel.SetActive(false);
        }

        newPanel.SetActive(true);
        panelStack.Push(newPanel);

        SelectDefaultButton(newPanel);
    }

    public void OpenPanel(Monologo newPanel)
    {
        if (panelStack.Count > 0)
        {
            var topPanel = panelStack.Peek();

            if (topPanel != mainMenuScreen && topPanel.TryGetComponent<Monologo>(out var monologo))
            {
                topPanel.SetActive(false);
                panelStack.Pop();
            }
        }

        newPanel.gameObject.SetActive(true);
        panelStack.Push(newPanel.gameObject);
    }

    public void ClosePanel()
    {
        if (panelStack.Count == 0)
        {
            Debug.LogWarning("Tentou fechar painel sem nem ter painel aberto.");
            return;
        }

        var currentPanel = panelStack.Pop();
        currentPanel.SetActive(false);

        if (panelStack.Count > 0)
        {
            var previousPanel = panelStack.Peek();
            previousPanel.SetActive(true);

            SelectDefaultButton(previousPanel);
        }
    }

    public void ClosePanel(Monologo panelToClose)
    {
        if (panelStack.Count == 0)
        {
            Debug.LogWarning("Tentou fechar painel sem nem ter painel aberto.");
            return;
        }

        var currentPanel = panelStack.Peek();
        if (currentPanel == panelToClose.gameObject) panelStack.Pop();
        currentPanel.SetActive(false);

        if (panelStack.Count > 0)
        {
            var previousPanel = panelStack.Peek();
            previousPanel.SetActive(true);

            SelectDefaultButton(previousPanel);
        }
    }

    private void SelectDefaultButton(GameObject panel)
    {
        var defaultButton = panel.GetComponentInChildren<UnityEngine.UI.Selectable>();
        if (defaultButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        }
    }

    public void CloseAllPanels()
    {
        while (panelStack.Count > 0)
        {
            var panel = panelStack.Pop();
            panel.SetActive(false);
        }
    }
}
