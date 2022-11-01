using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float minZoomDist;
    [SerializeField] private float maxZoomDist;

    private Camera cam;

    public static bool AllowMovement = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        NakamaMatchHandler.MatchStart += OnMatchStart;
    }

    void OnMatchStart(IMatchState newState)
    {
        AllowMovement = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (AllowMovement)
        {
            Move();
            Zoom();

        }
    }
    private void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.forward * zInput + transform.right * xInput;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
    private void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float dist = Vector3.Distance(transform.position, cam.transform.position);

        if (dist < minZoomDist && scrollInput > 0.0f)
            return;
        else if (dist > maxZoomDist && scrollInput < 0.0f)
            return;

        cam.transform.position += cam.transform.forward * scrollInput * zoomSpeed;
    }
}
