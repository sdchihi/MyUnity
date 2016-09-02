using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {


    //캠핑 방지를 위한 플레이어 위치 추적. 즉 같은위치에 일정시간 이상 머물면 그위치에 스폰이 이루어지도록 
    LivingEntity playerEntity;
    Transform playerT;

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRamainingAlive;
    float nextSpawnTime;

    MapGenerator map;
    
    //캠핑 검사 간격
    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    void Start() {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        playerEntity.OnDeath += OnPlayerDeath;
        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update() {

        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);      //얼마나 움직였나 체크를 해본다.
                campPositionOld = playerT.position;
            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                /*
                Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
                spawnedEnemy.OnDeath += OnEnemyDeath;           // Enemy를 스폰할때마다 OnEnemyDeath 함수를 추가시켜줌.. (onDeath에다가)
                */
                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;       //적이 소환될때 반짝이는 시간을 저장하는 변수
        float tileFlashSpeed = 4;   // 초당 타일이 몇번 반짝일지.

        Transform randomTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            randomTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color fleshColor = Color.red;
        float spawnTimer = 0;               //소환 시간 측정기  이 코루틴을 시작한지 얼마나 되었는지 측정합니다.

        while (spawnTimer < spawnDelay)
        {
            //0 - > 1 ,  1 - > 0 으로 오르락 내리락할수있게 Mathf.pingpong을쓴다.. 첫번째 파라미터는 속도 , 두번째는 범위
            tileMat.color = Color.Lerp(initialColor, fleshColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;          //한 프레임 딜레이시켜

        }
        //반짝이고 생성한다.


        Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;           // Enemy를 스폰할때마다 OnEnemyDeath 함수를 추가시켜줌.. (onDeath에다가)
    }

    void NextWave() {
        currentWaveNumber++;

        print(" Wave : " + currentWaveNumber);

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRamainingAlive = enemiesRemainingToSpawn;
        }
    }

    void OnPlayerDeath() {
        isDisabled = true;
    }

    //사실상 OnEnemyDeath 함수는 LivingEntity의 OnDeath를 통해 호출되는것. 한마디로 그쪽에서 이걸 불러온다고 생각하면 되겠네
    void OnEnemyDeath() {
        enemiesRamainingAlive--;

        if (enemiesRamainingAlive == 0) {
            NextWave();
        }
    }


    // system.Serializable을 함으로써 Inspector창에 wave가 보인다.
    [System.Serializable]
    public class Wave{
        public int enemyCount;
        public float timeBetweenSpawns;
    }

}
