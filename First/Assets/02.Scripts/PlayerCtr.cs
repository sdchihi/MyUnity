using UnityEngine;
using System.Collections;

//inspector 뷰에 노출된다.
[System.Serializable]
public class Anim {
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}


public class PlayerCtr : MonoBehaviour {

    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f;

    public float rotSpeed = 10.0f;

    public Anim anim;               //위에 선언한 클래스 변수.
    public Animation _animation;    // animation 컴포넌트를 접근하기위한 변수

	void Start () {
        //tr에 Transform 컴포넌트를 할당함.
        tr = GetComponent<Transform>();
        _animation = GetComponentInChildren<Animation>();       //자신의 하위에 있는 animation 컴포넌트를 얻어오는것

        _animation.clip = anim.idle;
        _animation.Play();

       
	}
	
	void Update () {
        // Input.GetAxis("name")  --> 해당 이름의 키조합을 받아오는 기능을함. 즉 h엔 Horizontal키조합인 left, right방향키 , a, d 로 수평으로 움직이는것을 인식함
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        // Vector3.forward -- > z축방향 단위벡터의 축약형임. 
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //tr.Translate( 방향 * 속도 * 델타타임 , 기준좌표계)
        //normalized를 해주어야 대각선 방향의 방향성분을 정확히 얻을수있음. (피타고라스의 정리에 의해.)
        tr.Translate( moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        if (v >= 0.1f)
        {
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else {
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }
}
