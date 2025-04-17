using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool has_key = false;
    public bool is_2d = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnCollisionEnter(Collision collision)
    {
        string coll_tag = collision.gameObject.tag;
        switch (coll_tag)
        {
            case "Key":
                has_key = true;
                Destroy(collision.gameObject);
                break;

            case "Door":
                if (has_key)
                {
                    Destroy(collision.gameObject);
                }
                break;

            case "Exit":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;

            default:
                break;
        }
    }
}
