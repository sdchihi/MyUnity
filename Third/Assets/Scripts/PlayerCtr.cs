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

    public void LookAt(Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint );
    }
}
