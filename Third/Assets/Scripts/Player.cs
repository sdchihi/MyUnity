using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerCtr))]
[RequireComponent(typeof(GunCtr))]
public class Player : LivingEntity
{
    
    Camera viewCamera;
    public float moveSpeed = 5.0f;
    PlayerCtr controller;
    GunCtr gunController;

    protected override void Start()
    {
        //
        base.Start();
        controller = GetComponent<PlayerCtr>();
        gunController = GetComponent<GunCtr>();
        viewCamera = Camera.main;
    }

    void Update()
    {

        //이동을 입력받는 곳
        //GetAxisRaw - > 스무딩 효과가 적용되지 않음 . 
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);


        //바라보는 방향을 입력받는곳
        //ScreenPointToRay - > 보이는 화면상의 위치를 반환해주는 메소드인데 Input.mousePosition을 인자로 넣음으로서 마우스가 가르키는 곳의 위치를 반환
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);     //법선
        float rayDistance;
        //이게 카메라부터 그 플레인에 닿는곳 까지 광선을쏨
        //raycast - > 플레인과 인자 ray간의 교차 여부를 알려주는 함수인데 out으로 들어간 인자에 해당 리턴값이 할당되는 그런 기능을하고 있음.
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);

            controller.LookAt(point);
        }


        //무기 조작 입력
        if (Input.GetMouseButton(0)) {
            gunController.Shoot();
        }
         
    }
}