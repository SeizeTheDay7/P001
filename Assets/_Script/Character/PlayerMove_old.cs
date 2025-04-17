using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove_old : MonoBehaviour
{
    [Header("Cameras / 2D Collider")]
    [SerializeField] Camera camera_2d;
    [SerializeField] Camera camera_3d;
    [SerializeField] Collider coll_2d;

    [Header("Speeds")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 5f;

    [Header("Setup")]
    [SerializeField] bool start_with_3d = false;
    [SerializeField] Transform ceiling;
    float ceiling_y;
    Rigidbody rb;
    bool is_2d = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (start_with_3d) ReverseDimension();
    }

    public void ReverseDimension()
    {
        is_2d = !is_2d;

        if (!is_2d)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            coll_2d.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            coll_2d.enabled = true;
        }

        camera_3d.transform.localRotation = Quaternion.identity;
        camera_2d.enabled = !camera_2d.enabled;
        camera_3d.enabled = !camera_3d.enabled;
    }


    void FixedUpdate()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Move();
    }

    // 부모가 된 player_2d가 이동한다
    private void Move_2d()
    {
        Vector3 dir = MoveDir_2d();
        dir.Normalize();

        PlayerAlwaysTop();
    }

    // player의 xz좌표와 ceiling의 y좌표에서 아래로 raycast를 쏴서 맞은 위치에 player를 위치시킨다
    private void PlayerAlwaysTop()
    {
        Vector3 ray_start = transform.position;
        ray_start.y = ceiling_y;

        RaycastHit hit;
        if (Physics.Raycast(ray_start, Vector3.down, out hit))
        {

        }
    }

    // player_3d가 이동한다
    private void Move_3d()
    {
        Vector3 dir = MoveDir_3d();
        // dir.y = 0;
        dir.Normalize();

        Vector3 targetPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }



    private void Move()
    {
        Vector3 dir = is_2d ? MoveDir_2d() : MoveDir_3d();
        dir.y = is_2d ? 0f : -1f;
        dir.Normalize();

        Vector3 targetPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    private Vector3 MoveDir_2d()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        return direction;
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
        Rotate();
    }

    private void Rotate()
    {
        if (is_2d) Rotate_2d();
        else Rotate_3d();
    }

    private void Rotate_2d()
    {
        Ray ray = camera_2d.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y;
            Quaternion q = Quaternion.LookRotation((target - transform.position).normalized);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, q, Time.deltaTime * 10f));
        }
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
