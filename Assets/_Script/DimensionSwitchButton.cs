using UnityEngine;

public class DimensionSwitchButton : MonoBehaviour
{
    bool touched = false;

    void OnTriggerEnter(Collider other)
    {
        if (touched) return;
        touched = true;

        other.transform.GetComponent<PlayerDimension>().ChangeDimension();
        // other.transform.GetComponent<PlayerMove_old>().ReverseDimension();
        Destroy(gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        touched = false;
    }
}
