using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    // Start is called before the first frame update
    public byte type;
    void Start()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            switch (type)
            {
                case 0:
                    p.extraDamage = 5f;
                    break;
                case 1:
                    p.ExtraSpeed = 2f;
                    break;
                case 2:
                    p.extraBulletSpeed = 10f;
                    break;
                case 3:
                    p.critic = true;
                    break;
                case 4:
                    p.timeBtwShoot = 0f;
                    break;
                case 5:
                    p.shield = true;
                    break;
                default:
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}
