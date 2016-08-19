using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour , IDamageable {

    public float startingHealth;
    protected float health;
    protected bool dead;


    //델리게이트 메소드로서  다른 메소드의 위치를 가르키고 불러올수있음.  함수 포인터라고 생각하랍니다.
    public event System.Action OnDeath;


    //   중요 -   >  virtual을 추가함으로써 Start함수가 겹치지만 그렇지않게 도와준다.
    protected virtual void Start() {

        health = startingHealth;

    }

    public void TakeHit(float damage, RaycastHit hit) {

        health -= damage;

        if (health <= 0 && ! dead )
        {
            Die();
        }
    }

    protected void Die() {
        GameObject.Destroy(gameObject);

        if (OnDeath != null) {
            OnDeath();
        }
        dead = true;
    }
}
