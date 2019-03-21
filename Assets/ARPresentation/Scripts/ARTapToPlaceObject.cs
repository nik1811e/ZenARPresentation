using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    public Transform removeButton;
    private String objectName = "";

    private ARSessionOrigin arOrigin;
    private ARSession arSession;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arSession = FindObjectOfType<ARSession>();
        removeButton.GetComponent<Button>().interactable = false;
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void CreateANewObject()
    {
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var pos = new Vector3(placementPose.position.x, placementPose.position.y + objectToPlace.GetComponent<Transform>().position.y, placementPose.position.z);
            var gameObj = (GameObject) Instantiate(objectToPlace, pos,placementPose.rotation);
            objectName = gameObj.name;
            placementIndicator.GetComponent<Renderer>().enabled = false;
            removeButton.GetComponent<Button>().interactable = true;
        }
    }

    public void Remove()
    {
        Destroy(GameObject.Find(objectName));
        placementIndicator.GetComponent<Renderer>().enabled = true;
        removeButton.GetComponent<Button>().interactable = false;
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}