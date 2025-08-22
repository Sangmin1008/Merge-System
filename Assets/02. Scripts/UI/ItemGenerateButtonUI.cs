using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGenerateButtonUI : MonoBehaviour
{
    private Button _generateButton;

    private void Awake()
    {
        _generateButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _generateButton.onClick.AddListener(GenerateItem);
    }

    private void OnDisable()
    {
        _generateButton.onClick.RemoveListener(GenerateItem);
    }

    private void GenerateItem()
    {
        GameManager.Instance.GenerateItem();
    }
}
