using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterPool : MonoBehaviour
{
    private const int MAX_POOL_SIZE = 200;
    private const int DEFAULT_SIZE = 10;
    
    private IObjectPool<IMonster> _pool;

    //각 Wave에서 Spawn 가능한 상한 ID, Index 0은 사용하지 않는다.
    private List<int> _possibleSpawnUpperIDEachWaves = new() { -1, 3, 6, 10 };
    
    //플레이어 시야 밖에서 몬스터를 생성하기 위한 변수
    [SerializeField] private Camera _mainCamera;
    
    [Header("Monster Table ID 기준으로 순서에 맞게 들어가 있어야 합니다.")]
    [Tooltip("절대 0번 인덱스에 값을 할당하지 마세요")]
    [SerializeField] private List<GameObject> _monsterPrefabs;
    
    public IObjectPool<IMonster> Pool
    {
        get
        {
            if (_pool is null)
            {
                _pool = new UnityEngine.Pool.ObjectPool<IMonster>(
                    Create,
                    TakeFromPool,
                    ReturnToPool,
                    DestroyPoolObject,
                    true,
                    DEFAULT_SIZE,
                    MAX_POOL_SIZE);
            }
            
            return _pool;
        }
    }

    private IMonster Create()
    {
        /*
         * 1. 몬스터 테이블에 Prefab 경로도 추가함
         * 2. 현재 웨이브에 나올 수 있는 몬스터 ID의 하한과 상한을 여기서 정의함
         * TODO: 3. 현재 웨이브에 맞는 몬스터가 더 많이 나오도록 Weight 값을 선언해 확률적으로 더 많이 나오게 한다
         * 4. 랜덤으로 ID를 뽑아 해당 ID에 맞는 몬스터 Prefab을 불러온다.
         * 5. 해당 Prefab의 IMonster를 GetComponent해서 테이블의 데이터들을 채워준다. (GetComponent는 Create()할 때만 한 번 실행, Pool에서 Take할 때는 IMonster로 저장할거니깐 이후에는 할 필요X)
         */
        int randomID = SelectMonsterID();
        
        var monster = Instantiate(_monsterPrefabs[randomID], new Vector3(0, 0, 0), Quaternion.identity).GetComponent<IMonster>();
        
        var randomPositionOfOutsideCamera = DecideMonsterSpawnRandomPosition(monster);
        
        monster.transform.SetParent(transform);
        monster.transform.position = randomPositionOfOutsideCamera;
        
        //Elite Idx는 101부터 시작함, Noraml 몬스터는 1부터 시작
        monster.ItemDropMethod = (randomID - 100 >= 0)
            ? DropItemManager.Instance.DropEliteReward
            : DropItemManager.Instance.DropEnemyReward;

        monster.name = _monsterPrefabs[randomID].name;
        
        monster.pool = Pool;
        
        return monster;
    }

    private int SelectMonsterID()
    {
        //TODO: 각 Wave에 맞는 Monster ID 정보를 랜덤을 뽑아야함
        int currentWave = PointManager.Instance.CurrentWave;
        
        int currentUpperID = _possibleSpawnUpperIDEachWaves[currentWave];
        
        return Random.Range(1, currentUpperID + 1);
    }

    private void TakeFromPool(IMonster monster)
    {
        monster.transform.position = DecideMonsterSpawnRandomPosition(monster);
        
        monster.gameObject.SetActive(true);
    }

    private void ReturnToPool(IMonster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void DestroyPoolObject(IMonster monster)
    {
        Destroy(monster.gameObject);
    }

    /// <summary>
    /// 인자로 받은 Monster의 크기에 맞게 현재 시야에 보이지 않는 랜덤한 위치를 생성합니다.
    /// </summary>
    /// <param name="monster">생성될 몬스터</param>
    /// <returns>시야에 보이지 않는 랜덤 좌표</returns>
    private Vector3 DecideMonsterSpawnRandomPosition(IMonster monster)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
        Vector3 randomPosition = Vector3.zero;
        
        Bounds randomBounds = new(monster.MonsterBound.center, monster.MonsterBound.size);

        int maxWorldXCoordinate = GameManager.Instance.MaxOfWorldXCoordinate;
        int maxWorldZCoordinate = GameManager.Instance.MaxOfWorldZCoordinate;

        int spawnPositionPadding = 250;
        
        while (true)
        {
            int randx = Random.Range(-maxWorldXCoordinate + spawnPositionPadding, maxWorldXCoordinate - spawnPositionPadding);
            int randz = Random.Range(-maxWorldZCoordinate + spawnPositionPadding, maxWorldZCoordinate - spawnPositionPadding);
            
            randomPosition.x = randx;
            randomPosition.z = randz;
            
            randomBounds.center = randomPosition;
            
            if(GeometryUtility.TestPlanesAABB(planes, randomBounds) is false)
                break;
        }

        return randomPosition;
    }
    
    public void Spawn()
    {
        Pool.Get();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Spawn();
        }
    }
}