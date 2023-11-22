using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using AR_Fukuoka;

[System.Serializable]
public class LocationData
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class GeometryData
{
    public LocationData location;
}

[System.Serializable]
public class PlaceData
{
    public string name;
    public double rating;
    public GeometryData geometry;
}

public class PlaceDetails
{
    public double rating;
    public double latitude;
    public double longitude;
    public string placeName;
    public string name;
    public PlaceDetails(string name,double rating, double latitude, double longitude)
    {
        this.name = name;
        this.rating = rating;
        this.latitude = latitude;
        this.longitude = longitude;
        

    }
}

public class nearbyPlaces : MonoBehaviour
{
    string apiKey = "APIKEY";
    string baseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";

    public Dictionary<string, PlaceDetails> placesDetails = new Dictionary<string, PlaceDetails>();

    IEnumerator Start()
    {
        string keyword = "School";
        string location = "13.12735227359088, 77.58798019953939"; // Replace with the desired latitude and longitude
        int radius = 1000; // Radius in meters

        string url = $"{baseUrl}location={location}&radius={radius}&keyword={keyword}&key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            Debug.Log(jsonResponse);

            // Deserialize JSON and extract place details
            ExtractPlaceDetailsFromJSON(jsonResponse);
        }
    }

    void ExtractPlaceDetailsFromJSON(string jsonResponse)
    {
        // Deserialize the JSON response
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

        if (responseData != null && responseData.results != null)
        {
            foreach (PlaceData place in responseData.results)
            {
                string name = place.name;
                double rating = place.rating;
                double latitude = place.geometry.location.lat;
                double longitude = place.geometry.location.lng;

                // Store details in the dictionary
                placesDetails[name] = new PlaceDetails(name,rating, latitude, longitude);
            }
        }

        // Access place details from the dictionary as needed
        foreach (var pair in placesDetails)
        {
            string name = pair.Key;
            PlaceDetails details = pair.Value;
            Debug.Log($"Name: {name}, Rating: {details.rating}, Latitude: {details.latitude}, Longitude: {details.longitude}");
        }
    }
}

[System.Serializable]
public class ResponseData
{
    public List<PlaceData> results;
}
