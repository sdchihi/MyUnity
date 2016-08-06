using UnityEngine;
using System.Collections;

public class BulletCtr : MonoBehaviour {

    public int damage = 10;
    public float speed = 1000.0f;

	void Start () {
	
	}
	
	void Update () {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}
}
