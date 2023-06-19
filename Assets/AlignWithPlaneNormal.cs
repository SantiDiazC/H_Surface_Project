using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithPlaneNormal : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] 
    private LayerMask WhatIsGround;
    [SerializeField]
    private Transform panel;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        SurfaceAlignment();
    }

    private void SurfaceAlignment()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        Debug.DrawRay(transform.position, -5f*transform.up, Color.red);
        RaycastHit info = new RaycastHit();
        if (Physics.Raycast(ray, out info, WhatIsGround))
        {
            // Get the normal of the plane
            panel.rotation = Quaternion.FromToRotation(Vector3.up, info.normal);
        }
    }
}