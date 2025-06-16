using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory : MonoBehaviour
{
    public GameObject CreateMonsterToID(int monsterID)
    {
        //TODO: id로 Parsing
        
        //에러 안나게 임시 코드
        return GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
}
