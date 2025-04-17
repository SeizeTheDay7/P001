using UnityEngine;

public class PlayerMove3D : MonoBehaviour
{
    [SerializeField] Camera camera_3d;

    [Header("Speeds")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 5f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Move();
    }

    private void Move()
    {
        Vector3 dir = MoveDir_3d();
        dir.y = -1f;
        dir.Normalize();

        Vector3 targetPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    private Vector3 MoveDir_3d()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction += -transform.forward;
        if (Input.GetKey(KeyCode.A)) direction += -transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        return direction;
    }

    void Update()
    {
        Rotate_3d();
    }

    private void Rotate_3d()
    {
        Vector3 player_rot = transform.eulerAngles;
        player_rot.y += Input.GetAxis("Mouse X") * rotateSpeed;
        transform.eulerAngles = player_rot;

        Vector3 camera_rot = camera_3d.transform.eulerAngles;
        camera_rot.x -= Input.GetAxis("Mouse Y") * rotateSpeed;
        if (camera_rot.x > 180f) camera_rot.x -= 360f;
        camera_rot.x = Mathf.Clamp(camera_rot.x, -90, 90);
        camera_3d.transform.eulerAngles = camera_rot;
    }
}
