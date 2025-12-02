using Unity.VisualScripting;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float speed;

    Vector3 startingPosition;
    Vector3 endPosition;
    float movementFactor;

    private void Start()
    {
        startingPosition = transform.position;
        endPosition = startingPosition + movementVector;
    }

    private void Update()
    {
        movementFactor = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(startingPosition, endPosition, movementFactor);
    }
}
