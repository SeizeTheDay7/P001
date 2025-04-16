using UnityEngine;

public class DimensionSwitchButton : MonoBehaviour
{
    [SerializeField] Camera camera_2d;
    [SerializeField] Camera camera_3d;
    bool touched = false;

    void OnTriggerEnter(Collider other)
    {
        if (touched) return;
        touched = true;
        camera_2d.enabled = !camera_2d.enabled;
        camera_3d.enabled = !camera_3d.enabled;

        other.transform.GetComponent<ConnectedMove>().ReverseDimension();
    }

    void OnTriggerExit(Collider other)
    {
        touched = false;
    }
}
