using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameOverUI : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.testGameOverUI = gameObject;
        gameObject.SetActive(false);
    }
}
