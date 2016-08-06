using UnityEngine;
using System.Collections;

public class FireCtr : MonoBehaviour {

    public GameObject bullet;
    public Transform firePos;

	
	
	void Update () {

        if (Input.GetMouseButtonDown(0)) {
            Fire();
        }
	}

    void Fire() {
        CreateBullet();
    }
    void CreateBullet() {
        //동적으로 bullet 프리팹에서 bullet오브젝트를 생성
        Instantiate(bullet, firePos.position, firePos.rotation); 
    }
}
