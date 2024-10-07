using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importante para trabajar con UI

public class Spawner : MonoBehaviour
{
    // Instancia privada y estática del Spawner
    private static Spawner instance = null;

    // Objeto para sincronización de hilos
    private static readonly object padlock = new object();

    // Propiedad pública para obtener la instancia única del Spawner
    public static Spawner Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    // Buscar una instancia existente en la escena
                    instance = FindObjectOfType<Spawner>();

                    // Si no hay ninguna instancia en la escena, crear un nuevo GameObject con Spawner
                    if (instance == null)
                    {
                        GameObject spawnerObject = new GameObject("Spawner");
                        instance = spawnerObject.AddComponent<Spawner>();
                    }
                }
                return instance;
            }
        }
    }

    // Configuración de tiempo y generación de enemigos
    public float timeBtwSpawn = 2.3f;
    private float timer = 0;
    public Transform leftPoint;
    public Transform rightPoint;
    public List<GameObject> enemyPrefabs;

    public int wave = 1; // Contador de oleadas
    private int totalWaves = 5; // Total de oleadas
    public int enemiesPerWave = 2; // Número de enemigos por ola
    private int enemiesSpawned = 0; // Contador de enemigos generados en la oleada actual

    // Referencia al texto en el Canvas
    public Text waveText;

    // Referencia al prefab del Boss
    public GameObject bossPrefab; // Prefab del jefe
    private bool bossSpawned = false; // Indica si el jefe ya fue generado

    void Awake()
    {
        // Asegurar que solo haya una instancia y destruir duplicados
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el Spawner entre escenas si es necesario
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(ShowWaveMessage()); // Mostrar el mensaje al inicio
    }

    void Update()
    {
        if (wave <= totalWaves) // Si no hemos alcanzado la última ola
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (timer < timeBtwSpawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            if (enemiesSpawned < enemiesPerWave)
            {
                float x = Random.Range(leftPoint.position.x, rightPoint.position.x);

                // Obtener el rango de enemigos según la ola
                int minEnemyIndex = GetMinEnemyIndexForWave();
                int maxEnemyIndex = GetMaxEnemyIndexForWave();

                // Selecciona un enemigo aleatorio entre el rango de enemigos disponibles para la ola actual
                int enemy = Random.Range(minEnemyIndex, maxEnemyIndex + 1);
                Vector3 newPos = new Vector3(x, transform.position.y, transform.position.z);

                // Instancia el enemigo
                Instantiate(enemyPrefabs[enemy], newPos, Quaternion.Euler(0, 180, 0));

                enemiesSpawned++; // Incrementar el contador de enemigos generados
            }
            else
            {
                NextWave(); // Pasar a la siguiente ola
            }
        }
    }

    int GetMinEnemyIndexForWave()
    {
        switch (wave)
        {
            case 1: return 0;  // Solo enemigos de tipo 1 (posición 0)
            case 2: return 0;  // Enemigos de tipo 1 y 2 (posición 0-1)
            case 3: return 1;  // Enemigos de tipo 2 y 3 (posición 1-2)
            case 4: return 2;  // Enemigos de tipo 3 y 4 (posición 2-3)
            case 5: return 0;  // Todos los enemigos (posición 0 en adelante)
            default: return 0;
        }
    }

    int GetMaxEnemyIndexForWave()
    {
        switch (wave)
        {
            case 1: return 0;  // Solo enemigos de tipo 1 (posición 0)
            case 2: return 1;  // Enemigos de tipo 1 y 2 (posición 0-1)
            case 3: return 2;  // Enemigos de tipo 2 y 3 (posición 1-2)
            case 4: return 3;  // Enemigos de tipo 3 y 4 (posición 2-3)
            case 5: return enemyPrefabs.Count - 1; // Todos los enemigos disponibles
            default: return 0;
        }
    }

    void NextWave()
    {
        if (wave < totalWaves)
        {
            wave++; // Pasar a la siguiente ola
            enemiesSpawned = 0; // Reiniciar el contador de enemigos para la nueva ola

            // Mostrar el mensaje de la nueva oleada
            StartCoroutine(ShowWaveMessage());
        }
        else if (!bossSpawned) // Solo generar el jefe si aún no ha sido generado
        {
            SpawnBoss(); // Generar al jefe
        }
    }

    // Método para generar al jefe
    void SpawnBoss()
    {
        Debug.Log("Generando al jefe...");
        float x = (leftPoint.position.x + rightPoint.position.x) / 2; // Posición centrada para el jefe
        Vector3 bossPos = new Vector3(x, transform.position.y, 0);

        Instantiate(bossPrefab, bossPos, Quaternion.identity); // Genera el jefe
        bossSpawned = true; // Asegurar que solo se genere una vez
    }

    // Coroutine para mostrar el mensaje de la oleada y ocultarlo después de 2 segundos
    IEnumerator ShowWaveMessage()
    {
        waveText.text = "Oleada " + wave; // Actualizar el texto con el número de la oleada
        waveText.gameObject.SetActive(true); // Mostrar el texto
        yield return new WaitForSeconds(2); // Esperar 2 segundos
        waveText.gameObject.SetActive(false); // Ocultar el texto
    }
}
