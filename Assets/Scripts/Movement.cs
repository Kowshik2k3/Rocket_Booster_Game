using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Required for Button and fuel slider

public class Movement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem vfxMain;
    [SerializeField] private ParticleSystem vfxRight;
    [SerializeField] private ParticleSystem vfxLeft;

    [Header("Component References")]
    private Rigidbody rb;
    private AudioSource audioSource;
    [SerializeField] private AudioClip sfxThrust;

    [Header("Power Value")]
    [SerializeField] private float thrustPower;
    [SerializeField] private float rotationPower;

    [Header("Fuel System")]
    [SerializeField] private Image fuelFill;   // drag your Fill image here in Inspector
    [SerializeField] private float fuel = 100f; // current fuel level
    [SerializeField] private float maxFuel = 100f; // maximum fuel capacity
    [SerializeField] private float fuelConsumptionRate = 50f; // Fuel consumed per second while thrusting
    

    private void Start()    
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (fuelFill != null)
            fuelFill.fillAmount = fuel / maxFuel; // Initialize fuel UI
    }
    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust(); // Custom method to handle thrust input
    }

    private void Update()
    {
        ProcessRotation();
    }
    private void ProcessRotation()
    {
        float rotationValue = rotation.ReadValue<float>();

        if (rotationValue == 1)
        {
            rb.freezeRotation = true;
            transform.Rotate(Vector3.forward * rotationPower * Time.deltaTime);
            rb.freezeRotation = false;

            if (!vfxRight.isPlaying) vfxRight.Play();
            if (vfxLeft.isPlaying) vfxLeft.Stop();
        }
        else if (rotationValue == -1)
        {
            rb.freezeRotation = true;
            transform.Rotate(-Vector3.forward * rotationPower * Time.deltaTime);
            rb.freezeRotation = false;

            if (!vfxLeft.isPlaying) vfxLeft.Play();
            if (vfxRight.isPlaying) vfxRight.Stop();
        }
        else
        {
            if (vfxLeft.isPlaying) vfxLeft.Stop();
            if (vfxRight.isPlaying) vfxRight.Stop();
        }
    }
    private void ProcessThrust()
    {
        if (fuel > 0 && thrust.IsPressed())
        {
            fuel -= fuelConsumptionRate * Time.deltaTime;
            fuel = Mathf.Clamp(fuel, 0, maxFuel);

            if (fuelFill != null)
                fuelFill.fillAmount = fuel / maxFuel;

            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(sfxThrust);

            if (!vfxMain.isPlaying) vfxMain.Play();

            rb.AddRelativeForce(Vector3.up * thrustPower * Time.deltaTime);
        }
        else
        {
            audioSource.Stop();
            vfxMain.Stop();
        }
    }

    public void AddFuel(float amount)
    {
        fuel += amount;
        fuel = Mathf.Clamp(fuel, 0, maxFuel);

        if (fuelFill != null)
            fuelFill.fillAmount = fuel / maxFuel;
    }


}
