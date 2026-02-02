using UnityEngine;

public class ReturnToPoolDelay : MonoBehaviour
{
    [SerializeField] private float delay = 1.0f;

    private void OnEnable()
    {
        Invoke(nameof(Return), delay);
    }

    private void Return()
    {
        GetComponent<PoolMember>().ReturnToPool();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}