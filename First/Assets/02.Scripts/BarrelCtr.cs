using UnityEngine;
using System.Collections;

public class BarrelCtr : MonoBehaviour {

    public GameObject expEffect;
    private Transform tr;

    private int hitCount = 0;

    //무작위로 텍스쳐를 연결할 배열
    public Texture[] textures;

	void Start(){
        tr = GetComponent<Transform>();

        int idx = Random.Range(0, textures.Length);

        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
	}

    void OnCollisionEnter(Collision col) {
        if (col.collider.tag == "BULLET") {
            Debug.Log(hitCount);
            Destroy(col.gameObject);

            if (++hitCount >= 3)
                ExpBarrel();
        }
    }

    void ExpBarrel() {

        Instantiate(expEffect, tr.position, Quaternion.identity);

        //tr의 위치를 중심으로 10 만큼 영역에 들어와있는 collider객체 추출합니다.
        Collider[] cols = Physics.OverlapSphere(tr.position, 10.0f);

        //추출한 객체에 폭발력을 전달하는 부분
        foreach (Collider col in cols) {

            Rigidbody rbody = col.GetComponent<Rigidbody>();
            if (rbody != null) {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }

        //폭발이 일어나고 5초뒤에 오브젝트 제거
        Destroy(gameObject, 5.0f);
    }
}
