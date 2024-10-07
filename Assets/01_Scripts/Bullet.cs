using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5;
    public float damage = 1;
    public float timeToDestroy = 4;
    public bool playerBullet = false;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && playerBullet)
        {
            Enemy e = collision.gameObject.GetComponent<Enemy>();
            e.TakeDamage(damage);
            Destroy(gameObject);
        }else if (collision.gameObject.CompareTag("Player") && !playerBullet)
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Boss") && playerBullet)
        {
            Boss b = collision.gameObject.GetComponent<Boss>();
            b.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}