using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRamainingAlive;
    float nextSpawnTime;

    void Start() {
        NextWave();
    }

    void Update() {

        if (enemiesRemainingToSpawn > 0  && Time.time > nextSpawnTime) {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;           // Enemy를 스폰할때마다 OnEnemyDeath 함수를 추가시켜줌.. (onDeath에다가)
        }
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
