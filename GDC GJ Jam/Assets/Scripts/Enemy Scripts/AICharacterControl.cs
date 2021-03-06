using System;
using UnityEngine;

[RequireComponent(typeof (NavMeshAgent))]
[RequireComponent(typeof (ThirdPersonCharacter))]
public class AICharacterControl : MonoBehaviour
 {
     public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
     public ThirdPersonCharacter character { get; private set; } // the character we are controlling
     public Transform target;      
     // target to aim for
    private float damage;
    public float health;
     private void Start()
     {
         // get the components on the object we need ( should not be null due to require component so no need to check )
         agent = GetComponentInChildren<NavMeshAgent>();
         character = GetComponent<ThirdPersonCharacter>();

        agent.updateRotation = false;
        agent.updatePosition = true;
     }


     private void Update()
     {
        if (health < 0)
        {
            tag = "DeadEnemy";
            return;
        }
            
         if (target != null)
             agent.SetDestination(target.position);

         if (agent.remainingDistance > agent.stoppingDistance)
             character.Move(agent.desiredVelocity, false, false);
         else
             character.Move(Vector3.zero, false, false);
     }


     public void SetTarget(Transform target)
     {
         this.target = target;
     }

    public void Attack(GameObject obj)
    {

    }

    public void StrikeCurrentTarget(float dmg)
    {
        if (target)
        {
            Health health = target.GetComponent<Health>();
            if(health) { health.DealDamage(dmg); }

        }
        
    }

    public void OnCollisionEnter(Collision info)
    {
        if(info.collider.tag == "Weapon")
        {
            Animator ani = GetComponent<Animator>();
            ani.SetFloat("Health", -1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            Animator ani = GetComponent<Animator>();
            ani.SetFloat("Health", health);
            Rigidbody body = null;
            health -= 50;
            ani.SetFloat("Health", health);

        }
    }

 }
