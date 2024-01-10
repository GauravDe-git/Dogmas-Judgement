using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBug : MonoBehaviour
{ 
    [SerializeField] private Transform pointA, pointB;
    
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int attackDamage = 5;

    [SerializeField] private AudioClip enemyDeathSFX;
    [SerializeField] private GameObject enemyDeathVFX;

    private float moveSpeed = 2f;
    private Transform _target;
    private Camera _camera;

    private SpriteRenderer enemySprite;
    private Player _player;

    private void Start()
    {
        _camera = Camera.main;
        currentHealth = maxHealth;
        enemySprite = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        Movement();
        FlipSprite();
    }

    private void Movement()
    {
        if (transform.position == pointB.position)
        {
            _target = pointA;
        }
        else if (transform.position == pointA.position)
        {
            _target = pointB;
        }

        transform.position = Vector2.MoveTowards(transform.position, _target.position,
            moveSpeed * Time.deltaTime);
    }

    private void FlipSprite()
    {
        if (_target == pointB)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        else if (_target == pointA)
        {
            transform.localScale = new Vector3(1,1,1);
        }
    }

    public void TakeDamageAndDie(int damage)
    {
        StartCoroutine(HurtColor());
        currentHealth -= damage;
        AudioSource.PlayClipAtPoint(enemyDeathSFX,_camera.transform.position,0.7f);
        if (currentHealth <= 0)
        {
            Destroy(gameObject,0.4f);
            AudioSource.PlayClipAtPoint(enemyDeathSFX,_camera.transform.position,1f);
            StartCoroutine(BloodSplash());
        }
    }

    private IEnumerator BloodSplash()
    {
        yield return new WaitForSeconds(0.36f);
        Instantiate(enemyDeathVFX, transform.position, quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().GetDamagedAndDie(attackDamage);
            other.attachedRigidbody.velocity = new Vector2(10f,10f);
        }
    }
    
    private IEnumerator HurtColor() {
        for (int i = 0; i < 5; i++) {
            enemySprite.color = new Color (1f, 1f, 1f, 0.3f); //Red, Green, Blue, Alpha/Transparency
            yield return new WaitForSeconds (.1f);
            enemySprite.color = Color.white; //White is the default "color" for the sprite, if you're curious.
            yield return new WaitForSeconds (.1f);
        }
    }
}

