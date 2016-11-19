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
        //do some stuff here        아마 파티클 효과 생성할듯
        TakeDamage(damage);
    }

    //신기한 기능 Test할때 주로 쓰이는 기능으로
    //Inspector에서 해당 함수를 곧바로 실행할수 있다. 
    //현재 플레이어가 죽었을때 상황을 만들려고할때 굳이 기다릴 필요없이 바로 실행시키면된다.
    [ContextMenu("Self Destruct")]
    protected void Die() {
        GameObject.Destroy(gameObject);

        if (OnDeath != null) {
            OnDeath();
        }
        dead = true;
    }

    public void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }
}
