using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static event Action<Vector3> MovementCommand = delegate { };
    public static event Action<Unit> AttackCommand = delegate { };
    public static InputHandler Instance;
    private Camera cam;

    private static bool captureInput = true;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        Instance = this;
    }

    public static void StopCapturingInput()
    {
        captureInput = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!captureInput) return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
                HandleRightClick(hit);
        }
    }

    void HandleRightClick(RaycastHit hit)
    {
        // Handle terrain click
        Vector3 hitPosition;

        if (hit.transform.CompareTag("Terrain"))
        {
            hitPosition = hit.point;
            MovementCommand.Invoke(hitPosition);
        }

        // Handle unit click
        if (hit.transform.CompareTag("Unit"))
        {
            Unit unit = hit.transform.GetComponent<Unit>();

            if (unit.Team != PlayerManager.PlayerTeam)
            {
                AttackCommand.Invoke(unit);
            }
        }
    }
}
