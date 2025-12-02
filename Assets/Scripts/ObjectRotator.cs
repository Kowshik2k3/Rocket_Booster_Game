using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(90f, 0f, 0f);
    // Default: rotate 90 degrees per second on X axis

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
