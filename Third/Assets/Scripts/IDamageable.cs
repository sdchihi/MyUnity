using UnityEngine;

public interface IDamageable { 

    //raycasthit 를통해 충돌지점이 어디인지 등의 기타 정보를 얻어오는것
    void TakeHit(float damage, RaycastHit hit);
    void TakeDamage(float damage);
}