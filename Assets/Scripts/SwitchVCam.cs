using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class SwitchVCam : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    private CinemachineVirtualCamera virtualCamera;
    private InputAction aimAction;

    public Canvas thirdPersonCanvas;

    public Canvas aimCanvas;

    [SerializeField]
    private int priotityBoostAmount = 10;
    // Start is called before the first frame update
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Aim"];
        aimCanvas.enabled = false;
    }

    private void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();
    }

    private void OnDisable()
    {
        aimAction.performed -= _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
    }

    void StartAim()
    {
        virtualCamera.Priority += priotityBoostAmount;
        aimCanvas.enabled = true;
        thirdPersonCanvas.enabled = false;
    }

    void CancelAim()
    {
        virtualCamera.Priority -= priotityBoostAmount;
        aimCanvas.enabled = false;
        thirdPersonCanvas.enabled = true;
    }
}
