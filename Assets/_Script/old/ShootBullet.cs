using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    [SerializeField] Camera camera_2d;
    [SerializeField] GameObject bullet;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera_2d.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit: " + hit.collider.name);
                Vector3 targetPosition = hit.point;
                Vector3 mouseDir = (targetPosition - bullet.transform.position).normalized;
                mouseDir.y = 0;
                Instantiate(bullet, transform.position + transform.forward, Quaternion.LookRotation(mouseDir));
            }
        }
    }
}
