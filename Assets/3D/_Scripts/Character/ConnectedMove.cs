using UnityEngine;

public class ConnectedMove : MonoBehaviour
{
    [SerializeField] Camera camera_2d;
    [SerializeField] Camera camera_3d;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 5f;
    CharacterController cc;
    bool is_2d = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        PlayerRotate();
    }

    public void ReverseDimension()
    {
        is_2d = !is_2d;
        if (!is_2d)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void PlayerMove()
    {
        Vector3 direction;
        if (is_2d) direction = MoveDir_2d();
        else direction = MoveDir_3d();

        direction.y = 0f; // y축 이동 방지
        direction.Normalize();

        cc.Move(direction * moveSpeed * Time.deltaTime);
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

    private void PlayerRotate()
    {
        if (is_2d) PlayerRotate_2d();
        else PlayerRotate3d();
    }

    private void PlayerRotate_2d()
    {
        Ray ray = camera_2d.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; // Maintain the same height
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void PlayerRotate3d()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        Vector3 rotation = transform.eulerAngles;
        rotation.y += mouseX;
        rotation.x -= mouseY;
        if (rotation.x > 180f) rotation.x -= 360f;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);

        transform.eulerAngles = rotation;
    }
}
