using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

using TMPro;

public class ArTapCreate : MonoBehaviour
{
    private ARSessionOrigin arOrigin;
    private Pose placementPose;

    private bool placementPoseIsValid = false;

    public GameObject placementIndicator;

    public TextMeshPro areaText;
    public ARPlane arPlane;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    void Update()
    {
        areaText.transform.rotation = 
        Quaternion.LookRotation(areaText.transform.position - 
           Camera.main.transform.position);
           
        UpdatePlacementePose();
        UpdatePlacementIndicator();
    }

    private void UpdatePlacementIndicator()
    {
        if(placementPoseIsValid) {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementePose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        var rayCastMgr = GetComponent<ARRaycastManager>();
        rayCastMgr.Raycast(screenCenter, hits);

        placementPoseIsValid = hits.Count > 0;

        if(placementPoseIsValid) {
            placementPose =  hits[0].pose;
        }
        
    }

    private void ArPlane_BoundaryChanged(ARPlaneBoundaryChangedEventArgs obj)
    {
        areaText.text = CalculatePlaneArea(arPlane).ToString();
    }

    private float CalculatePlaneArea(ARPlane plane)  
    {        
        return plane.size.x * plane.size.y;
    }

    public void ToggleAreaView()
    {
        if(areaText.enabled)
            areaText.enabled = false;
        else
            areaText.enabled = true;
    }
}
