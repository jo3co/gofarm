using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

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

    public ARPlaneManager planeManager;

    public GameObject placementIndicator;

    public ARPlane arPlane;

    public GameObject[] farmPrefabs;

    private List<FarmPlaneObject> farmPlaneObjects;

    private bool cooldownFarmObject = false;

    public Text idRaycastLabel;
    public Text distanceLabel;
    public Text areaLabel;

    public Text takenLabel;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        farmPlaneObjects = new List<FarmPlaneObject>();
    }

    public void LeaveGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void OnApplicationPause( bool pauseStatus )
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void Update()
    {
        UpdatePlacementePose();
        
         if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
    {
        Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit))
        {
            Debug.Log("gofarm Something Hit : "+raycastHit.collider.name);
        }
    }
        
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

    private void createFarmObj(int id, float area, float dist, Pose position, Vector3 center) {
        Debug.Log("gofarm : #" + id + " plane area " + area.ToString() + " dist: " + dist.ToString());

        int randPrefabNumMin = UnityEngine.Random.Range(0, 4);
        int randPrefabNumMax = UnityEngine.Random.Range(4, 12);
        int rand = 0;
        if(area < 1.5f) {
            rand = randPrefabNumMin;
        }
        else {
            rand = randPrefabNumMax;
        }
        var obj = Instantiate(farmPrefabs[5], center, Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z))) as GameObject;
        // var objScale = new Vector3(obj.transform.localScale.x * dist, obj.transform.localScale.y * dist, obj.transform.localScale.z * dist);
        // obj.transform.localScale = objScale;

        var farmObj = new FarmPlaneObject();
        farmObj.gameId = id;
        farmObj.hasObj = true;
        farmObj.gameObj = obj;

        farmPlaneObjects.Add(farmObj);

        StartCoroutine(StartCooldownTimer());
    }

    IEnumerator StartCooldownTimer() {
        cooldownFarmObject = true;
        yield return new WaitForSeconds(10);

        cooldownFarmObject = false;
    }

    private void UpdatePlacementePose()
    {
        if(Camera.current == null) {
            return;
        }

        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        var rayCastMgr = GetComponent<ARRaycastManager>();
        rayCastMgr.Raycast(screenCenter, hits);

        if(hits.Count > 0) {
            placementPose =  hits[0].pose;

            var plane = planeManager.GetPlane(hits[0].trackableId);
            if(plane) {
                Debug.Log("gofarm plane raycast found");
                var planeArea = plane.size.x * plane.size.y;

                idRaycastLabel.text = plane.trackableId.GetHashCode().ToString();
                distanceLabel.text = hits[0].distance.ToString();
                areaLabel.text = planeArea.ToString();

                
                if(!hasFarmId(hits[0].trackableId.GetHashCode()) && planeArea > 1.0 && !cooldownFarmObject) {
                    //createFarmObj(hits[0].trackableId.GetHashCode(), planeArea, hits[0].distance, placementPose, plane.center);
                }
                else {
                    Debug.Log("plane already hit : #"+hits[0].trackableId.GetHashCode().ToString());
                }
            } 
            
        }
    }

    private float CalculatePlaneArea(ARPlane plane)  
    {        
        return plane.size.x * plane.size.y;
    }
}
