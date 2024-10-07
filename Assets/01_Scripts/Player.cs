using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public float MaxLife;
    float life = 3;
    public float speed = 2f;
    public float ExtraSpeed = 0f;
    public float timeBtwShoot = 1.5f;
    public int bullets = 5;
    public float extraDamage = 0f;
    public float damage= 5f;
    public bool critic = false;
    public float bulletSpeed= 10f;
    public float extraBulletSpeed = 0f;
    int currentBullets;
    float timer = 0;
    float timer2 = 0;
    public bool shield;
    bool canShoot = true;
    public Rigidbody2D rb;
    public Transform firePoint;
    public Bullet bulletPrefab;
    public Text lifeText;
    public Image lifeBar;
    void Start()
    {
        Debug.Log("Inició el juego");
        currentBullets = bullets;
        life = MaxLife;
        lifeBar.fillAmount = life / MaxLife;
        lifeText.text = "Life = " + life;
    }

    void Update()
    {
        Debug.Log("Juego en progreso");
        Movement();
        Reload();
        CheckIfShoot();
        Shoot();
        if (shield)
        {
            timer2 += Time.deltaTime;
        }
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(x, y) * (speed+ExtraSpeed);
    }

    void Shoot()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canShoot && currentBullets > 0)
        {
            Bullet b = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            if (critic)
            {
                int prob = Random.Range(1, 10);
                if (prob < 4)
                {
                    b.damage = (damage + extraDamage)*2;
                }
                else
                {
                    b.damage = damage + extraDamage;
                }
            }
            else
            {
                b.damage = damage + extraDamage;
            }
            b.speed= bulletSpeed + extraBulletSpeed;
            canShoot = false;
            currentBullets--;
        }
    }

    void Reload()
    {
        if(currentBullets == 0 && Input.GetKeyDown(KeyCode.R))
        {
            currentBullets = bullets;
        }
    }

    public void TakeDamage(float dmg)
    {
        if (shield && timer2 < 6)
        {
            timer2 += Time.deltaTime;
        }
        else
        {
            timer2 = 0;
            shield = false;
            life -= dmg;
            lifeBar.fillAmount = life / MaxLife;
            lifeText.text = "Life = " + life;
        }
        
        if (life <= 0)
        {
            //Destroy(gameObject);
            SceneManager.LoadScene("Game");
        }
    }

    void CheckIfShoot()
    {
        if (!canShoot)
        {
            if (timer < timeBtwShoot)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                canShoot = true;
            }
        }
    }

}