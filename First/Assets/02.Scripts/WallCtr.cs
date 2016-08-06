using UnityEngine;
using System.Collections;

public class WallCtr : MonoBehaviour {


    void OnCollisionEnter(Collision col) {

        if (col.gameObject.tag == "BULLET") {
            Destroy(col.gameObject);
        }

    }

}
