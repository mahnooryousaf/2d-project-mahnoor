using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using TMPro;
public class PlayerController : MonoBehaviour
{
    //Fields
    public static PlayerController instance=null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public CharacterController characterController;
    public float playerSpeed = 2.0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Transform playerCamera;
    public Transform surfaceCheck;
   public  bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;
    Vector3 velocity;
    public float jumpHeight = 1f;
    public float gravity = -9.8f;
    public float sprintSpeed = 4f;

    public Animator animator;


    public bool isAiming;
    public GameObject ThirdPersonCamera;
    public GameObject FirstPersonCamera;
    public GameObject BulletPosition;
    public GameObject Gun;
    public TextMeshProUGUI BulletText;
    public int BulletCounter = 30;
    public float Cooldown = 0f;
    private bool isReloading = false;

    public ParticleSystem BulletMuzzleParticles;
    public GameObject BulletImpactParticles;
    public GameObject CrossHair;

    private bool isPlayerShooting = false;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
  
    }

    
    void Update()
    {
        if(Manager.instance.CurrentGameState==Manager.GameState.Running)
        {
            if (Cooldown > 0)
            {
                Cooldown -= Time.deltaTime;
            }
            onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
            if (onSurface && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.M))
            {
                isPlayerShooting = true;
                if (Cooldown <= 0)
                {
                    Fire();
                }

            }
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.M))
            {
                isPlayerShooting = false;
                animator.SetBool("IdleAim", false);
            }
            //if(Input.GetKey(KeyCode.R))
            //{
            //    if(isReloading==false)
            //    {
            //        isReloading = true;

                    
            //        SoundManager.instance.PlayFiringSound(false);
            //        SoundManager.instance.Reload.Play();
            //        animator.SetBool("AimWalk", false);
            //        animator.SetBool("Reloading", true);
            //    }
              
            //}

            if (isAiming == false && isReloading == false)
            {
               
                playerMove();

                Jump();

                Sprint();
            }

            else
            {
                if (isReloading == false)
                {
                    float v = Input.GetAxis("Vertical") * 2 * Time.deltaTime;
                    float h = Input.GetAxis("Horizontal") * 2 * Time.deltaTime;

                    if (v != 0 || h != 0)
                    {
                        animator.SetBool("AimWalk", true);
                    }
                    else
                        animator.SetBool("AimWalk", false);
                    float strafe = 1;
                    strafe *= Time.deltaTime;
                    //  translation *= Time.deltaTime;
                    //  rotation *= Time.deltaTime;
                    transform.Translate(h, 0, v);
                    //    transform.Translate(0, 0, translation);
                    //transform.Rotate(0, rotation, 0);



                }
                float RotationSpeed = 60f;
                transform.Rotate(0, (Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime), 0, Space.World);
                
            }

            if (GameManager.instance.CanPickUpItem)
            {
                GetPickUpInput();
            }

            if (Input.GetMouseButtonDown(1))
            {


              CrossHair.SetActive(true);

                isAiming = true;
                FirstPersonCamera.SetActive(true);
                ThirdPersonCamera.SetActive(false);
                animator.SetBool("IdleAim", true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
              CrossHair.SetActive(false);
                SoundManager.instance.PlayFiringSound(false);
                isAiming = false;
                FirstPersonCamera.SetActive(false);
                ThirdPersonCamera.SetActive(true);
                animator.SetBool("IdleAim", false);
            }
            if (Input.GetMouseButtonUp(0))
            {
                SoundManager.instance.PlayFiringSound(false);
            }
        }
        
    }
    public void ForceRemoveReload()
    {
        isReloading = false;
    }
    public void Fire()
    {
        if(BulletCounter!=0)
        {
            if(isAiming==false)
            animator.SetBool("IdleAim", true);
            SoundManager.instance.PlayFiringSound(true);

            Gun.transform.DOShakePosition(10f, 10, 30, 90);
            BulletMuzzleParticles.Play();

            Vector3 fwd = BulletPosition.transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(BulletPosition.transform.position, fwd * 50, Color.green);
            RaycastHit objectHit;

            if (Physics.Raycast(BulletPosition.transform.position, fwd, out objectHit, 100))
            {
              
                if (objectHit.transform.gameObject.tag== "Enemy")
                {
                    objectHit.transform.GetComponent<StateMachine>().DecrementHealth(this.gameObject);
                }
                else
                {
                    GameObject t = Instantiate(BulletImpactParticles, objectHit.point, BulletImpactParticles.transform.rotation);
                }
            }
            Cooldown = 0.15f;
            BulletCounter--;
            BulletText.text = BulletCounter.ToString();

            //if (isAiming == false)
            //    animator.SetBool("IdleAim", false);
        }
        else if(isReloading==false)
        {
            isReloading = true;
            SoundManager.instance.PlayFiringSound(false);
            SoundManager.instance.Reload.Play();
            animator.SetBool("AimWalk", false);
            animator.SetBool("Reloading", true);

        }
      
    }
    public void PlayerDied()
    {
        FirstPersonCamera.SetActive(false);
        ThirdPersonCamera.SetActive(true);
        animator.SetBool("Die", true);
    }
    public void ReloadDone()
    {
        animator.SetBool("Reloading", false);
        isReloading = false;
        BulletCounter = 30;
        BulletText.text = BulletCounter.ToString();
    }
  
    public void GetPickUpInput()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
       
      
            if(GameManager.instance.ObjectInView.GetComponent<Item>()!=null)
            {
                animator.SetTrigger("ItemPickUp");
                GameManager.instance.ItemPicked();

            }

            if (GameManager.instance.ObjectInView.GetComponent<Door>() != null)
                GameManager.instance.OpenDoor();

        }

    }

    void playerMove()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float VerticalAxis = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(horizontalAxis, 0f, VerticalAxis).normalized;

        if(moveDirection.magnitude >= 0.1f)
        {
            if(isAiming==false && isPlayerShooting)
            {
                animator.SetBool("IdleAim",true);
                animator.SetBool("AimWalk", true);
                animator.SetBool("Walk", false);
            }
            else
            {
                if (SoundManager.instance.FootStepSound.isPlaying==false)
                SoundManager.instance.FootStepSound.Play();
                animator.SetBool("AimWalk", false);
                animator.SetBool("IdleAim", false);
                animator.SetBool("Walk", true);
            }
              
     

            animator.SetBool("Running", false);
            animator.SetBool("Idle", false);
            animator.SetTrigger("Jump");
            animator.SetBool("WalkingAim", false);
          

            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y; 
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 Direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(Direction.normalized * playerSpeed * Time.deltaTime);
        }

        else
        {
            animator.SetBool("Idle", true);
            animator.SetTrigger("Jump");
            animator.SetBool("Walk", false);
            animator.SetBool("Running", false);
            animator.SetBool("WalkingAim", false);
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && onSurface)
        {
            animator.SetBool("Walk", false);
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        else
        {
            animator.ResetTrigger("Jump");
        }
    }

    void Sprint()
    {
        if (Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)

        {
            animator.SetBool("Running", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("IdleAim", false);

            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            float VerticalAxis = Input.GetAxisRaw("Vertical");
            Vector3 moveDirection = new Vector3(horizontalAxis, 0f, VerticalAxis).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 Direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                characterController.Move(Direction.normalized * sprintSpeed * Time.deltaTime);
            }

            else
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Walk", false);
            }
        }
    }
}
