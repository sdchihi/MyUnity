using UnityEngine;
using System.Collections;

public class WallCtr : MonoBehaviour {

    public GameObject sparkEffect;

    void OnCollisionEnter(Collision col) {

        if (col.collider.tag == "BULLET") {

            GameObject spark = (GameObject)Instantiate(sparkEffect, col.transform.position, Quaternion.identity);

            Destroy(spark, spark.GetComponent<ParticleSystem>().duration + 0.2f);
            Destroy(col.gameObject);            
        }

    }

}
