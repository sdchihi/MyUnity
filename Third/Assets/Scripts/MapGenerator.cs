using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{

    public Transform tilePrefab;
    public Vector2 mapSize;

    [Range(0,1)]
    public float outLinePercent;

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {

        string holderName = "Generated map";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);       //에디터에서 실행할거라서 destroy가아닌 destroyImmediate
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;


        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                //맵을 생성할 위치설정
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right *90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outLinePercent);    //테두리 결정

                newTile.parent = mapHolder;
            }
        }
    }




}


