using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

        if (state == State.Alive) {

            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void RespondToThrustInput() {

        if (Input.GetKey(KeyCode.Space)) {

            ApplyThrust();
        } else {

            audioSource.Stop();
        }
    }

    private void ApplyThrust() {

        rigidBody.AddRelativeForce(Vector3.up * mainThrust);

        if (!audioSource.isPlaying) {

            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput() {

        rigidBody.freezeRotation = true; // take manual control of roatation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
 
            transform.Rotate(Vector3.forward * rotationThisFrame);
        } else if (Input.GetKey(KeyCode.D)) {

            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    private void OnCollisionEnter(Collision collision) {

        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag) {

            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextLevel", 1f);
                break;
            default:
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }

    private void LoadNextLevel() {

        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel() {

        SceneManager.LoadScene(0);
    }
}
