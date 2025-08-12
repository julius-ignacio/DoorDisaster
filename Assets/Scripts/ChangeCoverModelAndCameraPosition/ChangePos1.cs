using UnityEngine;

public class ChangePos1 : MonoBehaviour
{
    public GameObject Player, Cube;
    public Camera CoverCamera;


    void Start()
    {
        Player.SetActive(true); // Ensure player is active at start 
        CoverCamera.gameObject.SetActive(false); // Ensure cover camera is inactive at start


    }


    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.name == "TRIGGER_START")
        {
            Player.transform.position = Cube.transform.position;
            Debug.Log("Player position changed to Cube position");
        }
// if (collision.gameObject.name == "TRIGGER_START")
// {
//     Player.transform.position = new Vector3(
//         -0.7653198f,
//         0.9984987f,
//         -1.193405f
//     );
//     Player.transform.rotation = Quaternion.Euler(
//         8.33f,
//         90.24f,
//         0f
//     );
// }
// else if (collision.gameObject.name == "TRIGGER_LIBRARY")
// {
//     Player.transform.position = new Vector3(
//         -16.917f,
//         0.506f,
//         -3.078f
//     );
//     Player.transform.rotation = Quaternion.Euler(
//         -6.4f,
//         279.6f,
//         0f
//     );
// }
// else if (collision.gameObject.name == "TRIGGER_LIBRARY2")
// {
//     Player.transform.position = new Vector3(
//         -0.7653198f,
//         0.5084987f,
//         -1.193405f
//     );
//     Player.transform.rotation = Quaternion.Euler(
//         8.33f,
//         90.24f,
//         0f
//     );
// }

    }






}
