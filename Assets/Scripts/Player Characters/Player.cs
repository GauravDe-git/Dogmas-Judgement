using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
     private int maxHealth = 100;
    [SerializeField] public int currentHealth;
    private int attack1Damage = 10;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip swordSwingSFX;
    [SerializeField] private AudioClip playerDamagedSFX;
    [SerializeField] private AudioClip transformSFX;

    [Header("Ez Find GameObject")] 
    [SerializeField] private CinemachineVirtualCamera runCamera;
    [SerializeField] private CinemachineVirtualCamera idleCamera;
    [SerializeField] private Transform flameHeroPos;
    [SerializeField] private Transform iceHeroPos;
    [SerializeField] private Transform thunderHeroPos;

    [Header("Health Portraits")] 
    [SerializeField] private Image healthPortrait;
    [SerializeField] private Sprite flameHeroPortrait;
    [SerializeField] private Sprite iceHeroPortrait;
    [SerializeField] private Sprite thunderHeroPortrait;
    
    //States
    


    //cached references
    private Rigidbody2D _myRigidbody2D;
    private Animator _myAnimator;
    private Collider2D _myCollider2D;
    private SpriteRenderer _spriteRenderer;
    private NPC_Controller npc;
    public HealthBar HealthBar;


    private void Start()
    {
        _myRigidbody2D = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _myCollider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (!inDialogue())
        {
            Run();
            FlipSprite();
            Jump();
        }

        Attack();
        ReloadLevel();
        //TransformFlame();
    }

    private void ReloadLevel()
    {
        if (transform.position.y < -8)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Run()
    {
        float move = Input.GetAxis("Horizontal");
        if (!this._myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        { 
            _myRigidbody2D.velocity = new Vector2(move * moveSpeed, _myRigidbody2D.velocity.y);
        }

        bool isMoving = Mathf.Abs(_myRigidbody2D.velocity.x) > Mathf.Epsilon;
        _myAnimator.SetBool("isRunning", isMoving);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && _myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            StartCoroutine(JumpAnimation());
            Vector2 jumpy = new Vector2(0f, jumpSpeed);
            _myRigidbody2D.velocity += jumpy;
        }
    }

    private IEnumerator JumpAnimation()
    {
        _myAnimator.SetBool("isJumping", true);
        yield return new WaitForSeconds(0.47f);
        _myAnimator.SetBool("isJumping", false);
    }

    private void FlipSprite()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else if(Input.GetAxis("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && _myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")) 
                                         && !this._myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            _myAnimator.SetTrigger("Attack");
            AudioSource.PlayClipAtPoint(swordSwingSFX,Camera.main.transform.position,0.6f);
            _myRigidbody2D.velocity = Vector2.zero;
            
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyBug>().TakeDamageAndDie(attack1Damage);
            }
        }
    }

    public void GetDamagedAndDie(int enemyATK)
    {
        AudioSource.PlayClipAtPoint(playerDamagedSFX,Camera.main.transform.position,0.7f);
        StartCoroutine(HurtColor());
        currentHealth -= enemyATK;
        HealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position,attackRange);
    }
    
    private IEnumerator HurtColor() {
        for (int i = 0; i < 5; i++) {
            _spriteRenderer.color = new Color (1f, 1f, 1f, 0.3f); //Red, Green, Blue, Alpha/Transparency
            yield return new WaitForSeconds (.1f);
            _spriteRenderer.color = Color.white; //White is the default "color" for the sprite, if you're curious.
            yield return new WaitForSeconds (.1f);
        }
    }

    public void TransformFlame()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        {
            AudioSource.PlayClipAtPoint(transformSFX,Camera.main.transform.position,1f);
            _myAnimator.SetTrigger("Transform_Flame");
            StartCoroutine(FlameHeroAppear());
            healthPortrait.sprite = flameHeroPortrait;
        }
    }

    private IEnumerator FlameHeroAppear()
    {
        yield return new WaitForSeconds(1.2f);
        flameHeroPos.gameObject.SetActive(true);
        flameHeroPos.position = new Vector2(transform.position.x,transform.position.y);  
        runCamera.m_Follow = flameHeroPos;
        idleCamera.m_Follow = flameHeroPos;
        gameObject.SetActive(false);
    }

    public void TransformIce()
    {
        AudioSource.PlayClipAtPoint(transformSFX,Camera.main.transform.position,1f);
        _myAnimator.SetTrigger("Transform_Ice");
        StartCoroutine(IceHeroAppear());
        healthPortrait.sprite = iceHeroPortrait;
    }

    private IEnumerator IceHeroAppear()
    {
        yield return new WaitForSeconds(1.2f);
        iceHeroPos.gameObject.SetActive(true);
        iceHeroPos.position = new Vector2(transform.position.x,transform.position.y);
        runCamera.m_Follow = iceHeroPos;
        idleCamera.m_Follow = iceHeroPos;
        gameObject.SetActive(false);
    }

    public void TransformThunder()
    {
        AudioSource.PlayClipAtPoint(transformSFX,Camera.main.transform.position,1f);
        _myAnimator.SetTrigger("Transform_Thunder");
        StartCoroutine(ThunderHeroAppear());
        healthPortrait.sprite = thunderHeroPortrait;
    }

    private IEnumerator ThunderHeroAppear()
    {
        yield return new WaitForSeconds(1.2f);
        thunderHeroPos.gameObject.SetActive(true);
        thunderHeroPos.position = new Vector2(transform.position.x,transform.position.y);
        runCamera.m_Follow = thunderHeroPos;
        idleCamera.m_Follow = thunderHeroPos;
        gameObject.SetActive(false);
    }

    private bool inDialogue()
    {
        if (npc != null) 
            return npc.DialogueActive();
        else
        {
            return false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            npc = other.gameObject.GetComponent<NPC_Controller>();
            if (Input.GetKey(KeyCode.R))
            {
                npc.ActivateDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        npc = null;
    }
    
}

