﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UINavigationManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    private Stack<GameObject> panelStack = new Stack<GameObject>();

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
