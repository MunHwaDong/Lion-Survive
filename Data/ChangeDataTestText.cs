using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDataTestText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Status status;

    void OnEnable()
    {
        DataController.Instance.ObserveData(status, UpdateText);
    }

    void UpdateText(object data)
    {
        text.text = text.text.Substring(0, text.text.IndexOf(':') + 1) + data;
    }
}