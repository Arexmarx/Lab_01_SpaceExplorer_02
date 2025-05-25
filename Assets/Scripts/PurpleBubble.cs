using UnityEngine;

public class PurpleBubble : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().ActivateShield();
            Destroy(gameObject);
        }
    }
}
