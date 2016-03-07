using UnityEngine;
using System;
using System.Collections;
using Vuforia;

public class CloudHandler : MonoBehaviour, ICloudRecoEventHandler {
	public ImageTargetBehaviour ImageTargetTemplate;
	public string username;
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private string mTargetMetadata;
	public string BaseURL; //should point to the folder on your server where users' models are stored

	// Use this for initialization
	void Start () {
		// register this event handler at the cloud reco behaviour
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (mCloudRecoBehaviour){
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}

	}

	// Update is called once per frame
	void Update () {

	}

	public void OnInitialized() {
		Debug.Log ("Cloud Reco initialized");
	}
	public void OnInitError(TargetFinder.InitState initError) {
		Debug.Log ("Cloud Reco init error " + initError.ToString());
	}
	public void OnUpdateError(TargetFinder.UpdateState updateError) {
		Debug.Log ("Cloud Reco update error " + updateError.ToString());
	}

	public void OnStateChanged(bool scanning) {
		if (scanning)
		{
			// clear all known trackables
				ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.TargetFinder.ClearTrackables(false);
		}
	}

	// Here we handle a cloud target recognition event
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult) {
		// First clear all trackables
		TrackerManager.Instance.GetTracker<ObjectTracker>().TargetFinder.ClearTrackables(false);

		// load assetbundle based on metadata
		mTargetMetadata = targetSearchResult.MetaData;
		StartCoroutine(Download(username, mTargetMetadata));

			// Build augmentation based on target
		if (ImageTargetTemplate) {
			// enable the new result with the same ImageTargetBehaviour:
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker> ();
			ImageTargetBehaviour imageTargetBehaviour =
				(ImageTargetBehaviour)tracker.TargetFinder.EnableTracking (
					targetSearchResult, ImageTargetTemplate.gameObject);
		}
		

	}

	//modified example code to download assetbundle
	IEnumerator Download(string username, string modelName){

		//Compute request url for server
		//string BundleURL = BaseURL + "/" + username + "/" + modelName;
		//string BundleURL = BaseURL+"?username="+username+"&modelFileName="+modelName;
		UriBuilder uriBuilder = new UriBuilder();
		uriBuilder.Scheme = "http";
		uriBuilder.Host = BaseURL;
		uriBuilder.Path = username.Trim() + "/" + modelName.Trim();
		string BundleURL = uriBuilder.ToString ();
		Debug.Log ("Attempting to download " + BundleURL);

		// Download desired assetbundle
		using (WWW www = new WWW(BundleURL)) {
			yield return www;
			if (www.error != null)
				throw new Exception("WWW download had an error:" + www.error);
			AssetBundle bundle = www.assetBundle;
			Debug.Log ("Got assets "+string.Join(",", bundle.GetAllAssetNames ()));
			GameObject loadedModel = Instantiate(bundle.LoadAllAssets()[0]) as GameObject;
		
			//attach to the image target
			loadedModel.transform.parent = ImageTargetTemplate.gameObject.transform;
			loadedModel.transform.localScale = new Vector3 (1, 1, 1);

			// Unload the AssetBundles compressed contents to conserve memory
			bundle.Unload(false);

		} // memory is freed from the web stream (www.Dispose() gets called implicitly)
	}

/*
	void OnGUI() {
		// Display current 'scanning' status
		GUI.Box (new Rect(100,100,200,50), mIsScanning ? "Scanning" : "Not scanning");
		// Display metadata of latest detected cloud-target
		GUI.Box (new Rect(100,200,200,50), "Metadata: " + mTargetMetadata);
		// If not scanning, show button
		// so that user can restart cloud scanning
		if (!mIsScanning) {
			if (GUI.Button(new Rect(100,300,200,50), "Restart Scanning")) {
				// Restart TargetFinder
				mCloudRecoBehaviour.CloudRecoEnabled = true;
			}
		}
	}
*/
}
