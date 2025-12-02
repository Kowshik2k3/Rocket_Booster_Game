using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // REQUIRED for IEnumerator


public class CollisionHandler : MonoBehaviour
{

    [SerializeField] private AudioClip sfxCrash; // Explosion sound effect
    [SerializeField] private AudioClip sfxFinish; // Finish sound effect
    [SerializeField] private ParticleSystem vfxCrash; // Explosion particle effect
    [SerializeField] private ParticleSystem vfxFinish; // Finish particle effect
    [SerializeField] private AudioClip sfxFuelCollect; // Fuel pickup sound

    [SerializeField] private GameObject winPanel;   // assign your Win Panel here
    [SerializeField] private GameObject[] stars;    // assign Star1, Star2, Star3
    [SerializeField] private int crystalsCollected; // tracks collected crystals


    private AudioSource audioSource; // Reference to the AudioSource component

    [SerializeField] float delayTime = 1.2f;   // total time before reload (keeps existing behavior)
    [SerializeField] float hideDelay = 1f; // how long to wait before hiding the rocket visuals

    bool isControlable = false;
    private bool isTransitioning = false; // prevents double-triggering


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the Player Rocket!");
        }

    }


    private void Update()
    {
        CheckDebugKeys();
        CheckForQuit();
    }

    private static void CheckForQuit()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit(); // Quit the application when 'Escape' key is pressed

        }
    }

    private void CheckDebugKeys()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Debug.Log("L key pressed. loading next scene.");
            LoadNextScene(); // Load the next scene when 'L' key is pressed
        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isControlable = !isControlable; // Toggle controlable state when 'C' key is pressed
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isControlable)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Friendly collision detected." + collision.gameObject.name);
                // Handle friendly collision logic
                break;

            case "Finish":
                Debug.Log("Finish line reached.");
                // Level complete logic
                UnlockNewLevel(); // Unlock the next level
                StartFinishSequence(); // Start the finish sequence

                break;
            default:
                Debug.Log(" Explode the rocket");
                StartCrashSequence();
                // Explode the rocket
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isControlable) return;

        if (other.CompareTag("Fuel"))
        {
            Debug.Log("Fuel collected via trigger!");

            // Add fuel to the player
            Movement playerMove = GetComponent<Movement>();
            if (playerMove != null)
            {
                playerMove.AddFuel(15f); // refill amount
            }

            // Play fuel collect SFX at rocket position
            if (sfxFuelCollect != null)
            {
                AudioSource.PlayClipAtPoint(sfxFuelCollect, transform.position, 50f);
                // 1.2f = slightly boosted volume (you can adjust 0.5f–2f)
            }

            // Destroy fuel pickup
            Destroy(other.gameObject);
        }
    } 



private void StartCrashSequence()
    {
        // guard to prevent multiple crash sequences
        if (isTransitioning) return;
        isTransitioning = true;

        isControlable = true;               // prevent further inputs
        Movement move = GetComponent<Movement>();
        if (move != null) move.enabled = false; // stop movement script

        // Play crash sound independent from player object so it'll keep playing.
        if (sfxCrash != null)
            AudioSource.PlayClipAtPoint(sfxCrash, transform.position);

        // Spawn an independent crash VFX at the player's position so it persists.
        if (vfxCrash != null)
        {
            ParticleSystem spawned = Instantiate(vfxCrash, transform.position, Quaternion.identity);
            spawned.Play();
            // destroy spawned VFX after a safe duration (adjust if your VFX is longer)
            Destroy(spawned.gameObject, 4f);
        }

        // start the timed sequence (hide and reload happen inside coroutine)
        StartCoroutine(HandleCrashSequenceCoroutine());
    }

    private IEnumerator HandleCrashSequenceCoroutine()
    {
        // Wait a short time so the VFX and sound can start and be visible/audible
        yield return new WaitForSeconds(hideDelay);

        // Stop the thrust looping sound if it was playing
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Hide player visuals (renderers) so the rocket disappears, but this script is still running.
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        // Disable all colliders so no further collisions occur while waiting to reload
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }

        // Wait the remaining time until reload. Protect against negative value.
        float remaining = Mathf.Max(0f, delayTime - hideDelay);
        yield return new WaitForSeconds(remaining);

        // Finally reload the level (same as your old ReloadScene)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // ---------- end crash sequence ----------
    private void StartFinishSequence()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        vfxFinish?.Play();
        audioSource?.PlayOneShot(sfxFinish);

        GetComponent<Movement>().enabled = false;

        Invoke(nameof(ShowWinPanel), 1f); // delay for effects
    }

    private void ShowWinPanel()
    {
        winPanel.SetActive(true);

        // Show stars based on collected crystals
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(i < crystalsCollected);
        }

        // Save stars collected for this level
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int previousStars = PlayerPrefs.GetInt("Level" + currentLevel + "_Stars", 0);

        // Save only if the new score is higher (don’t overwrite with fewer stars)
        if (crystalsCollected > previousStars)
        {
            PlayerPrefs.SetInt("Level" + currentLevel + "_Stars", crystalsCollected);
            PlayerPrefs.Save();
        }
    }


    // called from crystal pickup
    public void AddCrystal()
    {
        crystalsCollected++;
    }


    private void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        if (nextIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextIndex = 1; // Loop back to the first level (assuming index 0 is the main menu)  
        }
     
        SceneManager.LoadScene(nextIndex);

    }

    void UnlockNewLevel() // Unlock the next level if the current level is the highest reached level
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex")) // Check if the current level is the highest reached level
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1); // Update the highest reached index to the next level
            PlayerPrefs.SetInt("UnlockedLevel",PlayerPrefs.GetInt("UnlockedLevel", 1) + 1 ); // Increment the count of unlocked levels
            PlayerPrefs.Save(); // Save the changes to PlayerPrefs
        }
    }
    private void ReloadScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

}
