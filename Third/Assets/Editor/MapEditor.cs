using UnityEngine;
using System.Collections;
using UnityEditor;

//이 에디터 스크립트가 어떤클래스 , 스크립트를 다루는지 명시해야함  밑에
[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor {
    //에디터를 사용함으로써 굳이 게임을 실행하지 않더라도 리얼타임으로 상태를 갱신할수있다.
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;      //CustomEditor로 다룰거라고 선언한 오브젝트는 target으로 접근이 가능하다.

        map.GenerateMap();
    }

}
