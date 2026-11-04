
using UnityEngine;

public class BillboardIcon : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool invertForward = false;

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        Vector3 direction = targetCamera.transform.position - transform.position;

        if (invertForward)
            direction = -direction;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
