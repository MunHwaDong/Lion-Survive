using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    [SerializeField] private NavGraph navGraph;
    
    [SerializeField] private Collider detectHurdleBox;
    
    private Coroutine coroutine;

    void Start()
    {
        coroutine = StartCoroutine(MoveDirect());
    }

    private IEnumerator MoveDirect()
    {
        while (true)
        {
            transform.position -= (transform.position - player.transform.position).normalized * (Time.deltaTime * 2f);

            yield return new WaitForFixedUpdate();
            
            Debug.DrawLine(player.transform.position, transform.position, Color.red);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurdle")
        {
            var st = navGraph.FindClosestNode(transform.position);
            var en = navGraph.FindClosestNode(player.transform.position);

            var path = Astar.FindPath(navGraph, st, en);
            
            Debug.Log(path);
            
            if(coroutine is not null) StopCoroutine(coroutine);
            
            //coroutine = StartCoroutine(Move(path));
        }
    }

    IEnumerator Move(List<NavNode> nodes)
    {
        for (int i = 0; i < nodes.Count;)
        {
            Vector3 targetPos = nodes[i].worldPosition;
            
            Debug.LogWarning(nodes[i].worldPosition);
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 2f);

            transform.rotation = Quaternion.LookRotation(transform.position - targetPos, Vector3.up);

            if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
            {
                i++;
            }

            //몬스터와 플레이어 사이에 장애물이 없다면 바로 Player 방향으로 이동한다.
            if (Physics.Raycast(transform.position, player.transform.position - transform.position, 
                    out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("Hurdle")) is false)
            {
                StopCoroutine(coroutine);
                
                coroutine = StartCoroutine(MoveDirect());
            }

            yield return null;
        }
    }
}
