using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0,1)]
    public float outLinePercent;
    [Range(0,1)]
    public float obstaclePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    public int seed = 10;
    Coord mapCenter;

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {

        allTileCoords = new List<Coord>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++) {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),seed));   //toArray - >  배열로 반환
        mapCenter = new Coord((int)(mapSize.x / 2), (int)(mapSize.y / 2));

                string holderName = "Generated map";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);       //에디터에서 실행할거라서 destroy가아닌 destroyImmediate
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;


        //타일 생성부분.
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                //맵을 생성할 위치설정
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right *90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outLinePercent);    //테두리 결정

                newTile.parent = mapHolder;
            }
        }
        
        //이제 쓸모없이 갖히는 공간을 만들지 않기 위한 obstacle map
        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        //장애물 생성
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x,randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

    }


    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
        /*
        Flood Fill 알고리즘이용 - > 중앙에는 장애물이 없는것을 알고있으니까 중앙부터 시작해서 레이더가 퍼져나가듯
                                  타일을 검색해나가고 전체 타일수가 얼마나 되는지 알고있는 상태에서  현재 장애물의 수를 이용해 
                                  거기에 장애물이 아닌 타일이 얼마나 존재해야하는지 알아낸다 .
        */

        bool[,] mapFlag = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(mapCenter);
        mapFlag[mapCenter.x, mapCenter.y] = true;

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
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);

        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x , int y) {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y); 
    }

    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);        //랜덤하게 저장된 coord 큐에서 deque했다가 다시 맨뒤로 enqueue해줌..
        return randomCoord;
    }

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

}


