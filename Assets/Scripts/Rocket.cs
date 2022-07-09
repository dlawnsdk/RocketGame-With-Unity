using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour
{

    //todo fix lighting bug
    [SerializeField] float rcsThrust = 50f;
    [SerializeField] float mainThrust = 30f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deadGame;
    [SerializeField] AudioClip succesGame;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem deadGameParticle;
    [SerializeField] ParticleSystem succesGameParticle;

    Rigidbody rigidBody;
    AudioSource audioSource;
    
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionToggle = false;

    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody   = GetComponent<Rigidbody>();
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
        RespondToDebugKey();
    }

    private void RespondToDebugKey()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionToggle = !collisionToggle;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || collisionToggle){ return; }
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
        deadGameParticle.Play();
        audioSource.PlayOneShot(deadGame);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void SuccessGame()
    {

        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(succesGame);
        succesGameParticle.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int curScenIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScenIndex = curScenIndex + 1;

        if(nextScenIndex == SceneManager.sceneCountInBuildSettings)
        {
            print("Test");
            LoadFirstLevel();
        }
        print(nextScenIndex);

        SceneManager.LoadScene(nextScenIndex);

    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
            {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }


    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticle.Play();
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
            mainEngineParticle.Stop();
        }
    }

}
