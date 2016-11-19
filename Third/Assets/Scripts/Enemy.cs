using UnityEngine;
using System.Collections;


// Enemy 클래스는 LivingEntity 를 상속함으로써 MonoBehaviour 와 Idamageable을 같이 상속하고있습니다.
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State { Idle , Chasing , Attacking};
    State currentState;

    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color originColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttack = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    // LivingEntity를 상속하고있다.  LivingEntity 클래스에도 Start가 있으므로 override 한뒤에 LivingEntity의 start도 불러오기위해
    // base.start 를  추가해줌으로써  start가  부모클래스의 것도 같이 쓸수있게된다.
	protected override void Start () {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            skinMaterial = GetComponent<Renderer>().material;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            originColor = skinMaterial.color;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            currentState = State.Chasing;

            StartCoroutine(UpdatePath());
            
        }
    }
	

	void Update () {

        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                //Vector3.Distance()   - >  제곱근 연산을 하므로 비용이 큼.
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;   // 두 위치간 차의 제곱 

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) //sqrDistToTarget 이 attackDistance의 제곱 보다 작은지.  
                {
                    nextAttackTime = Time.time + timeBetweenAttack;
                    StartCoroutine(Attack());
                }
            }
        }
	}

    IEnumerator Attack() {
        Vector3 originPosition = transform.position;
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;    
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);
      

        float percent = 0;
        float attackSpeed = 3;

        pathFinder.enabled = false;
        currentState = State.Attacking;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1) {

            if(percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            //이거 잘봐두자 .  Interpolation  이거 ..  이렇게해서 공격했다가 돌아오는 걸 구현하는구나.
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;    //  4(-x^2 +x)
            transform.position = Vector3.Lerp(originPosition, attackPosition, interpolation);       // Lerp는  두 지점의 사이의 비례값 ( 0 - 1 ) 에따른 지점 반환해줌

            yield return null;
        }

        skinMaterial.color = originColor;
        
        pathFinder.enabled = true;
        currentState = State.Chasing;
    }

    IEnumerator UpdatePath() {
        //update 에서 path를 계속 갱신하는것은 성능에 좋지않은 영향을 미치기때문에 (특히 장애물이 많은경우)
        //Coroutine에서 실행해주는것이 좀더낫다. 
        
        float refreshRate = .25f;

        while (hasTarget) {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.transform.position - transform.position).normalized;    // 적이 플레이어를 공격할때 보는 방향벡터
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2); 
                //위의 타겟포지션이 지정됨으로써 적은 플레이어로부터 겹치지 않는지점으로 위치 추적을 실행한다.
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            
            yield return new WaitForSeconds(refreshRate);
        }

    }

    void OnTargetDeath() {
        hasTarget = false;
        currentState = State.Idle;
    }
}
