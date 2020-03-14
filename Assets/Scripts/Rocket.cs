using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] private float rcsThrust = 300f;
    [SerializeField] private float mainThrust = 1500f;    
    [SerializeField] private float levelLoadDelay = 2f;
    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip deathSound2;
    [SerializeField] private AudioClip levelLoad;

    [SerializeField] private ParticleSystem mainEngineParticles;
    [SerializeField] private ParticleSystem levelLoadParticles;
    [SerializeField] private ParticleSystem deathParticles;

    Rigidbody rigidbody;
    AudioSource audioSource;
    private Animator _animator;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    private bool collisionsEnabled = true;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (state != State.Alive) return;
        HandleThrust();
        HandleRotate();

        if(Debug.isDebugBuild)
        { RespondToDebugKeys();}
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || !collisionsEnabled) return;

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                CompleteLevel();
                break;
            default:
                Defeat();
                break;
        }
    }

    private void Defeat()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound2);
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        state = State.Dying;
        Invoke(nameof(LoadFirstLevel), levelLoadDelay);
    }

    private void CompleteLevel()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(levelLoad);
        levelLoadParticles.Play();
        state = State.Transcending;
        Invoke(nameof(LoadNextScene), levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        mainEngineParticles.Stop();
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var nextScene = currentSceneIndex + 1;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        print(currentSceneIndex + "currentSceneIndex");
        print(nextScene + "nextScene");
        print(totalScenes + "totalScenes");

        SceneManager.LoadScene(nextScene < totalScenes ? nextScene : 0);
    }

    private void HandleThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            audioSource.volume = 1f; 

            mainEngineParticles.Stop();
            _animator.speed = 0.2f;
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * (mainThrust * Time.deltaTime));
        
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0.4f; 
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
        
        _animator.speed = 1f;
        _animator.SetBool("Run", true);
    }

    private void HandleRotate()
    {
        rigidbody.freezeRotation = true; //take control of rotation

        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        rigidbody.freezeRotation = false; // resume physics
    }
}
