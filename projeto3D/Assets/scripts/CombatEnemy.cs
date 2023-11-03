using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatEnemy : MonoBehaviour
{ 
    [Header("Atributos")]
    public float totalHealth;

    public float attackDamage;

    public float movementSpeed;
    public float lookRadius;
    public float colliderRadius = 2f;

    [Header("Componentes")] 
    private Animator anim;

    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    [Header("Others")] 
    public Transform player;

    private bool walking;
    private bool attacking;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= lookRadius)
        {
             // dentro do raio de ação
             agent.isStopped = false;

             if (!attacking)
             {
                 agent.SetDestination(player.position);
                 anim.SetBool("Walk Forward", true);
                 walking = true;
             }
             
             
            if (distance <= agent.stoppingDistance)
            {
                Debug.Log("atacar!");
                // raio de ataque
                // metodo de ataque
                
                agent.isStopped = true;
            }
            else
            {
                attacking = false;
            }
            
        }
        else
        {
            // fora do raio de ataque 
            agent.isStopped = true;
            anim.SetBool("Walk Forward", false);
            walking = false;
            attacking = false;
        }
    }
    
    IEnumerator Attack()
    {
        
        yield return new WaitForSeconds(0.4f);

        GetEnemiesList();

       
        
        yield return new WaitForSeconds(1f);
        
      
            
        
    }

    void GetEnemiesList()
    {

        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
               
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
