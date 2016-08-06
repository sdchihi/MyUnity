using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

    public Transform targetTr;          //추적할 오브젝트의 트랜스폼 컴포넌트를 얻어올 변수
    public float dist = 10.0f;
    public float height = 3.0f;
    public float dampTrace = 20.0f;     //부드러운 추적을 위한 변수라고함.

    private Transform tr;

	void Start () {
        tr = GetComponent<Transform>();
	}

    //타깃 오브젝트가 이동한뒤에 카메라가 쫒아가는 형태로 조작하기위한 lateUpdate 함수이용
    void LateUpdate() {

        //카메라의 위치를 추적대상 dist만큼 뒤쪽에 배치 후에  height만큼 위로 올림.
        tr.position = Vector3.Lerp(tr.position
            , targetTr.position - (targetTr.forward * dist) + (Vector3.up * height)
            , Time.deltaTime * dampTrace);

        //카메라가 타겟 게임오브젝트를 바라보게 설정.
        tr.LookAt(targetTr.position);
    }
	
}
