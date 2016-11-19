using UnityEngine;
using System.Collections;

using System.Collections.Generic;
public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshMaskPrefab;
    public Transform navmeshFloor;
    public Vector2 maxMapSize;
    

    [Range(0,1)]
    public float outLinePercent;

    public float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;

    Map currentMap;


    void Start() {
        //Spawner의 이벤트에 여기 OnNewWave를 등록함
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
       
    }

    void OnNewWave(int waveNumber) {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap() {
        
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y*tileSize);

        //좌표 생성
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),currentMap.seed));   //toArray - >  배열로 반환

        //맵홀더 오브젝트생성 (상하위구조)
        string holderName = "Generated map";
        if (transform.FindChild(holderName))        //Generated Map 아래 차일드 게임오브젝트가 존재하면 ??
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);       //에디터에서 실행할거라서 destroy가아닌 destroyImmediate
        }

        Transform mapHolder = new GameObject(holderName).transform;         //Generated Map 게임오브젝트를 Map아래 (하위)로 생성
        mapHolder.parent = transform;


        //타일 생성부분.
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                //맵을 생성할 위치설정
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right *90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outLinePercent) * tileSize;    // localScale로 크기 결정.  (즉 간격 설정해주는 기능)

                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }
        
        //이제 쓸모없이 갖히는 공간을 만들지 않기 위한 obstacle map
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        //장애물 생성
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);     //모든좌표의 타일을 리스트에 담아놨다가 장애물 생싱할때마다 빼줍니다.
        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x,randomCoord.y] = true;
            currentObstacleCount++;

            //밑에 조건문  해석 - > random으로 얻어온 좌표가 중앙이 아니거나!  접근할수없는 블록이 아닐경우
            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2 , Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale =new Vector3(((1 - outLinePercent) * tileSize),obstacleHeight,((1 - outLinePercent) * tileSize));
                
                //밑에는 맵마다 material을 다르게 적용시키기 위한코드 ( 색 )
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;           //y값에따른 그라데이션
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;         //좀 번거롭지만 Swap처럼 사용하는것같다.

                allOpenCoords.Remove(randomCoord);
            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));
        // nav mesh obstacle생성을 위한 부분이요 
        Transform leftMask = Instantiate(navmeshMaskPrefab, Vector3.left*(currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity)as Transform;
        leftMask.parent = mapHolder;
        leftMask.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1 ,currentMap.mapSize.y) * tileSize;

        Transform rightMask = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        rightMask.parent = mapHolder;
        rightMask.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform topMask = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        topMask.parent = mapHolder;
        topMask.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform bottomMask = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        bottomMask.parent = mapHolder;
        bottomMask.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;

    }


    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
        /*
        Flood Fill 알고리즘이용 - > 중앙에는 장애물이 없는것을 알고있으니까 중앙부터 시작해서 레이더가 퍼져나가듯
                                  타일을 검색해나가고 전체 타일수가 얼마나 되는지 알고있는 상태에서  현재 장애물의 수를 이용해 
                                  거기에 장애물이 아닌 타일이 얼마나 존재해야하는지 알아낸다 .
        */

        bool[,] mapFlag = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(currentMap.mapCenter);
        mapFlag[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        // Flood Fill 
        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++)
                {   
                    //이웃한 8개의 타일을 순환할거야.
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if (x == 0 || y == 0) {
                        //좌표가 obstacleMap 안에 있는지 여부 확인 (밑에)
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1)) {
                            if(! mapFlag[neighborX,neighborY] && !obstacleMap[neighborX, neighborY]){   //장애물이 아닌지?
                                mapFlag[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);

        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x , int y) {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) *tileSize; 
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt((position.x / tileSize + (currentMap.mapSize.x - 1) / 2f));        //그냥 형변환 하면 내림밖에 안함.
        int y = Mathf.RoundToInt((position.z / tileSize + (currentMap.mapSize.y - 1) / 2f));

        x = Mathf.Clamp(x, 0, tileMap.GetLength(0)-1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1)-1);
        return tileMap[x, y];
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);        //랜덤하게 저장된 coord 큐에서 deque했다가 다시 맨뒤로 enqueue해줌..
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);        //랜덤하게 저장된 coord 큐에서 deque했다가 다시 맨뒤로 enqueue해줌..
        return tileMap[randomCoord.x, randomCoord.y];
    }


    [System.Serializable]
    public struct Coord {
        public int x;
        public int y;

        public Coord(int _x, int _y) {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2) {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

    }

    [System.Serializable]
    public class Map{

        public Coord mapSize;
        [Range (0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter {
            get {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }

    }

}


