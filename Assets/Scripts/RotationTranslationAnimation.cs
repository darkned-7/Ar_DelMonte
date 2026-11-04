using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTranslationAnimation : MonoBehaviour
{

    private enum Axis { X, Y, Z };

    [SerializeField]
    private Axis rotationAxis;

    [SerializeField]
    [Range(-2f, 5f)]
    private float rotationSpeed;

    void Start()
    {

    }

    void FixedUpdate()
    {
        Rotation();
    }

    public void Rotation()
    {
        switch (rotationAxis)
        {
            case Axis.X:
                transform.Rotate(rotationSpeed * Vector3.right);
                break;
            case Axis.Y:
                transform.Rotate(rotationSpeed * Vector3.up);
                break;
            case Axis.Z:
                transform.Rotate(rotationSpeed * Vector3.forward);
                break;
        }
    }
}
