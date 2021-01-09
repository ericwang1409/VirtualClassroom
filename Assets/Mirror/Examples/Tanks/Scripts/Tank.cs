using UnityEngine;
using UnityEngine.AI;

namespace Mirror.Examples.Tanks
{
    public class Tank : NetworkBehaviour
    {
        private bool sitting = false;

        [Header("Components")]
        public NavMeshAgent agent;
        public Animator animator;

        [Header("Movement")]
        public float rotationSpeed = 100;

        [Header("Firing")]
        public KeyCode shootKey = KeyCode.Space;
        public GameObject projectilePrefab;
        public Transform projectileMount;

        void Update()
        {
            // movement for local player
            if (!isLocalPlayer) return;

            // rotate
            float horizontal = Input.GetAxis("Horizontal");
            transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

            // move
            float vertical = Input.GetAxis("Vertical");
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            agent.velocity = forward * Mathf.Max(vertical, 0) * agent.speed;
            //animator.SetBool("Moving", agent.velocity != Vector3.zero);

            if (agent.velocity != new Vector3(0, 0, 0))
            {
                animator.SetBool("Walk", true);
                //Debug.Log("here");
            }
            else
                animator.SetBool("Walk", false);

            if (!sitting && Input.GetKeyDown("space"))
            {
                animator.SetBool("Sit", true);
                sitting = true;
            }
            else if (sitting && Input.GetKeyDown("space"))
            {
                animator.SetBool("Sit", false);
                sitting = false;
            }

            // shoot
            if (Input.GetKeyDown(shootKey))
            {
                CmdFire();
            }
        }

        // this is called on the server
        [Command]
        void CmdFire()
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        // this is called on the tank that fired for all observers
        [ClientRpc]
        void RpcOnFire()
        {
            //animator.SetTrigger("Shoot");
        }
    }
}
