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
    public String gameId;
    public bool hasObj;

    public GameObject gameObj;
}

public class GoFarmGame : MonoBehaviour
{
    private ARSessionOrigin arOrigin;
    private ARSession aRSession;

    private Pose placementPose;

    public ARPlaneManager planeManager;

    public GameObject placementIndicator;

    public ARPlane arPlane;

    public GameObject[] farmPrefabs;

    private List<FarmPlaneObject> farmPlaneObjects;

    private bool cooldownFarmObject = false;

    //public Text idRaycastLabel;
    //public Text distanceLabel;
    //public Text areaLabel;
    //public Text takenLabel;

    public Sprite whiteBorder;
    public Sprite greenBorder;

    public Image borderImage;

    private bool activeFarmObject = false;
    private GameObject currentFarmObject = null;

    public Canvas uIMenuCanvas;
    public Canvas uIGameCanvas;

    public bool isGameEnabled = false;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRSession = FindObjectOfType<ARSession>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        farmPlaneObjects = new List<FarmPlaneObject>();

        arOrigin.enabled = false;
        aRSession.enabled = false;
    }

    public void StartGame() {
        //uIMenuCanvas.enabled = false;
        //uIGameCanvas.enabled = true;

        isGameEnabled = true;
        arOrigin.enabled = true;
        aRSession.enabled = true;
    }

    public void LeaveGame ()
    {
        //uIMenuCanvas.enabled = true;
        //uIGameCanvas.enabled = false;
        isGameEnabled = false;

        if(currentFarmObject) {
            Destroy(currentFarmObject);
            currentFarmObject = null;
        }

        farmPlaneObjects.Clear();

        aRSession.Reset();
        arOrigin.enabled = false;
        aRSession.enabled = false;

    }

    void OnApplicationPause( bool pauseStatus )
    {
        if(pauseStatus) {
            LeaveGame();
        }
        
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
            Debug.Log("gofarm raycast found : "+raycastHit.collider.tag);

            if(raycastHit.collider.tag.Equals("FarmObj")) {
               
                captureFarmObject(raycastHit.collider.name);
                
            }
            
        }
    }
        
    }

    private void captureFarmObject(String name) {
        Debug.Log("gofarm capture objcet "+name);

        Destroy(currentFarmObject);
        currentFarmObject = null;

        activeFarmObject = false;

        farmPlaneObjects.Clear();

        StartCoroutine(StartCooldownTimer());

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        if(name.Contains("Cabra")) {
            Debug.Log("gofarm : Cabra");
        }
        if(name.Contains("Cachorro")) {
            Debug.Log("gofarm : Cachorro");
        }
        if(name.Contains("Cafe")) {
            Debug.Log("gofarm : Cafe");
        }
        if(name.Contains("Cavalo")) {
            Debug.Log("gofarm : Cavalo");
        }
        if(name.Contains("Galinha")) {
            Debug.Log("gofarm : Galinha");
        }
        if(name.Contains("Girassol")) {
            Debug.Log("gofarm : Girassol");
        }
        if(name.Contains("Laranja")) {
            Debug.Log("gofarm : Laranja");
        }
        if(name.Contains("Melancia")) {
            Debug.Log("gofarm : Melancia");
        }
        if(name.Contains("Pato")) {
            Debug.Log("gofarm : Pato");
        }
        if(name.Contains("Porco")) {
            Debug.Log("gofarm : Porco");
        }
        if(name.Contains("Vaca")) {
            Debug.Log("gofarm : Vaca");
        }
    }


    private bool hasFarmId(String id) {
        bool found = false;
        foreach(FarmPlaneObject farm in farmPlaneObjects) {
            if(farm.gameId.Equals(id)) {
                found = true;
                Debug.Log("gofarm HAS FARM ID");
            }
        }

        return found;
    }

    private void createFarmObj(String id, float area, float dist, Pose position, Vector3 center) {
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
        currentFarmObject = Instantiate(farmPrefabs[3], center, Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z))) as GameObject;
        if(dist < 1.5) {
            float minScale = 0.7f;
            if(dist < 0.8) {
                minScale = 0.3f;
            }
            var objScale = new Vector3(
            currentFarmObject.transform.localScale.x * minScale,
            currentFarmObject.transform.localScale.y * minScale,
            currentFarmObject.transform.localScale.z * minScale);

        currentFarmObject.transform.localScale = objScale;
        }
        

        var farmObj = new FarmPlaneObject();
        farmObj.gameId = id;
        farmObj.hasObj = true;
        farmObj.gameObj = currentFarmObject;

        farmPlaneObjects.Add(farmObj);

        activeFarmObject = true;

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

        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        var rayCastMgr = GetComponent<ARRaycastManager>();
        rayCastMgr.Raycast(screenCenter, hits);

        if(hits.Count > 0) {
            placementPose =  hits[0].pose;

            var plane = planeManager.GetPlane(hits[0].trackableId);
            if(plane && plane.gameObject.activeSelf) {
//                 Debug.Log("gofarm plane raycast found");
                var planeArea = plane.size.x * plane.size.y;

                //idRaycastLabel.text = plane.trackableId.GetHashCode().ToString();
                //distanceLabel.text = hits[0].distance.ToString();
                //areaLabel.text = planeArea.ToString();

                borderImage.sprite = greenBorder;

                Debug.Log("gofarm create object "+hits[0].trackableId.ToString());
                
                if(!hasFarmId(hits[0].trackableId.ToString()) && planeArea > 0.4 && !cooldownFarmObject && !activeFarmObject) {
                    createFarmObj(hits[0].trackableId.ToString(), planeArea, hits[0].distance, placementPose, plane.center);
                }
                else {
                    // Debug.Log("plane already hit : #"+hits[0].trackableId.GetHashCode().ToString());
                }
            } 
            else {
                if(borderImage.sprite != whiteBorder)
                    borderImage.sprite = whiteBorder;
            }
            
        }
    }

    private float CalculatePlaneArea(ARPlane plane)  
    {        
        return plane.size.x * plane.size.y;
    }
}
