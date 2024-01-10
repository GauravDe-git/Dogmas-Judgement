using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderHero : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private GameObject thunderBallPrefab;
    [SerializeField] private Transform thunderballPos;
    
    [Header("Health")]
    [SerializeField] private HealthBar _healthBar;

    [Header("Audio")] 
    [SerializeField] private AudioClip damagedSFX;
    [SerializeField] private AudioClip thunderBallSFX;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Run();
        FlipSprite();
        Jump();
        StartCoroutine(FireBallAttack());
        
    }


    private void Run()
    { 
        float move = Input.GetAxis("Horizontal");
        _rigidbody2D.velocity = new Vector2(move * moveSpeed, _rigidbody2D.velocity.y);
        bool isMoving = Mathf.Abs(_rigidbody2D.velocity.x) > Mathf.Epsilon;
        _animator.SetBool("isWalking", isMoving);
    }
    
    
    private void FlipSprite()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if(Input.GetAxis("Horizontal") < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
    
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && _collider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Vector2 jumpy = new Vector2(0f, jumpSpeed);
            _rigidbody2D.velocity += jumpy;
        }
    }

    private IEnumerator FireBallAttack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            AudioSource.PlayClipAtPoint(thunderBallSFX,Camera.main.transform.position,0.7f);
            _animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(0.7f);
            Instantiate(thunderBallPrefab, thunderballPos.position, transform.rotation);
            _animator.SetBool("isAttacking", false);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBug"))
        {
            _healthBar.ReduceHealth(5);
            AudioSource.PlayClipAtPoint(damagedSFX,Camera.main.transform.position,1f);
            _rigidbody2D.velocity = new Vector2(10f,10f);
            StartCoroutine(HurtColor());
        }
    }
    
    private IEnumerator HurtColor() {
        for (int i = 0; i < 5; i++) {
            _spriteRenderer.color = new Color (1f, 1f, 1f, 0.3f); //Red, Green, Blue, Alpha/Transparency
            yield return new WaitForSeconds (.1f);
            _spriteRenderer.color = Color.white; //White is the default "color" for the sprite, if you're curious.
            yield return new WaitForSeconds (.1f);
        }
    }
}
