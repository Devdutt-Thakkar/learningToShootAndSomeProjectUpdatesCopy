using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private PlayerInput playerInput;
    private Transform cameraTransform;

    private InputAction moveAction;
    //private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    [SerializeField]
    private SwitchVCam switchV;

    [SerializeField]
    private GameObject weapon;
    

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 6f;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform barrelTransform;
    [SerializeField]
    Transform bulletParent;
    [SerializeField]
    float bulletHitMissDist = 25f;
    [SerializeField]
    private float animationSmoothTime = 0.1f;
    [SerializeField]
    private float animationPlayTransission = 0.1f;
    [SerializeField]
    private Transform aimTarget;
    [SerializeField]
    private float backwardMove = -2.5f;

    [SerializeField]
    private float aimDistance;

    private MultiAimConstraint headaimconstraint;
    private MultiAimConstraint chestaimconstraint;

    private TwoBoneIKConstraint righthand;
    private TwoBoneIKConstraint lefthand;

    private GameObject pistolarmrig;

    [SerializeField]
    private Vector3 gunpositioning;

    [SerializeField]
    private GameObject reflefthand;

    

    [SerializeField]
    private Quaternion gunangle;

    private Animator animator;

    [SerializeField]
    private Vector3 gunpositioningonAim;

    [SerializeField]
    private Quaternion gunangleonaim;

    int moveXAnimationParamId;
    int moveZAnimationParamId;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;
    [SerializeField]
    private GameObject objectToFind;
    private GameObject chestaim;
    private GameObject headaim;
    int jumpAnimation;

    [SerializeField]
    Vector3 shiftlefthand;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        //lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        cameraTransform = Camera.main.transform;
        objectToFind = transform.GetChild(3).gameObject;
        chestaim = objectToFind.transform.GetChild(1).gameObject;
        headaim = objectToFind.transform.GetChild(2).gameObject;

        pistolarmrig = transform.GetChild(0).gameObject;

        lefthand = pistolarmrig.transform.GetChild(1).gameObject.GetComponent<TwoBoneIKConstraint>();
        righthand = pistolarmrig.transform.GetChild(0).gameObject.GetComponent<TwoBoneIKConstraint>();

        headaimconstraint = headaim.GetComponent<MultiAimConstraint>();
        chestaimconstraint = chestaim.GetComponent<MultiAimConstraint>();
        Cursor.lockState = CursorLockMode.Locked;
        //Animations
        animator = GetComponent<Animator>();
        //animator.SetFloat("MoveX", 1f);
        moveZAnimationParamId = Animator.StringToHash("MoveZ");

        moveXAnimationParamId = Animator.StringToHash("MoveX");

        jumpAnimation = Animator.StringToHash("Pistol Jump");

    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }

    void ShootGun()
    {
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            bulletController.target = hit.point;
            bulletController.hit = true;
        }
        else
        {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDist;
            bulletController.hit = false;
        }
    }

    void Update()
    {

        
        if (switchV.aimCanvas.enabled == true)
        {

            weapon.transform.localPosition = gunpositioningonAim;//new Vector3(-0.248f, -1.101f, -0.19f) ;  //This is a static value since there is no need to change it
            weapon.transform.localRotation = gunangleonaim;//new Quaternion(5.412f, 0f, 0f,0);  //This is a static value since there is no need to change it
            righthand.weight = 1;
            chestaimconstraint.weight = 0.5f;
            headaimconstraint.weight = 0.621f;
            aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            Vector2 input = moveAction.ReadValue<Vector2>();
            currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime); ;
            Vector3 move = new Vector3(input.x, 0, input.y);
            move = move.x * cameraTransform.right.normalized + backwardMove * cameraTransform.forward.normalized;
            move.y = 0;
            controller.Move(move * Time.deltaTime * playerSpeed);
            animator.SetFloat(moveXAnimationParamId, currentAnimationBlendVector.x);
            animator.SetFloat(moveZAnimationParamId, -1);
            // Changes the height position of the player..
            if (jumpAction.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                animator.CrossFade(jumpAnimation, animationPlayTransission);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Rotate towards camera direction
            float targetAngle = cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else if(switchV.thirdPersonCanvas.enabled == true)
        {
            
            weapon.transform.localPosition = gunpositioning;
            weapon.transform.localRotation = gunangle;
            righthand.weight = 0;
            chestaimconstraint.weight = 0;
            headaimconstraint.weight = 0f;
            aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            Vector2 input = moveAction.ReadValue<Vector2>();
            currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime); ;
            Vector3 move = new Vector3(input.x, 0, input.y);
            move = move.x * cameraTransform.right.normalized + backwardMove * cameraTransform.forward.normalized;
            move.y = 0;
            controller.Move(move * Time.deltaTime * playerSpeed);
            animator.SetFloat(moveXAnimationParamId, currentAnimationBlendVector.x/2);
            animator.SetFloat(moveZAnimationParamId, -0.5f);
            // Changes the height position of the player..
            if (jumpAction.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                animator.CrossFade(jumpAnimation, animationPlayTransission);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Rotate towards camera direction
            float targetAngle = cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, 180+cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        
    }
}