using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    private const float MOVE_DISTANCE = 5f;
    private float move = 0f;
    private Vector3 dir = Vector3.right;
    
    private void Start()
    {
        GameManager.Instance.invoker.RegisterCommand(typeof(GameOverState), new PrintGameOverUICommand());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Status.CURRENT_HP.Set(Status.CURRENT_HP.Get<float>() - 10);
            Debug.LogWarning("Player Get 10 Damage, Current HP : " + Status.CURRENT_HP.Get<float>());
            Debug.LogWarning("Current Max HP : " + Status.MAX_HP.Get<float>());
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var item = Item.BOOK.Get<(float, int)>().Item1;
            
            Item.BOOK.Set(item + 99);

            Debug.LogWarning(Item.BOOK.Get<(float, int)>());
        }

        if (Mathf.Abs(move) > MOVE_DISTANCE)
        {
            move = 0f;
            dir = dir == Vector3.right ? Vector3.left : Vector3.right;
        }
        else
        {
            move += Time.deltaTime;
            transform.position += dir * Time.deltaTime;
        }
    }
}
