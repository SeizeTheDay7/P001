using UnityEngine;

public class PlayerDimension : MonoBehaviour
{
    [SerializeField] PlayerMove2D pm2D;
    [SerializeField] PlayerMove3D pm3D;

    [Header("Cameras / Rigidbodies")]
    [SerializeField] Camera camera_2d;
    [SerializeField] Camera camera_3d;
    [SerializeField] Collider coll_2d;
    [SerializeField] Rigidbody rb_2d;
    [SerializeField] Rigidbody rb_3d;
    bool is_2d = true;
    [SerializeField] bool start_with_3d = false;

    void Start()
    {
        if (start_with_3d) ChangeInto3D();
    }

    public void ChangeDimension()
    {
        if (is_2d) ChangeInto3D();
        else ChangeInto2D();
    }

    private void ChangeInto3D()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pm3D.transform.parent = null;
        pm2D.transform.parent = pm3D.transform;

        pm3D.enabled = true;
        pm2D.enabled = false;

        rb_2d.isKinematic = true;
        rb_3d.isKinematic = false;

        camera_2d.enabled = false;
        camera_3d.enabled = true;

        is_2d = false;
    }

    private void ChangeInto2D()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pm2D.transform.parent = null;
        pm3D.transform.parent = pm2D.transform;

        rb_2d.isKinematic = false;
        rb_3d.isKinematic = true;

        pm2D.enabled = true;
        pm3D.enabled = false;

        camera_2d.enabled = true;
        camera_3d.enabled = false;

        is_2d = true;
    }
}
