using UnityEngine;
using System.Collections;

public class PlayerCtr : MonoBehaviour {

    private float h = 0.0f;
    private float v = 0.0f;

    private bool jump = false;
    public float jumpPower = 0.0f;

    private Transform tr;
    public float moveSpeed=10.0f;

    public enum PlayerState { idle, walk, gotHit, attack, die};


    public PlayerState monsterState = PlayerState.idle;

    private bool isDie = false;

    private Animator animator;
    private Rigidbody rigid;

    

	void Start () {
        tr = GetComponent<Transform>();

        animator = this.gameObject.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
	}
	


	void Update ()
    {
        

        if (animator.GetBool("IsAttack"))
        {
            v = 0;
            h = 0;
        }
        else {
            v = -Input.GetAxis("Horizontal");
            h = Input.GetAxis("Vertical");
        }
        Vector3 moveForward = ((Vector3.forward * v) + (Vector3.forward * h)).normalized;



        if (v >= 0.1f)
        {
            tr.rotation = Quaternion.Euler(0, 0, 0);
            moveForward = Vector3.forward * v;
            if (h >= 0.1f)
            {
                tr.rotation = Quaternion.Euler(0, 45, 0);
                moveForward = Vector3.forward * h + Vector3.forward * v;
            }
            else if (h <= -0.1f)
            {
                tr.rotation = Quaternion.Euler(0, -45, 0);
                moveForward = Vector3.back * h + Vector3.forward * v;
            }
            animator.SetBool("IsWalk", true);
        }
        else if (v <= -0.1f)
        {
            tr.rotation = Quaternion.Euler(0, 180, 0);
            moveForward = Vector3.back * v;
            if (h >= 0.1f)
            {
                tr.rotation = Quaternion.Euler(0, 135, 0);
                moveForward = Vector3.forward * h + Vector3.back * v;
            }
            else if (h <= -0.1f)
            {
                tr.rotation = Quaternion.Euler(0, 225, 0);
                moveForward = Vector3.back * h + Vector3.back * v;
            }
            animator.SetBool("IsWalk", true);
        }
        else if (h >= 0.1f)
        {
            tr.rotation = Quaternion.Euler(0, 90, 0);
            moveForward = Vector3.forward * h;
            animator.SetBool("IsWalk", true);
        }
        else if (h <= -0.1f)
        {
            tr.rotation = Quaternion.Euler(0, -90, 0);
            moveForward = Vector3.back * h;
            animator.SetBool("IsWalk", true);
        }
        else
        {
            animator.SetBool("IsWalk", false);
        }
        moveForward.y = 0;

        tr.Translate(moveForward.normalized * Time.deltaTime * moveSpeed, Space.Self);
        

        if (!animator.GetBool("IsAttack")) {
            if (Input.GetMouseButton(0))
            {
                StartCoroutine(this.PlayerAttack());
            }
        }

        jump = Input.GetButtonDown("Jump");

        if (jump && !animator.GetBool("IsJump") && !animator.GetBool("IsAttack") ) {
            Vector3 dir = tr.forward;
            dir.y = 1.0f;

            StartCoroutine(PlayerJump());
            
            // rigid.velocity = new Vector3(0, 10, 0);
        }
        
    }

    IEnumerator PlayerJump() {
        Vector3 dir = tr.forward;
        dir.y = 1.0f;

        animator.SetBool("IsJump", true);

        rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        yield return new WaitForSeconds(0.8f);
        animator.SetBool("IsJump", false);
    }


    IEnumerator PlayerAttack()
    {
        animator.SetBool("IsAttack", true);
        
        yield return new WaitForSeconds(0.16f);

        animator.SetBool("IsAttack", false);

    }


}
