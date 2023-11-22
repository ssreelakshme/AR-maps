using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions.Samples.Geospatial;
using AR_Fukuoka;
using TMPro;

namespace AR_Fukuoka
{
    public class SampleScript : MonoBehaviour
    {
        [SerializeField] AREarthManager EarthManager;
        [SerializeField] VpsInitializer Initializer;
        [SerializeField] Text OutputText;
        [SerializeField] double HeadingThreshold = 25;
        [SerializeField] double HorizontalThreshold = 20;
        [SerializeField] nearbyPlaces nearbyPlaces;
        public double Latitude;
        public double Longitude;
        float minDistanceThreshold = 1.5f;
        public double Altitude = 0;
        public double Heading = 0;
        public GameObject ContentPrefab;
        public ARAnchorManager AnchorManager;

        void Update()
        {
            if (!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
            {
                return;
            }

            string status = "";
            GeospatialPose pose = EarthManager.CameraGeospatialPose;
            status = "Creating markers";
            foreach (PlaceDetails placeDetails in nearbyPlaces.placesDetails.Values)
            {
                Latitude = placeDetails.latitude;
                Longitude = placeDetails.longitude;
                Altitude = pose.Altitude - 1.5f;
                Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);
                bool placeMarker = true; // New variable for density check

                foreach (ARAnchor existingAnchor in AnchorManager.trackables)
                {
                    float distance = Vector3.Distance(new Vector3((float)Latitude, (float)Altitude, (float)Longitude), existingAnchor.transform.position);

                    if (distance < minDistanceThreshold)
                    {
                        placeMarker = false;
                        break;
                    }
                }

                if (placeMarker)
                {
                    ARGeospatialAnchor anchor = AnchorManager.AddAnchor(Latitude, Longitude, Altitude, quaternion);

                    if (anchor != null)
                    {
                        GameObject displayObject = Instantiate(ContentPrefab, anchor.transform);
                        
                        TextMeshPro textMeshPro = displayObject.GetComponentInChildren<TextMeshPro>();
                        if (textMeshPro != null)
                        {
                            // Set the text values based on the placeDetails
                            textMeshPro.text = $"{placeDetails.name}\nRating: {placeDetails.rating}";
                            float zOffset = 0;
                            textMeshPro.transform.localPosition += new Vector3(0f, 0f, zOffset);
                        }
                    }
                }
            }


            Debug.Log($"GeospatialPose: {pose}");
            Debug.Log($"Latitude: {pose.Latitude}, Longitude: {pose.Longitude}, Altitude: {pose.Altitude}, Heading: {pose.EunRotation}");

            ShowTrackingInfo(status, pose);
        }

        void ShowTrackingInfo(string status, GeospatialPose pose)
        {
            Debug.Log("Setting text: " + OutputText.text);
            if (OutputText == null) return;

            Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);
            float headingInEulerAngles = quaternion.eulerAngles.y;

            OutputText.text = string.Format(
              "\n" +
              "Latitude/Longitude: {0}°, {1}°\n" +
              "Horizontal Accuracy: {2}m\n" +
               "Altitude: {3}m\n" +
              "Vertical Accuracy: {4}m\n" +
               "Heading: {5}°\n" +
               "Heading Accuracy: {6}°\n" +
              "{7} \n",
              pose.Latitude.ToString("F6"),  //{0}
              pose.Longitude.ToString("F6"),//{1}
                                            pose.HorizontalAccuracy.ToString("F6"), //{2}
                                            pose.Altitude.ToString("F2"),  //{3}
                                            pose.VerticalAccuracy.ToString("F2"),  //{4}
                                            headingInEulerAngles.ToString("F1"),   //{5}
                                           pose.OrientationYawAccuracy.ToString("F1"),   //{6}
                                            status //{7}
          );
        }
    }
}