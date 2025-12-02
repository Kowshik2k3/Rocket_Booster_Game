using UnityEngine;

public class CrystalCollectible : MonoBehaviour
{
    [SerializeField] private AudioClip starCollectSFX;
    [SerializeField] private ParticleSystem collectVFX; // prefab to spawn

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Tell CollisionHandler we collected a star
        other.GetComponent<CollisionHandler>()?.AddCrystal();
        Debug.Log("Star collected!");

        // --- VFX ---
        if (collectVFX != null)
        {
            ParticleSystem vfx = Instantiate(collectVFX, transform.position, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, vfx.main.duration); // destroy VFX once finished
        }

        // --- Hide only visuals of the star immediately ---
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        // --- Play SFX ---
        float clipLength = 0f;
        if (starCollectSFX != null)
        {
            audioSource.PlayOneShot(starCollectSFX);
            clipLength = starCollectSFX.length;
        }

        // --- Finally, destroy the star GameObject after sound finishes ---
        Destroy(gameObject, clipLength > 0 ? clipLength : 0.1f);
    }
}
