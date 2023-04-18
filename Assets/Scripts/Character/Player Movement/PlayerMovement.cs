using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public static PlayerMovement Instance;
    public StateMachine PlayerCharacter;

    public bool shouldMove;

    public float WalkSpeed;
    public float RunSpeed;

    public float moveSpeed;
    public float rotateSpeed;

    [SerializeField]
    private Joystick joystick;

    private Vector3 _moveDirHolder = new Vector3();
    private Vector3 _moveVector = new Vector3();
    private Vector3 _moveRot = new Vector3();
    public bool isMoving { get; }
    private CharacterController moveController;
    public PlayerAnimator playerAnimator;
    public float gravity= 4f;
    public bool DifferentCam = false;
    public float currentSpeed { get {
            if(moveController != null)
            {
                return (joystick.Direction.sqrMagnitude > 0.05) ? moveController.velocity.magnitude : 0;
            }
            return 0;
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update


    public Text WalkSpeedText;
    public Text RunSpeedText;
    public Text RotationSpeedText;


    public void ChangeWalkSpeed(float speed)
    {
        WalkSpeed += speed;
        //WalkSpeed = Mathf.Clamp(WalkSpeed, 0.5f, 100);
        WalkSpeedText.text = WalkSpeed.ToString();
    }
    public void ChangeRunspeed(float speed)
    {
        RunSpeed += speed;
        //RunSpeed = Mathf.Clamp(WalkSpeed, 0.5f, 100);
        RunSpeedText.text = RunSpeed.ToString();
    }
    public void ChangeRotationSpeed(float speed)
    {
        rotateSpeed += speed;
        //rotateSpeed= Mathf.Clamp(rotateSpeed, 0.5f, 100);
        RotationSpeedText.text = rotateSpeed.ToString();
    }
    void Start()
    {

        WalkSpeedText.text = WalkSpeed.ToString();
        RunSpeedText.text = RunSpeed.ToString();
        RotationSpeedText.text = rotateSpeed.ToString();



        moveController = GetComponent<CharacterController>();
        _moveRot = Vector3.zero;
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<PlayerAnimator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void Move()
    {
        if(PlayerCharacter.GetState()!=CurrentState.Dead)
        {


            if (joystick.Direction.sqrMagnitude > 0.001f)
            {




                if (joystick.Direction.sqrMagnitude <0.15f)
                {
                   
                    moveSpeed = WalkSpeed;

                    playerAnimator.SetAnimatorState((int)LocoMotionState.Walk);
                }
                else
                {
                    moveSpeed = RunSpeed;
                    playerAnimator.SetAnimatorState((int)LocoMotionState.Run);

                }



                if (DifferentCam)
                {
                   
                    _moveDirHolder.x = -joystick.Vertical;//joystick.Horizontal;
                    _moveDirHolder.z = joystick.Horizontal; /*joystick.Vertical;*/




                    _moveDirHolder.Normalize();
                  
                    float angle = Mathf.Atan2(-joystick.Vertical, joystick.Horizontal) * 180.0f / 3.14159f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), rotateSpeed * Time.deltaTime);
                    //_moveDirHolder *= moveSpeed;
                    _moveVector = _moveDirHolder;
                    if (!moveController.isGrounded)
                    {
                        _moveVector.y -= gravity;
                    }

                  



                    moveController.Move(_moveVector * moveSpeed * Time.deltaTime * _moveDirHolder.magnitude);
                    //moveController.Move(_moveDirHolder * Time.deltaTime);
                   
                }
                else
                {
                    _moveDirHolder.x = joystick.Horizontal;
                    _moveDirHolder.z = joystick.Vertical;
                    
                    _moveDirHolder.Normalize();
                    float angle = Mathf.Atan2(joystick.Horizontal, joystick.Vertical) * 180.0f / 3.14159f;
                   
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, angle, 0)), rotateSpeed * Time.deltaTime);
                    //_moveDirHolder *= moveSpeed;
                    _moveVector = _moveDirHolder;
                    if (!moveController.isGrounded)
                    {
                        _moveVector.y -= gravity;
                    }
                    moveController.Move(_moveVector * moveSpeed * Time.deltaTime * _moveDirHolder.magnitude);
                    //moveController.Move(_moveDirHolder * Time.deltaTime);
                  // playerAnimator.SetSpeedValue(1);
                }
                //movement working
               
            }
            else
            {
                PlayerCharacter.SetLocomotionState(LocoMotionState.Idle);

                //playerAnimator.SetSpeedValue(0);
            }
        }
    }




    float ClampingValue(float value)
    {
        float nValue=0;
        if (value >0)
        {
            nValue = 1f;
        } else if (value <0)
        {
            nValue = -1;
        }
        return nValue;
    }


    public void SetAnimHoldObject(bool holding)
    {
        playerAnimator.OnHoldObject(holding);
    }
}
