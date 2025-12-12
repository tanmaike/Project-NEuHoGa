using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform PlayerCamera;
    public void Update()
    {
        transform.LookAt(PlayerCamera);
    }
}
