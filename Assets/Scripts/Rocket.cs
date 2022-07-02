using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour
{
    //todo fix lighting bug
    [SerializeField] float rcsThrust = 50f;
    [SerializeField] float mainThrust = 30f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deadGame;
    [SerializeField] AudioClip succesGame;

    Rigidbody rigidBody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if( state == State.Alive )
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive){ return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":                
                break;
            case "Finish":
                SuccessGame();
                break;
            default:
                FailedGame();
                break;
        }
    }

    private void FailedGame()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deadGame);
        Invoke("LoadFirstLevel", 1f);
    }

    private void SuccessGame()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(succesGame);
        Invoke("LoadNextLevel", 1f);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }


    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToThrustInput() 
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
    }

}
