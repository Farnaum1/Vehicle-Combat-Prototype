using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private float radius = 1f;

    public float Radius => radius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}
