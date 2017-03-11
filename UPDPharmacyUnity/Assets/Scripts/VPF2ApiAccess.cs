using UnityEngine;
using System.Collections;

public class VPF2ApiAccess : MonoBehaviour {

    public string clientID, clientSecret, ScenarioID;

    private string  accessTokenUrl =  "https://vpf2.cise.ufl.edu:35000/oauth2/token";
    private string baseURL;
    private string accessToken;
	// Use this for initialization
	void Start () {        
        baseURL = "https://vpf2.cise.ufl.edu:35000/api/Interaction/FindResponse?ScenarioID=" + ScenarioID + "&userinput=";
        StartCoroutine(GetAccessToken());        
    }
	
	// Update is called once per frame
	void Update () {
    }


    public IEnumerator FindResponse(string speechText, System.Action<string> callback)
    {
        var url = baseURL + WWW.EscapeURL(speechText) + "&access_token=" + accessToken;

        var www = new WWW(url);
        yield return www;

        string result = www.text;

        callback(result);        
    }

    IEnumerator GetAccessToken()
    {        
        var form = new WWWForm();

        form.AddField("grant_type", "client_credentials");

        var headers = form.headers;
        var rawData = form.data;

        headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(clientID + ":" + clientSecret));

        var www = new WWW(accessTokenUrl, rawData, headers);

        yield return www;

        JSONObject obj = new JSONObject(www.text);
        
        accessToken = obj["access_token"].str;

        Invoke("GetAccessToken", 3600);
        
    }
}
