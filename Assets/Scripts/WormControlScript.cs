using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormControlScript : MonoBehaviour
{
    private Coroutine wormMoveCourotine;
    public WormScript wormScript;
    public EquipmentScript equipment;
    UsableItemScript currentItem;
    CameraControlScript cameraControlScript;

    public float moveSpeed = 5f;
    public float verticalJumpForce = 5f;
    public float horizontalJumpForce = 2f;
    public bool canUseWeapon = true;

    public enum Direction
    {
        Left,
        Right
    }

    public enum WormState
    {
        Idle,
        Moving,
        InTheAir
    }

    public WormState currentWormState = WormState.Idle;
    public Direction currentDirection = Direction.Right;
    void Start()
    {
        cameraControlScript = FindObjectOfType<CameraControlScript>();
        EventManager.WeaponUsedEvent += HandleWeaponUsed;
        currentItem = equipment.getCurrentItem();
    }

    void Update()
    {
        CheckIfWormInTheAirAndChangeState();

        HandleJumpInput();
        HandleItemInput();
        HandleMoveInput();

        ChangeModelDependingOnWormState(currentWormState);

        if(currentWormState != WormState.Idle)
        {
            cameraControlScript.StopFollowing();
        }

    }

    private void HandleWeaponUsed()
    {
        canUseWeapon = false;
    }
    private void HandleMoveInput()
    {
        if (currentWormState != WormState.InTheAir)
        {
            if (Input.GetKey("a"))
            {
                currentWormState = WormState.Moving;
                RotateWormAndChangeDirection(Direction.Left);
                WormStepUp(currentDirection);
                wormScript.wormRb.AddForce(-moveSpeed * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
            }

            else if (Input.GetKey("d"))
            {
                currentWormState = WormState.Moving;
                RotateWormAndChangeDirection(Direction.Right);
                WormStepUp(currentDirection);
                wormScript.wormRb.AddForce(moveSpeed * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
            }
            else //tzn nie jest w powietrzu i sie nie rusza. Fajnie byloby to jakos zmienic
            {
                currentWormState = WormState.Idle;
                if (wormMoveCourotine != null)
                {
                    StopCoroutine(wormMoveCourotine);
                    wormMoveCourotine = null;
                    ReplaceActiveWormModel(wormScript.wormIdleModel);
                }
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Jump();
        }
    }

    private void HandleItemInput()
    {
        if (canUseWeapon)
        {
            currentItem = equipment.getCurrentItem();

            if (Input.GetKeyDown(KeyCode.E))
            {
                equipment.SwitchItem();
            }

            equipment.currentItem.Use();
            equipment.currentItem.Rotate();
        }
    }

    private void Jump()
    {
        if (currentWormState != WormState.InTheAir)
        {
            currentWormState = WormState.InTheAir;
            if (currentDirection == Direction.Left)
            {
                wormScript.wormRb.AddForce(-horizontalJumpForce, verticalJumpForce, 0f, ForceMode.Impulse);
            }
            else
            {
                wormScript.wormRb.AddForce(horizontalJumpForce, verticalJumpForce, 0f, ForceMode.Impulse);
            }
        }
    }

    private void RotateWormAndChangeDirection(Direction dir)
    {
        if (dir != currentDirection)
        {
            wormScript.wormPivot.transform.Rotate(0f, 180f, 0f);
            currentDirection = dir;
        }
    }

    public void ReplaceActiveWormModel(GameObject newModel)
    {
        foreach (Transform modelTransform in wormScript.wormModelContainer.transform)
        {
            GameObject model = modelTransform.gameObject;
            if (model != newModel)
            {
                model.SetActive(false);
            }
            else
            {
                model.SetActive(true);
            }
        }
    }

    private void CheckIfWormInTheAirAndChangeState()
    {
        if (wormScript.collisionCounter > 0)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                currentWormState = WormState.Moving;
            }
            else
            {
                currentWormState = WormState.Idle;
            }
        }
        else
        {
            currentWormState = WormState.InTheAir;
        }
    }

    public void SetModelToIdle()
    {
        ReplaceActiveWormModel(wormScript.wormIdleModel);
    }

    private void ChangeModelDependingOnWormState(WormState currentWormState)
    {
        if (currentWormState == WormState.Moving)
        {
            if (wormMoveCourotine == null)
            {
                wormMoveCourotine = StartCoroutine(AnimateMoving());
            }
        }

        else if (currentWormState == WormState.InTheAir)
        {
            StopMovingCoroutine();
            ReplaceActiveWormModel(wormScript.wormJumpingModel);
        }

        else if (currentWormState == WormState.Idle)
        {
            StopMovingCoroutine();
            ReplaceActiveWormModel(wormScript.wormIdleModel);
        }
    }


    private IEnumerator AnimateMoving()
    {
        while (currentWormState == WormState.Moving)
        {
            ReplaceActiveWormModel(wormScript.wormMovingModel);
            yield return new WaitForSeconds(0.5f);
            ReplaceActiveWormModel(wormScript.wormIdleModel);
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void StopMovingCoroutine()
    {
        if (wormMoveCourotine != null)
        {
            StopCoroutine(wormMoveCourotine);
            wormMoveCourotine = null;
        }
    }


    private void WormStepUp(Direction wormDirection)
    {
        float rayLength = 0.6f;
        float stepHeight = 0.65f;
        float offset = 0.2f;

        RaycastHit hit1;
        Vector3 direction = (wormDirection == Direction.Left) ? Vector3.left : Vector3.right;
        Vector3 rayOneOrigin = new Vector3(0f, -0.4f, 0f) + transform.position;

        if (Physics.Raycast(rayOneOrigin, direction, out hit1, rayLength))
        {

            Vector3 rayTwoOrigin = new Vector3(0f, 0.1f, 0f) + transform.position; //TODO usunac magiczne cyferki
            RaycastHit hit2;
            if (!Physics.Raycast(rayTwoOrigin, direction, out hit2, rayLength)) //0.1 + wysokosc voxla, TODO
            {
                Vector3 savedVelocity = wormScript.wormRb.velocity;
                transform.position = hit1.point + new Vector3(0, stepHeight, 0) - offset * direction;
                wormScript.wormRb.velocity = savedVelocity;
            }
        }
    }

    private void OnEnable()
    {
        cameraControlScript = FindObjectOfType<CameraControlScript>();
        if (cameraControlScript != null)
        {
            cameraControlScript.SetPosOnGameObject(this.gameObject);
        }
        canUseWeapon = true;
    }

    private void OnDisable()
    {
        if(currentItem != null)
        {
            currentItem.Hide();
        }
    }
}