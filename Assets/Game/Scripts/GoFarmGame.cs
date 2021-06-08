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

    public GameObject  whiteBorder;
    public GameObject greenBorder;

    public Image borderImage;

    private bool activeFarmObject = false;
    private GameObject currentFarmObject = null;

    public bool isGameEnabled = false;

    public AudioSource touchAudioSource;

    public Camera arCamera;

    private int currentScore;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRSession = FindObjectOfType<ARSession>();
        planeManager = FindObjectOfType<ARPlaneManager>();

        farmPlaneObjects = new List<FarmPlaneObject>();

        currentScore = 0;

        //arOrigin.enabled = false;
        //aRSession.enabled = false;
    }

    public void StartGame() {
        //uIMenuCanvas.enabled = false;
        //uIGameCanvas.enabled = true;

        //isGameEnabled = true;
        //arOrigin.enabled = true;
        //aRSession.enabled = true;
        
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

        PlayerPrefs.SetInt("score", currentScore);

        SceneManager.LoadScene(2);
    }

    public void RestartGame() {
        SceneManager.LoadScene(0);
    }

    void OnApplicationPause( bool pauseStatus )
    {
        if(pauseStatus) {
            SceneManager.LoadScene(0);
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

        touchAudioSource.Play(0);

        whiteBorder.SetActive(true);
        greenBorder.SetActive(false);

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        if(name.Contains("Cabra")) {
            Debug.Log("gofarm : Cabra");
            currentScore += 40;
        }
        if(name.Contains("Cachorro")) {
            Debug.Log("gofarm : Cachorro");
            currentScore += 70;
        }
        if(name.Contains("Cafe")) {
            Debug.Log("gofarm : Cafe");
            currentScore += 90;
        }
        if(name.Contains("Cavalo")) {
            Debug.Log("gofarm : Cavalo");
            currentScore += 60;
        }
        if(name.Contains("Galinha")) {
            Debug.Log("gofarm : Galinha");
            currentScore += 100;
        }
        if(name.Contains("Girasol")) {
            Debug.Log("gofarm : Girassol");
            currentScore += 50;
        }
        if(name.Contains("Laranja")) {
            Debug.Log("gofarm : Laranja");
            currentScore += 30;
        }
        if(name.Contains("Melancia")) {
            Debug.Log("gofarm : Melancia");
            currentScore += 70;
        }
        if(name.Contains("Pato")) {
            Debug.Log("gofarm : Pato");
            currentScore += 60;
        }
        if(name.Contains("Porco")) {
            Debug.Log("gofarm : Porco");
            currentScore += 80;
        }
        if(name.Contains("Vaca")) {
            Debug.Log("gofarm : Vaca");
            currentScore += 50;
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

        int rand = UnityEngine.Random.Range(0, 12);
        //int randPrefabNumMax = UnityEngine.Random.Range(4, 12);
        //int rand = 0;
        if(area < 1.5f) {
            //rand = randPrefabNumMin;
        }
        else {
            //rand = randPrefabNumMax;
        }

        currentFarmObject = Instantiate(farmPrefabs[rand], center, Quaternion.LookRotation(new Vector3(arCamera.transform.forward.x, 0, -arCamera.transform.forward.z))) as GameObject;
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

        whiteBorder.SetActive(false);
                    greenBorder.SetActive(true);

    }

    IEnumerator StartCooldownTimer() {
        cooldownFarmObject = true;
        yield return new WaitForSeconds(5);

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

                if(!activeFarmObject) {
                    whiteBorder.SetActive(false);
                    greenBorder.SetActive(true);
                }
                

                Debug.Log("gofarm create object "+hits[0].trackableId.ToString());
                
                if(!hasFarmId(hits[0].trackableId.ToString()) && planeArea > 0.4 && !cooldownFarmObject && !activeFarmObject) {
                    createFarmObj(hits[0].trackableId.ToString(), planeArea, hits[0].distance, placementPose, plane.center);
                }
                else {
                    // Debug.Log("plane already hit : #"+hits[0].trackableId.GetHashCode().ToString());
                }
            } 
            else {
                if(!activeFarmObject) {
                    whiteBorder.SetActive(true);
                    greenBorder.SetActive(false);
                }
            }
            
        }
    }

    private float CalculatePlaneArea(ARPlane plane)  
    {        
        return plane.size.x * plane.size.y;
    }
}
