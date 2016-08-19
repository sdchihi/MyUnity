using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
    public GameObject effectPrefab;
    private Animator anim; 

	void Start () {
        anim = this.gameObject.GetComponent<Animator>();
	}
	
	void Update () {
	
	}


}
