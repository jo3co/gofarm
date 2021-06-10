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

    public GameObject[] farmPrefabs;

    public GameObject[] farmFXPrefabs;

    private List<FarmPlaneObject> farmPlaneObjects;

    private bool cooldownFarmObject = false;

    public GameObject  whiteBorder;
    public GameObject greenBorder;

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
    }

    public void StartGame() {
        
    }

    public void LeaveGame ()
    {
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
                if(raycastHit.collider.tag.Equals("FarmObj")) {
                    captureFarmObject(raycastHit.collider.name);
                }
            }
        }      
    }

    private void captureFarmObject(String name) {
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
            currentScore += 40;
        }
        if(name.Contains("Cachorro")) {
            currentScore += 70;
        }
        if(name.Contains("Cafe")) {
            currentScore += 90;
        }
        if(name.Contains("Cavalo")) {
            currentScore += 60;
        }
        if(name.Contains("Galinha")) {
            currentScore += 100;
        }
        if(name.Contains("Girasol")) {
            currentScore += 50;
        }
        if(name.Contains("Laranja")) {
            currentScore += 30;
        }
        if(name.Contains("Melancia")) {
            currentScore += 70;
        }
        if(name.Contains("Pato")) {
            currentScore += 60;
        }
        if(name.Contains("Porco")) {
            currentScore += 80;
        }
        if(name.Contains("Vaca")) {
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

        int rand = UnityEngine.Random.Range(0, 11);
        //int randPrefabNumMax = UnityEngine.Random.Range(4, 12);
        //int rand = 0;
        if(area < 1.5f) {
            //rand = randPrefabNumMin;
        }
        else {
            //rand = randPrefabNumMax;
        }

        currentFarmObject = Instantiate(farmPrefabs[rand], center, Quaternion.LookRotation(new Vector3(arCamera.transform.forward.x, 0, -arCamera.transform.forward.z))) as GameObject;
        currentFarmObject.SetActive(true);
        if(dist < 1.5f) {
            float minScale = 0.7f;
            if(dist < 0.5) {
                minScale = 0.3f;
            }
            var objScale = new Vector3(
            currentFarmObject.transform.localScale.x * minScale,
            currentFarmObject.transform.localScale.y * minScale,
            currentFarmObject.transform.localScale.z * minScale);

            currentFarmObject.transform.localScale = objScale;
        }
        else {
            float minScale = 1.2f;
            var objScale = new Vector3(
            currentFarmObject.transform.localScale.x * minScale,
            currentFarmObject.transform.localScale.y * minScale,
            currentFarmObject.transform.localScale.z * minScale);

            currentFarmObject.transform.localScale = objScale;
        }

        int randFX = UnityEngine.Random.Range(0, 5);
        var farmFXObj = Instantiate(farmFXPrefabs[randFX], center, Quaternion.identity);
        farmFXObj.transform.localPosition = new Vector3(0f, 1.0f, 0f);
        farmFXObj.transform.parent = currentFarmObject.transform;

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
                var planeArea = plane.size.x * plane.size.y;

                if(!activeFarmObject) {
                    whiteBorder.SetActive(false);
                    greenBorder.SetActive(true);
                }
                                
                if(!hasFarmId(hits[0].trackableId.ToString()) && planeArea > 0.4 && !cooldownFarmObject && !activeFarmObject) {
                    createFarmObj(hits[0].trackableId.ToString(), planeArea, hits[0].distance, placementPose, hits[0].pose.position);
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
