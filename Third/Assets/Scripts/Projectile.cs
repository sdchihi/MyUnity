using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    //이 변수를 통해 어떤 오브젝트 ,  어떤 레이어가 발사체와 충돌할지 결정할수있음.
    public LayerMask collisionMask;

    public float speed = 10;
    float damage = 1;
    float lifeTime = 3;
    float skinWidth = 0.1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        //레이캐스트는 컬라이더 내부에서 발생하면 작동하지않는다 ->  붙은상태에서 총쏘면 원하는 충돌효과가 만들어지지 않아. 그래서 밑의 작업을 해줌
        Collider[] initialCollider = Physics.OverlapSphere(transform.position,0.1f, collisionMask);
        if (initialCollider.Length > 0) {       //총알이 생성되었을때 이미 충돌 오브젝트와 겹친 상태일때 .
            onHitObject(initialCollider[0]);
        }
    }
   

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

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask , QueryTriggerInteraction.Collide)) {
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

    void onHitObject(Collider c) {

        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }

        print(c.gameObject.name);
        GameObject.Destroy(gameObject);
    }
}
