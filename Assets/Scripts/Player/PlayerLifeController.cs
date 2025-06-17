using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
    [SerializeField] float InvincibleTime;
    [SerializeField] GameObject model;

    int startLife = 3;
    int maxLife = 3;
    int life = 0;

    float invincibleTime = 0;

    Action<int> onLifeChanged = (int i) => { };
    List<CollisionComponent> collisions = new();

    void Start()
    {
        ChangeLife(startLife);
    }

    void Update()
    {
        if (invincibleTime > 0)
        {
            HideModelWhileInvincible();
            invincibleTime -= Time.deltaTime;

            if (invincibleTime <= 0)
            {
                collisions.RemoveAll(x => x == null); // 消滅したものは除く

                if (collisions.Count == 0) return;

                ChangeLife(-1); // 無敵時間が終わったときに触れていたらダメージを受ける
            }
        }
    }

    void HideModelWhileInvincible()
    {
        if (invincibleTime < 0.25f)
        {
            model.SetActive(true);
        }
        else
        {
            model.SetActive((int)((invincibleTime - 0.25f) * 8) % 2 == 0);
        }
    }

    public void ChangeLife(int amount)
    {
        int oldLife = life;
        life = Mathf.Clamp(life + amount, 0, maxLife);

        if (oldLife != 0 && amount > 0) // oldLife == 0 となるのはライフ初期化時なのでログは出さない
        {
            if (life > oldLife) LogManager.Instance.Register(LogType.Heart);
            else LogManager.Instance.Register(LogType.HeartMax);
        }

        if (life < oldLife)
        {
            SoundManager.Instance.PlaySE("TakeDamage");
            LogManager.Instance.Register(LogType.TakeDamage);
        }

        onLifeChanged.Invoke(life);

        if (life == 0)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        else if (amount < 0) 
        {
            invincibleTime = InvincibleTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnEnter(collision.transform.GetComponentInParent<CollisionComponent>());
    }

    void OnTriggerEnter(Collider other)
    {
        OnEnter(other.GetComponentInParent<CollisionComponent>());
    }

    void OnEnter(CollisionComponent collisionComponent)
    {
        if (!collisionComponent) return;
        if (collisionComponent.Contains(CollisionType.DamageToPlayer))
        {
            if (!collisions.Contains(collisionComponent)) collisions.Add(collisionComponent);

            if (invincibleTime <= 0) ChangeLife(-1);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        OnExit(collision.transform.GetComponentInParent<CollisionComponent>());
    }

    void OnTriggerExit(Collider other)
    {
        OnExit(other.GetComponentInParent<CollisionComponent>());
    }

    void OnExit(CollisionComponent collisionComponent)
    {
        if (!collisionComponent) return;
        if (collisionComponent.Contains(CollisionType.DamageToPlayer)) collisions.Remove(collisionComponent);
    }

    public void AddOnLifeChanged(Action<int> action)
    {
        onLifeChanged += action;
    }
}
