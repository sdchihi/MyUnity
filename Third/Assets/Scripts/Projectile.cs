using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    //이 변수를 통해 어떤 오브젝트 ,  어떤 레이어가 발사체와 충돌할지 결정할수있음.
    public LayerMask collisionMask;

    public float speed = 10;
    float damage = 1;
    
    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        float moveDistance = speed * Time.deltaTime;

        CheckCollision(moveDistance);

        transform.Translate(Vector3.forward * moveDistance);	
	}

    void CheckCollision(float moveDistance) {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask , QueryTriggerInteraction.Collide)) {
            onHitObject(hit);
        }
    }

    void onHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null) {
            damageableObject.TakeHit(damage,hit);
        }

        print(hit.collider.gameObject.name);
        GameObject.Destroy(gameObject); 
    }

}
