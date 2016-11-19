using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    float nextShotTime;

    public void Shoot() {

        if (Time.time > nextShotTime)
        {
            //슈팅간 간격설정하는 방법. 쏠때마다 시간을 갱신한다.
            nextShotTime = Time.time + msBetweenShots / 1000 ;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.setSpeed(muzzleVelocity);
        }
    }

}
