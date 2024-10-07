using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public EnemyType type;
    public float MaxLife = 10;
    float life = 3;
    public float damage = 1;
    public float speed = 2f;
    public float timeBtwShoot = 1.5f;
    public float BulletSpeed = 2f;
    public int ExtraDamage = 1;
    public int ExtraSpeed = 1;
    public int scorePoints = 1;
    float timer = 0;
    public float range = 4;
    public float dropChance = 30f;
    bool targetInRange = false;
    Transform target;
    public Transform firePoint1;
    public Bullet bulletPrefab;
    public GameObject explosionEffect;
    public List<GameObject> powerUpPrefabs;
    public Image LifeBar;
    void Start()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Player");
        target = ship.transform;
        life = MaxLife;
        LifeBar.fillAmount = life/MaxLife;
    }

    void Update()
    {
        switch (type)
        {
            case EnemyType.Normal:
                MoveForward();
                break;
            case EnemyType.NormalShoot:
                MoveForward();
                Shoot(ExtraDamage,ExtraSpeed);
                break;
            case EnemyType.Kamikase:
                if (targetInRange)
                {
                    RotateToTarget();
                    MoveForward();
                }
                else
                {
                    MoveForward();
                    SearchTarget();
                }
                break;
            case EnemyType.Sniper:
                if (targetInRange)
                {
                    RotateToTarget();
                    Shoot(ExtraDamage, ExtraSpeed);
                }
                else
                {
                    MoveForward();
                    SearchTarget();
                }
                break;
        }
    }
    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Enemy e = collision.gameObject.GetComponent<Enemy>();
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
    public void TakeDamage(float dmg)
    {
        life -= dmg;
        LifeBar.fillAmount = life / MaxLife;
        if (life <= 0)
        {
            Vector3 v = gameObject.transform.position;
            if(Random.Range(0,100) <= dropChance)
            {
                int power = Random.Range(0, powerUpPrefabs.Count);
                Instantiate(powerUpPrefabs[power], transform.position, transform.rotation);
            }
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Spawner.instance.AddScore(scorePoints);
            Destroy(gameObject);
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void MoveForward(float m)
    {
        transform.Translate(Vector2.up * speed * m * Time.deltaTime);
    }

    void RotateToTarget()
    {
        Vector2 dir = target.position - transform.position;
        float angleZ = Mathf.Atan2 (dir.x, dir.y) * Mathf.Rad2Deg + 0;
        transform.rotation = Quaternion.Euler(0 , 0, -angleZ);
    }

    void SearchTarget()
    {
        float distance = Vector2.Distance(target.position, transform.position);
        if (distance <= range)
        {
            targetInRange = true;
        }
        else
        {
            targetInRange = false;
        }
    }

    void Shoot(int x, int y)
    {
        if(timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Bullet b=Instantiate(bulletPrefab, firePoint1.position, transform.rotation);
            b.speed = BulletSpeed*y;
            b.damage = damage*x;

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}

public enum EnemyType
{
    Normal,
    NormalShoot,
    Kamikase,
    Sniper
}