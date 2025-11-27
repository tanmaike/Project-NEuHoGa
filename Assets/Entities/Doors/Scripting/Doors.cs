using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, IInteractable
{
    public bool IsOpen = false;
    public bool isPaired = false;
    public Door pairedDoor;
    [Header("Portal Configs")]
    public bool isPortalDoor = false;
    public Portal portal;
    [SerializeField]
    private float Speed = 1f;
    [SerializeField]
    private bool IsRotatingDoor = true;
    [Header("Rotation Configs")]
    [SerializeField]
    private float RotationAmount = 90f;
    [SerializeField]
    private float ForwardDirection = 0;
    [Header("Sliding Configs")]
    [SerializeField]
    private Vector3 SlideDirection = Vector3.back;
    [SerializeField]
    private float SlideAmount = 1.9f;

    private Vector3 StartRotation;
    private Vector3 StartPosition;
    private Vector3 Forward;

    [Header("Key Configuration")]
    [SerializeField]
    public bool needsKey = false;
    public Item requiredKey;

    [Header("Navmesh Configs")]
    public NavMeshObstacle navObstacle;
    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        if (isPortalDoor) portal.gameObject.SetActive(false);
        StartRotation = transform.localEulerAngles;
        Forward = transform.right;
        StartPosition = transform.position;
        
        if (navObstacle != null) navObstacle.enabled = false;
    }


    public void Interact(Vector3 interactorPosition, Item item)
    {
        if (needsKey && item != requiredKey)
        {
            Debug.Log("Door is locked. Requires key: " + requiredKey.itemName);
            return;
        }
        else needsKey = false; // permamnently unlocks the door
        if (!IsOpen)
        {
            if (isPaired) pairedDoor.Open(interactorPosition); 
            Open(interactorPosition);
        }
        else
        {
            if (isPaired) pairedDoor.Close();
            Close();
        }
    }

    private Vector3 GetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return player.transform.position;
        }
        
        return transform.position + transform.forward * 2f;
    }

    public void Open(Vector3 UserPosition)
    {
        if (!IsOpen)
        {
            if (isPortalDoor) portal.gameObject.SetActive(true);
            if (navObstacle != null) navObstacle.enabled = true;
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
                AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
            else
            {
                AnimationCoroutine = StartCoroutine(DoSlidingOpen()); 
            }
        }
    }

    private IEnumerator DoRotationOpen(float ForwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (ForwardAmount >= ForwardDirection)
        {
            endRotation = Quaternion.Euler(StartRotation.x, StartRotation.y, StartRotation.z + RotationAmount);
        }
        else
        {
            endRotation = Quaternion.Euler(StartRotation.x, StartRotation.y, StartRotation.z - RotationAmount);
        }

        IsOpen = true;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
        
        transform.rotation = endRotation;
    }

    private IEnumerator DoSlidingOpen()
    {
        Vector3 endPosition = StartPosition + SlideAmount * SlideDirection;
        Vector3 startPosition = transform.position;

        float time = 0;
        IsOpen = true;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }

    public void Close()
    {
        if (IsOpen)
        {
            if (navObstacle != null)
                    navObstacle.enabled = false;
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
            else
            {
                AnimationCoroutine = StartCoroutine(DoSlidingClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        IsOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
        
        transform.rotation = endRotation;
        if (isPortalDoor) portal.gameObject.SetActive(false);
    }

    private IEnumerator DoSlidingClose()
    {
        Vector3 endPosition = StartPosition;
        Vector3 startPosition = transform.position;
        float time = 0;

        IsOpen = false;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
        if (isPortalDoor) portal.gameObject.SetActive(false);
    }
}