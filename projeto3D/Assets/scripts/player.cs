using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private CharacterController controller;

    public float speed;
    public float gravity;
    public float colliderRadius;
    public float damege = 20;
    public float totalHealth;
    
    private Animator anim;
    private bool isWalking;
    private bool waitFor;
    public List<Transform> enemyList = new List<Transform>();

    public Transform cam;
    Vector3 moveDirection;

    public float smoothRotTime;

    public float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GetMouseInput();
    }

    void Move()
    {
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if (direction.magnitude > 0)
            {
                if (!anim.GetBool("attacking"))
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float smoothAngle =
                        Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                    moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;

                    anim.SetInteger("transition", 1);

                    isWalking = true;
                }
                else
                {
                    anim.SetBool("walking", false);
                    moveDirection = Vector3.zero;
                }
                
            }

            else if(isWalking)
            {
                // é executado qunado o player está parado
                
                anim.SetBool("walking", false);
                anim.SetInteger("transition", 0);
                moveDirection = Vector3.zero;

                isWalking = false;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("walking"))
                {
                    anim.SetBool("walking", false);
                    anim.SetInteger("transtion", 0);
                }

                if (!anim.GetBool("walking"))
                {
                    StartCoroutine("Attack");
                }
                
            }
        }
    }

    IEnumerator Attack()
    {
        if (!waitFor)
        {
            waitFor = true;
            anim.SetBool("attacking", true);
            anim.SetInteger("transition", 2);

            yield return new WaitForSeconds(0.4f);

            GetEnemiesList();

            foreach (Transform e in enemyList)
            {
                // aplica dano ao inimigo
                CombatEnemy enemy = e.GetComponent<CombatEnemy>();

                if (enemy != null)
                {
                    enemy.GetHit(damege);
                }
            }

            yield return new WaitForSeconds(1f);

            anim.SetInteger("transition", 0);
            anim.SetBool("attacking", false);
            waitFor = false;

        }
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }

    }
    
    public void GetHit(float damege)
    {
        totalHealth -= damege;
        if (totalHealth > 0)
        {
            // inimigo ainda tá vivo
            StopCoroutine("Attack");
            anim.SetInteger("transition", 3);
            
            StartCoroutine("RecoveryFromHit");

        }
        else
        {
            //inimigo morre
            anim.SetTrigger("Die");
        }
    }

    IEnumerator RecoveryFromHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Bite Attack", false);
       

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}