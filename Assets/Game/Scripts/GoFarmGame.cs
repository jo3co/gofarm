using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

using TMPro;

[System.Serializable]
public class FarmPlaneObject {
    public int gameId;
    public bool hasObj;

    public GameObject gameObj;
}

public class GoFarmGame : MonoBehaviour
{
    private ARSessionOrigin arOrigin;
    private Pose placementPose;

    private bool placementPoseIsValid = false;

    public ARPlaneManager planeManager;

    public GameObject placementIndicator;

    public ARPlane arPlane;

    public GameObject[] farmPrefabs;

    private List<FarmPlaneObject> farmPlaneObjects;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        farmPlaneObjects = new List<FarmPlaneObject>();
    }

    void Update()
    {
        UpdatePlacementePose();
        // UpdatePlacementIndicator();
    }

    private void UpdatePlacementIndicator()
    {
        // placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
    }

    private bool hasFarmId(int id) {
        bool found = false;
        foreach(FarmPlaneObject farm in farmPlaneObjects) {
            if(farm.gameId == id) {
                found = true;
            }
        }

        return found;
    }

    private void createFarmObj(int id, float size, float dist, Pose position) {
        Debug.Log("gofarm : #" + id + " plane area " + size.ToString() + " dist: " + dist.ToString());

        int randPrefabNum = UnityEngine.Random.Range(0, 12);
        var obj = Instantiate(farmPrefabs[randPrefabNum], position.position, Quaternion.identity) as GameObject;
        var farmObj = new FarmPlaneObject();
        farmObj.gameId = id;
        farmObj.hasObj = true;
        farmObj.gameObj = obj;

        farmPlaneObjects.Add(farmObj);
    }

    private void UpdatePlacementePose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        var rayCastMgr = GetComponent<ARRaycastManager>();
        rayCastMgr.Raycast(screenCenter, hits);

        if(hits.Count > 0) {
            placementPose =  hits[0].pose;

            var plane = planeManager.GetPlane(hits[0].trackableId);
            if(plane) {
                var planeArea = plane.size.x * plane.size.y;

                if(!hasFarmId(hits[0].trackableId.GetHashCode()) && planeArea > 1.0) {
                    createFarmObj(hits[0].trackableId.GetHashCode(), planeArea, hits[0].distance, placementPose);
                }
            } 
            
        }
    }

    private float CalculatePlaneArea(ARPlane plane)  
    {        
        return plane.size.x * plane.size.y;
    }
}
