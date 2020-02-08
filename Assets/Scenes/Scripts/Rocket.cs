using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip successSound;
    [SerializeField] AudioClip deathSound;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem successParticle;
    [SerializeField] ParticleSystem deathParticle;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;

    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

        if (!isTransitioning) {

            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild) {

            respondToDebugKeys();
        }
    }

    private void respondToDebugKeys() {

        if (Input.GetKeyDown(KeyCode.L)) {

            LoadNextLevel();
        } else if (Input.GetKeyDown(KeyCode.C)) {

            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void RespondToThrustInput() {

        if (Input.GetKey(KeyCode.Space)) {

            ApplyThrust();
        } else {

            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust() {

        audioSource.Stop();
        mainEngineParticle.Stop();
    }

    private void ApplyThrust() {

        rigidBody.AddRelativeForce(Vector3.up * mainThrust);

        if (!audioSource.isPlaying) {

            audioSource.PlayOneShot(mainEngineSound);
        }

        mainEngineParticle.Play();
    }

    private void RespondToRotateInput() {

        if (Input.GetKey(KeyCode.A)) {
            RotateManually(rcsThrust * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.D)) {

            RotateManually(-rcsThrust * Time.deltaTime);
        }
    }

    private void RotateManually(float rotationThisFrame) {

        rigidBody.freezeRotation = true; // take manual control of roatation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    private void OnCollisionEnter(Collision collision) {

        if (isTransitioning || collisionsDisabled) { return; }

        switch (collision.gameObject.tag) {

            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence() {

        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        successParticle.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence() {

        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticle.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel() {

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {

            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel() {

        SceneManager.LoadScene(0);
    }
}
