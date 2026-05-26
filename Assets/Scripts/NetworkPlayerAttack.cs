using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerAttack : NetworkBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 2f;
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (NetworkPlayerInput.WasAttackPressed())
        {
            RequestAttackServerRpc();
        }
    }

    [ServerRpc]
    private void RequestAttackServerRpc()
    {
        Vector3 attackCenter = transform.position + transform.forward;
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRange);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }

            if (!hit.CompareTag("Player"))
            {
                continue;
            }

            NetworkPlayerHealth targetHealth = hit.GetComponent<NetworkPlayerHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}
