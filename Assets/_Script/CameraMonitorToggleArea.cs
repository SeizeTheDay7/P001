using UnityEngine;

public class CameraMonitorToggleArea : MonoBehaviour
{
    [SerializeField] MeshRenderer targetRenderer;

    void Start()
    {
        // targetRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        targetRenderer.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        targetRenderer.enabled = false;
    }
}
