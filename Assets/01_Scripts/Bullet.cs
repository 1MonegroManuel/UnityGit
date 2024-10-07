using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float timeToDestroy = 4f;
    public float damage = 1;
    public bool playerBullet = false;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && playerBullet)
        {
            // Intentar obtener el componente Boss si está presente
            EnemyBoss boss = collision.gameObject.GetComponent<EnemyBoss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject);
            }
            else
            {
                // Intentar obtener el componente Enemy si Boss no está presente
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("Componente Enemy o Boss no encontrado en: " + collision.gameObject.name);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Player") && !playerBullet)
        {
            Player p = collision.gameObject.GetComponent<Player>();
            if (p != null)
            {
                p.TakeDamage(damage);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Componente Player no encontrado en: " + collision.gameObject.name);
            }
        }
    }
}
