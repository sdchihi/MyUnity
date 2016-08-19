using UnityEngine;
using System.Collections;

public class MonsterCtr : MonoBehaviour
{

    public enum MonsterState { idle, trace, attack, die, stun, roll ,gotHit};
    public MonsterState monsterState = MonsterState.idle;

    public GameObject stEffectPrefab;
    public GameObject ndEffectPrefab;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 1.0f;

    private bool isDie = false;
    private bool allowHit = true;

    private int hp = 100;

    private int hitNum = 0;

    

    // Use this for initialization
    void Awake()
    {
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
    }


    void Update()
    {
        if (animator.GetInteger("FinalHitMotion") == 2)
        {
            Vector3 temp = Vector3.back;
            monsterTr.Translate(temp*Time.deltaTime, Space.Self);
            
        }
    }


    IEnumerator CheckMonsterState()                   //수정필요
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (!animator.GetBool("IsGotHit") && animator.GetInteger("FinalHitMotion") == 0)
            {
                if (dist <= attackDist)
                {
                    monsterState = MonsterState.attack;
                }
                else if (dist <= traceDist && attackDist < dist)
                {
                    monsterState = MonsterState.trace;
                }
                else
                {
                    monsterState = MonsterState.idle;
                }
            }
            else if (animator.GetInteger("FinalHitMotion") == 1 )
            {
                monsterState = MonsterState.stun;
                yield return new WaitForSeconds(1.7f);
                monsterState = MonsterState.idle;
                animator.SetInteger("FinalHitMotion", 0);
            }
            else if (animator.GetInteger("FinalHitMotion") == 2)
            {
                monsterState = MonsterState.roll;
                monsterState = MonsterState.idle;
                animator.SetInteger("FinalHitMotion", 0);
                yield return new WaitForSeconds(3.5f);

                animator.SetBool("IsAttack", false);
            }
            else if (animator.GetBool("IsGotHit"))
            {   
                monsterState = MonsterState.gotHit;
                yield return new WaitForSeconds(0.8f);
                monsterState = MonsterState.idle;
                animator.SetBool("IsGotHit", false);
            }
            else {
                
            }
        }
    }

    

    void OnCollisionEnter(Collision coll)
    {
        GameObject effect = null;
        
        if (Input.GetMouseButton(0) && allowHit)
        {
            if (coll.collider.tag == "Punch")
            {

                animator.SetBool("IsGotHit", true);

                effect = (GameObject)Instantiate(stEffectPrefab, new Vector3(monsterTr.position.x, monsterTr.position.y + 0.5f, monsterTr.position.z), Quaternion.Euler(0, 0, 0));
                //hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
                /*
                if (hp <= 0)
                {
                    MonsterDie();
                }
                */
            }
            else if (coll.collider.tag == "Kick")
            {
                animator.SetInteger("FinalHitMotion", Random.Range(1, 3));
                effect = (GameObject)Instantiate(ndEffectPrefab, new Vector3(monsterTr.position.x, monsterTr.position.y + 0.5f, monsterTr.position.z), Quaternion.Euler(0, 0, 0));
            }
            hitNum++;   

            Debug.Log("어떤 공격인지 :" + coll.collider.tag + "  피격 당한 횟수 : " + hitNum);

            Destroy(effect, 0.8f);
            StartCoroutine(ctrHit());
        }

        
    }

    void MonsterDie()
    {
        StopAllCoroutines();

        gameObject.tag = "Untagged";
        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();
        animator.SetTrigger("IsDie");



        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        //StartCoroutine(this.PushObjectPool());
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {

            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.Stop();
                    animator.SetBool("IsWalk", false);
                    animator.SetBool("IsAttack", false);
                    break;

                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    animator.SetBool("IsWalk", true);
                    animator.SetBool("IsAttack", false);
                    break;

                case MonsterState.attack:
                    animator.SetBool("IsAttack", true);
                    animator.SetBool("IsWalk", false);
                    monsterTr.LookAt(playerTr.position);

                    if (playerTr.position.y < 0.01)
                    {
                        monsterTr.LookAt(new Vector3(playerTr.position.x, playerTr.position.y + 0.09f, playerTr.position.z));
                    }
                    break;
                
            }
            yield return null;
        }
    }

    void OnEnable()
    {
        //PlayerCtr.OnPlayerDie += this.OnPlayerDie;
        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }

    void OnDisable()
    {
        //PlayerCtr.OnPlayerDie -= this.OnPlayerDie;
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.Stop();
        animator.SetTrigger("IsPlayerDie");
    }
    /*
    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        gameObject.SetActive(false);
    }
    */
    void OnDamage(object[] _params)
    {
       
        
        hp -= (int)_params[1];
        if (hp <= 0)
        {
            MonsterDie();
        }
        animator.SetTrigger("IsGotHit");


    }




    IEnumerator ctrHit() {

        allowHit = false;
        yield return new WaitForSeconds(0.2f);
        allowHit = true;
    }


}