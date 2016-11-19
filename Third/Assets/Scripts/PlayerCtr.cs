using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class PlayerCtr : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidBody;

	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
	}

    //FixedUpdate를 쓰는이유 - > 정기적으로 짧고 반복적으로 실행되어야 하기때문에..     - > 프레임 저하가 발생해도 이동속도를 유지함 
    void FixedUpdate() {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 v) {
        velocity = v;
    }

    //LookAt을 정의하는데 Y축이 돌아가는것을 막기위해 정의한것같다. x,z축은 돌아가지만 몸체가 돌아가진않게. 그렇게한듯함.
    public void LookAt(Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint );
    }
}
