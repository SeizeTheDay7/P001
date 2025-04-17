using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform ceiling;
    [SerializeField] GameObject player_3d;
    float ceiling_y;
    float coll_3d_h;
    int layerMask;

    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ceiling_y = ceiling.position.y;
        layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        coll_3d_h = Get3dCollHeight();
    }

    private float Get3dCollHeight()
    {
        Vector3 p3dPos = player_3d.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(p3dPos, Vector3.down, out hit, 10f, layerMask))
        {
            return p3dPos.y - hit.point.y;
        }
        print("3d player의 높이를 감지하지 못함.");
        return 2.5f;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Move();
        Player3dOnTop();
    }

    private void Move()
    {
        Vector3 dir = MoveDir_2d();
        dir.y = 0f;
        dir.Normalize();

        Vector3 targetPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        targetPos.y = ceiling_y;
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

    private void Player3dOnTop()
    {
        Vector3 ray_start = transform.position;
        ray_start.y = ceiling_y;

        RaycastHit hit;
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 10f, layerMask))
        {
            Vector3 playerPos = hit.point;
            playerPos.y += coll_3d_h;
            player_3d.transform.position = playerPos;
        }
    }

    // void Update()
    // {
    //     Rotate_2d();
    // }

    // private void Rotate_2d()
    // {
    //     Ray ray = camera_2d.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         Vector3 target = hit.point;
    //         target.y = transform.position.y;
    //         Quaternion q = Quaternion.LookRotation((target - transform.position).normalized);
    //         rb.MoveRotation(Quaternion.Slerp(rb.rotation, q, Time.deltaTime * 10f));
    //     }
    // }
}
