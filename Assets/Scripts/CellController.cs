using UnityEngine;

public class CellController : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject bottomWall;
    public GameObject finish;

    public bool IsVisibleOnScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}