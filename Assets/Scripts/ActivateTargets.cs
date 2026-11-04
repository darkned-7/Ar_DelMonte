using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTargets : MonoBehaviour
{

    [SerializeField] private GameObject imageTracking;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateTrackingImage()
    {
        imageTracking.SetActive(true);
    }
}
