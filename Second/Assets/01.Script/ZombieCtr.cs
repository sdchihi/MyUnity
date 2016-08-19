using UnityEngine;
using System.Collections;

public class ZombieCtr : MonoBehaviour
{

    public enum MonsterState { idle, trace, attack, die, stun, roll, gotHit };
    public MonsterState monsterState = MonsterState.idle;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    private bool isDie = false;

    private int hp = 100;

    // Use this for initialization
    void Awake()
    {
        monsterTr = this.gameObject.GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = this.gameObject.GetComponent<Animator>();
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
                else if (dist <= traceDist)
                {
                    monsterState = MonsterState.trace;
                }
                else
                {
                    monsterState = MonsterState.idle;
                }
            }
            else if (animator.GetInteger("FinalHitMotion") == 1 || animator.GetInteger("FinalHitMotion") == 2)
            {
                
            }
            else if (animator.GetBool("IsGotHit"))
            {
                monsterState = MonsterState.gotHit;
            }
            else {

            }
        }
    }



    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Punch")
        {

            animator.SetTrigger("IsGotHit2");
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

        }

        Debug.Log(coll.collider.tag);
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
                    animator.SetBool("IsTrace", false);
                    break;

                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsAttack", false);
                    break;

                case MonsterState.attack:
                    animator.SetBool("IsAttack", true);
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

}