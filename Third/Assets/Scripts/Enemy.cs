using UnityEngine;
using System.Collections;


// Enemy 클래스는 LivingEntity 를 상속함으로써 MonoBehaviour 와 Idamageable을 같이 상속하고있습니다.
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    NavMeshAgent pathFinder;
    Transform target;


    // LivingEntity를 상속하고있다.  LivingEntity 클래스에도 Start가 있으므로 override 한뒤에 LivingEntity의 start도 불러오기위해
    // base.start 를  추가해줌으로써  start가  부모클래스의 것도 같이 쓸수있게된다.
	protected override void Start () {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }
	

	void Update () {
        
	}

    IEnumerator UpdatePath() {
        //update 에서 path를 계속 갱신하는것은 성능에 좋지않은 영향을 미치기때문에 (특히 장애물이 많은경우)
        //Coroutine에서 실행해주는것이 좀더낫다. 
        
        float refreshRate = .25f;

        while (target != null) {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            if (!dead)
            {
                pathFinder.SetDestination(targetPosition);
            }
            yield return new WaitForSeconds(refreshRate);
        }

    }
}
