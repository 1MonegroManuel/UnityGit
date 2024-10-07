using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyBoss : MonoBehaviour
{
    public float maxLife = 100;
    private float life;
    public float speed = 2f;
    public float timeBtwShoot = 1.5f;
    public float timeBtwShoot2 = 1.8f;
    public float bigBulletShootInterval = 2f;
    private float timer = 0;
    private float bigBulletTimer = 0;
    public Transform firePoint;
    public Transform firePoint2;
    public Bullet bulletPrefab;
    public Bullet bigBulletPrefab;
    public Image lifeBar;

    public Transform teleportPosition1;
    public Transform teleportPosition2;
    public float teleportMoveSpeed = 1f;

    public GameObject enemyObject; // Objeto que tiene el SpriteRenderer del enemigo
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer para cambiar el color

    public enum BossPhase
    {
        Phase1,
        Phase2,
        Phase3,
        Phase4
    }

    public BossPhase currentPhase = BossPhase.Phase1;
    public float phase2Threshold = 75f;
    public float phase3Threshold = 50f;
    public float phase4Threshold = 25f;

    private Transform target;
    private bool movingRight = true;
    private bool isTeleporting = false;

    public Text victoryText; // Referencia al texto de victoria

    // Variables adicionales para el manejo de daño e inmunidad
    private bool canTakeDamage = true; // Variable para controlar si puede recibir daño
    private bool isShooting = false; // Variable para controlar si está disparando
    public Color immuneColor = Color.red; // Color para cuando es inmune
    public Color vulnerableColor = Color.white; // Color para cuando es vulnerable

    void Start()
    {
        life = maxLife;
        lifeBar.fillAmount = life / maxLife;
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Inicializar el spriteRenderer del objeto enemigo
        if (enemyObject != null)
        {
            spriteRenderer = enemyObject.GetComponent<SpriteRenderer>();
        }

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false); // Asegurarse de que el texto está oculto al inicio
        }
    }

    void Update()
    {
        UpdatePhase();
        HandlePhaseBehavior();
    }

    void UpdatePhase()
    {
        if (life <= phase4Threshold)
        {
            currentPhase = BossPhase.Phase4;
        }
        else if (life <= phase3Threshold)
        {
            currentPhase = BossPhase.Phase3;
        }
        else if (life <= phase2Threshold)
        {
            currentPhase = BossPhase.Phase2;
        }
    }

    void HandlePhaseBehavior()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                Phase1Movement();
                Shoot();
                break;
            case BossPhase.Phase2:
                Phase2Positioning();
                ShootAtPlayer();
                break;
            case BossPhase.Phase3:
                Phase1Movement();
                RotateTowardsZero(); // Rotación fija en 0 para fase 3
                ShootWithDifferentPrefab();
                break;
            case BossPhase.Phase4:
                MoveToCenter();  // Agrega esta línea para que se mueva al centro
                RotateTowardsTarget(); // Apuntar hacia el jugador en fase 4
                if (!isShooting)
                {
                    StartCoroutine(Phase4ShootingRoutine());
                }
                break;
        }
    }
    void MoveToCenter()
    {
        // Ajusta la posición del jefe al centro (0,0)
        transform.position = new Vector3(0, transform.position.y, 0);
    }


    void RotateTowardsZero()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0); // Rotación fija a 0
    }

    void Phase1Movement()
    {
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (transform.position.x >= 7 || transform.position.x <= -7)
        {
            movingRight = !movingRight;
        }
    }

    void Phase2Positioning()
    {
        transform.position = new Vector3(0, transform.position.y, 0);
        RotateTowardsTarget();
    }

    void RotateTowardsTarget()
    {
        Vector2 directionToPlayer = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 270));
    }

    void Shoot()
    {
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Bullet b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            b.GetComponent<Rigidbody2D>().velocity = firePoint.up * b.speed;
        }
    }

    void ShootAtPlayer()
    {
        if (timer < timeBtwShoot * 0.5f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Bullet b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Vector2 direction = (target.position - firePoint.position).normalized;
            b.GetComponent<Rigidbody2D>().velocity = direction * b.speed;
        }
    }

    void ShootWithDifferentPrefab()
    {
        if (timer < timeBtwShoot2)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Bullet b = Instantiate(bigBulletPrefab, firePoint.position, firePoint.rotation);
            b.GetComponent<Rigidbody2D>().velocity = firePoint.up * b.speed;
        }
    }

    IEnumerator Phase4ShootingRoutine()
    {
        isShooting = true;
        canTakeDamage = false; // Hacer inmune durante los disparos
        spriteRenderer.color = immuneColor; // Cambiar color a inmune

        // Disparar 5 balas
        for (int i = 0; i < 5; i++)
        {
            ShootSingleBullet(); // Llamar a tu función de disparo
            yield return new WaitForSeconds(0.3f); // Esperar 0.3 segundos entre cada disparo
        }

        // Esperar 2 segundos sin disparar y vulnerable
        spriteRenderer.color = vulnerableColor; // Cambiar color a vulnerable
        canTakeDamage = true; // Hacerlo vulnerable
        yield return new WaitForSeconds(2f);

        isShooting = false; // Reiniciar el ciclo de disparo
    }

    void ShootSingleBullet()
    {
        Bullet b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        b.GetComponent<Rigidbody2D>().velocity = firePoint.up * b.speed;
    }

    public void TakeDamage(float dmg)
    {
        if (canTakeDamage) // Solo recibir daño si es vulnerable
        {
            life -= dmg;
            lifeBar.fillAmount = life / maxLife;

            if (life <= 0)
            {
                if (victoryText != null)
                {
                    StartCoroutine(DisplayVictoryMessage());
                }
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DisplayVictoryMessage()
    {
        Debug.Log("El jefe ha sido derrotado");
        Debug.Log("Activando el texto de victoria");

        if (victoryText != null)
        {
            victoryText.text = "¡Ganaste!";
            victoryText.gameObject.SetActive(true);
            Debug.Log("Texto activado");
        }
        else
        {
            Debug.LogError("No se encontró el componente 'victoryText'");
        }

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Game");
    }
}
