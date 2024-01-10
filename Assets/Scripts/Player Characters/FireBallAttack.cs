using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FireBallAttack : MonoBehaviour
{
    [SerializeField] private AudioClip enemyDieSFX;
    [SerializeField] private GameObject enemyDieVfx;
    
    
    
    
    private FlameHero _flameHero;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _flameHero = GetComponent<FlameHero>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = transform.right * 5f;
        Destroy(gameObject,10f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBug"))
        {
            Instantiate(enemyDieVfx, transform.position, quaternion.identity);
            AudioSource.PlayClipAtPoint(enemyDieSFX, Camera.main.transform.position, 0.7f);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
    
}
