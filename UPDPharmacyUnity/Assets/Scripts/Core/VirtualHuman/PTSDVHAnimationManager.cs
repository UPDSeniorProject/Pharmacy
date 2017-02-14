using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 
/// </summary>
public class PTSDVHAnimationManager : RenBehaviour
{
	
	#region Animation Lists
	protected List<string> animationList;
	protected bool HasLoaded = false;
	
	protected AnimationClip currentlyPlayingIdleAnimation;
	protected AnimationClip currentlyPlayingAnimation;
	protected bool okToIdle = true;
	
	public List<AnimationClip> idleAnimations = new List<AnimationClip>();
	public List<AnimationClip> talkingAnimations = new List<AnimationClip>();
	//private List<AnimationClip> idleAnimationsTemp = new List<AnimationClip>();
	
	/// <summary>
	/// Use this function to add the base animation to the idle least by default. 
	/// This is a convinience functionality to have something added automatically to idle.
	/// </summary>
	public bool IsBaseAnimationIdle = true;
	#endregion
	
	#region GUI fields
	public RenComboBox ComboBox = new RenComboBox();
	public RenButton PlayButton = new RenButton("Play Animation");
	public RenButton PlayLipSyncButton = new RenButton("Play LipSync");
	public RenButton PlayHeadAnimationButton = new RenButton("Play Head Animation");
	protected bool needComboBoxUpdate = true;
	public Rect GUIPosition = new Rect(0, 0, 300, 20);
	public bool ButtonOnRight = true;
	public bool displayGUI = true;
	#endregion
	
	#region VH References 
	private FaceFXControllerScript ffx;
	private HeadLookBehaviour HeadLook;
	#endregion
	
	#region Look At
	public GameObject LookAt =null;
	#endregion
	
	#region Startup methods
	
	protected override void Awake()
	{
		base.Awake();
		
		//Debug.Log( "VHAnimManager Awake call: " + gameObject.name);  
		animationList = new List<string>();
		
		
		
		ffx = gameObject.GetComponent<FaceFXControllerScript>();
		if (ffx == null)
		{//This could lead to all sorts of issues.
	//		AddDebugLine("Virtual Human doesn't have FaceFXController!!!");
		}else{
	//		Debug.Log ("Found FFX with parent gameObject: "  + ffx.gameObject);
			
		}
		
		HeadLook = gameObject.GetComponent<HeadLookBehaviour>();
		if (HeadLook == null)
		{
	//		AddDebugLine("Virtual Human doesn't have HeadLookBehaviour!");
		}
		else if(LookAt != null)
		{
			HeadLook.SetLookTarget(LookAt);
		}else{
			//The game object of the main camera
			HeadLook.SetLookTarget(Camera.main.gameObject);
	//		Debug.Log ("Found HeadLook with parent gameObject: "  + HeadLook.gameObject);
		}
		
		//Add Base animation as idle
		if (IsBaseAnimationIdle && gameObject.GetComponent<Animation>().clip != null)
		{
			idleAnimations.Add(gameObject.GetComponent<Animation>().clip);
		}
		
	}
	
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		this.AddGUIElement(ComboBox, false);
		this.AddGUIElement(PlayButton, false);
		this.AddGUIElement(PlayLipSyncButton, false);
		this.AddGUIElement(PlayHeadAnimationButton, false);
		PlayButton.ButtonPressed += new ButtonPressedEventHandler(PlayButton_ButtonPressed);
		PlayLipSyncButton.ButtonPressed += new ButtonPressedEventHandler(PlayLipSync_ButtonPressed);
		PlayHeadAnimationButton.ButtonPressed += PlayHeadAnimationButton_ButtonPressed;
		
		//Debug.Log ("handlers for lipsync button: "  + PlayLipSyncButton.CountButtonPressedEventHandlers());
		
		int i =0;
		
		foreach (AnimationState state in gameObject.GetComponent<Animation>())
		{
			//Debug.Log ("Animation " + i +" Name : " + state.name);
			i++;
			if(!state.name.StartsWith("facefx ")){
				//We ignore the facefx visemes.
				//Main reason for this is there are a lot. Also, they won't play
				//correctly with the calls from this animation manager.
				//Need to figure out a way to be able to play them.
				animationList.Add(state.name);
			}
			if(state.name.StartsWith("idle")){
				idleAnimations.Add (state.clip);
			}else if(state.name.StartsWith("talking")){
				talkingAnimations.Add (state.clip);
			}
		}

		//Debug.Log ("IDLE ANIMTIONS COUNT : "+ idleAnimations.Count);

		if (idleAnimations.Count > 0)
		{
			//PlayIdleAnimationTesting();
			List<AnimationClip> idleAnimationsTemp = new List<AnimationClip>();
			//StartCoroutine("PlayIdleAnimationTesting",idleAnimationsTemp, 0.5F);
			StartCoroutine(PlayIdleAnimationTesting(idleAnimationsTemp,0.25F));
		}
		
		HasLoaded = true;
	}
	
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		UpdateGUI();
	}
	
	#endregion
	
	#region GUI Functions
	private void UpdateGUI()
	{
		ComboBox.ShouldRender = PlayButton.ShouldRender = PlayLipSyncButton.ShouldRender = PlayHeadAnimationButton.ShouldRender = displayGUI;
		if (displayGUI)
		{
			if (needComboBoxUpdate)
			{
				needComboBoxUpdate = false;
				ComboBox.SetItems(animationList);
			}
			
			ComboBox.Position = GUIPosition;
			PlayButton.Position = GUIPosition;
			PlayLipSyncButton.Position = GUIPosition;
			PlayLipSyncButton.Position.y += GUIPosition.height + 10;
			
			PlayHeadAnimationButton.Position = GUIPosition;
			PlayHeadAnimationButton.Position.y = (GUIPosition.height + 10) * 2;
			
			if (ButtonOnRight)
			{
				PlayButton.Position.x += GUIPosition.width + 10;
				PlayLipSyncButton.Position.x += GUIPosition.width + 10;
				PlayHeadAnimationButton.Position.x += GUIPosition.width + 10;
				
			}
			else
			{
				ComboBox.Position.x += GUIPosition.width + 10;
			}
		}
	}
	void PlayButton_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
	{
		if (args.button == MouseButton.MOUSE_LEFT)
		{
			PlayAnimation(ComboBox.SelectedItem.text);
		}
	}
	
	protected virtual void PlayLipSync_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
	{
		if (args.button == MouseButton.MOUSE_LEFT)
		{
			PlayLipSync(ComboBox.SelectedItem.text);
		}
	}
	
	
	void PlayHeadAnimationButton_ButtonPressed(RenButton btn, ButtonPressedEventArgs args)
	{
		if (args.button == MouseButton.MOUSE_LEFT)
		{
			PlayHeadAnimation(ComboBox.SelectedItem.text);
		}
	}
	
	
	#endregion
	
	#region Manage Clips
	public bool ContainsClipWithName(string name)
	{
		return animationList.Contains(name);
	}
	
	public void AddClip(AnimationClip clip, string name)
	{
		AddClip(clip, name, false);
	}
	
	public void AddClip(AnimationClip clip, string name, bool overwrite) 
	{
		needComboBoxUpdate = true;
		if (!ContainsClipWithName(name))
		{
			animationList.Add(name); //add it to the list.
			gameObject.GetComponent<Animation>().AddClip(clip, name);
		}
		else if (overwrite)
		{
			gameObject.GetComponent<Animation>().RemoveClip(name);
			gameObject.GetComponent<Animation>().AddClip(clip, name);
		}
		else
		{
			AddDebugLine("Trying to add clip with name: " + name + " with no overwrite and one already exists!");
		}
	}
	
	public void RemoveClip(string name)
	{
		needComboBoxUpdate = true;
		animationList.Remove(name);
		gameObject.GetComponent<Animation>().RemoveClip(name);
	}
	
	#region Base Clip functions
	
	public void SetBaseClip(AnimationClip clip)
	{
		SetBaseClip(clip, true);
	}
	
	public void SetBaseClip(AnimationClip clip, bool play)
	{
		if (!ContainsClipWithName(clip.name))
		{
			AddClip(clip, clip.name);
		}
		
		gameObject.GetComponent<Animation>().clip = clip;
		if (play)
		{
			PlayAnimation(clip.name);
		}
	}
	
	#endregion
	
	#endregion
	
	#region Play Animation Code
	
	public void PlayAnimation(AnimationClip clip)
	{
		if (!ContainsClipWithName(clip.name))
		{
			AddClip(clip, clip.name);
		}
		
		PlayAnimation(clip.name);
	}
	
	
	public void PlayHeadAnimation(string anim, string idleToPlay = null, float fadeLength = 0.5f)
	{
		//Disable headLook
		HeadLook.enabled = false;
		CrossFadeOverAndReturnToIdle(anim, idleToPlay, fadeLength);
	}
	
	public void PlayAnimation(string animation)
	{
		if (ContainsClipWithName(animation))
		{
			if (!gameObject.GetComponent<Animation>().Play(animation))
			{
				AddDebugLine("Could not play animation: " + animation);
			}
		}
		else
		{
			AddDebugLine("Couldn't find animation with name: " + animation + ". Are you sure it has been added?");
		}
	}
	
	public void PlayAnimation(string animation, int layer, WrapMode wrapMode = WrapMode.Default , AnimationBlendMode blendMode = AnimationBlendMode.Blend)
	{
		
		if (ContainsClipWithName(animation))
		{
			AnimationState state = gameObject.GetComponent<Animation>()[animation];
			if (layer != -1)
			{
				state.layer = layer;
			}
			state.wrapMode = wrapMode;
			state.blendMode = blendMode;
			gameObject.GetComponent<Animation>().Play(animation);
		}
		else
		{
			AddDebugLine("Couldn't find animation with name: " + animation + ". Are you sure it has been added?");
		}
	}
	
	public void RepeatAnimationAndReturnToIdle(string animation, float fadeLength, WrapMode wrapMode, int animationLayer, int repeat)
	{
		string idle;
		if(ContainsClipWithName(animation))
		{
			currentlyPlayingAnimation = gameObject.GetComponent<Animation>().GetClip (animation);
			if (idleAnimations.Count > 0)
			{
				//get the idle.
				idle = idleAnimations[Random.Range(0, idleAnimations.Count)].name;
			}
			else
			{
				//the main clip should be an idle.
				idle = gameObject.GetComponent<Animation>().clip.name;
			}
			
			gameObject.GetComponent<Animation>().CrossFade(animation, fadeLength, PlayMode.StopSameLayer);
			
			StartCoroutine(PlayIdleAnimationWhenLoopFinished(currentlyPlayingAnimation, idle, currentlyPlayingAnimation.length/2.0f, repeat));
		}
		else
		{
			AddDebugLine("Couldn't find animation with name: " + animation + ". Are you sure it has been added?");
		}
	}
	
	
	public void CrossFadeAnimation(string animation, float fadeLength = 0.5f, WrapMode wrapMode = WrapMode.Once, int animationLayer = 1)
	{
		if (ContainsClipWithName(animation))
		{
			currentlyPlayingAnimation = gameObject.GetComponent<Animation>().GetClip(animation);
			
			gameObject.GetComponent<Animation>()[animation].layer = animationLayer;
			gameObject.GetComponent<Animation>()[animation].wrapMode = wrapMode;
			
			gameObject.GetComponent<Animation>().CrossFade(animation, fadeLength, PlayMode.StopSameLayer);
		}
		else
		{
			AddDebugLine("Couldn't find animation with name: " + animation + ". Are you sure it has been added?");
		}
	}
	
	
	public bool CrossFadeOverAndReturnToIdle(string animation, string idle = null, float fadeLength = 0.5f)
	{
		if (ContainsClipWithName(animation))
		{
			currentlyPlayingAnimation = gameObject.GetComponent<Animation>().GetClip(animation);
			if (idle == null)
			{
				if (idleAnimations.Count > 0)
				{
					//get the idle.
					idle = idleAnimations[Random.Range(0, idleAnimations.Count)].name;
				}
				else
				{
					//the main clip should be an idle.
					idle = gameObject.GetComponent<Animation>().clip.name;
				}
			}
			
			gameObject.GetComponent<Animation>().CrossFade(animation, fadeLength, PlayMode.StopSameLayer);
			
			StartCoroutine(PlaySingleIdleAnimationWhenFinished(currentlyPlayingAnimation, idle, fadeLength));
			return true;
		}
		else
		{
			AddDebugLine("Couldn't find animation with name: " + animation + ". Are you sure it has been added?");
			return false;
		}
	}
	
	public void CrossFadeOverIdleAnimation(string animation, float fadeLength = 0.5f)
	{
		if (ContainsClipWithName(animation))
		{
			okToIdle = false;
			currentlyPlayingAnimation = gameObject.GetComponent<Animation>().GetClip(animation);
			
			gameObject.GetComponent<Animation>().CrossFade(animation, fadeLength, PlayMode.StopAll);
			
			StartCoroutine(PlayIdleAnimationWhenFinished(currentlyPlayingAnimation, fadeLength));
		}
		else
		{
			AddDebugLine("Couldn't find animation with name: " + animation + ". Are you sure it has been added?");
		}	
	}
	
	public IEnumerator PlaySingleIdleAnimationWhenFinished(AnimationClip currentAnimation, string idle, float fadeLength)
	{
		yield return new WaitForSeconds(currentAnimation.length - fadeLength);
		
		// If we haven't moved on to playing a different animation, pick an idle animation to play
		if (currentlyPlayingAnimation = currentAnimation)
		{
			PlaySingleIdleAnimation(idle, fadeLength);
		}
	}
	
	public IEnumerator PlayIdleAnimationWhenLoopFinished(AnimationClip currentAnimation, string idle, float fadeLength, int repeat)
	{
		yield return new WaitForSeconds((currentAnimation.length*repeat) - fadeLength);
		
		if(currentlyPlayingAnimation = currentAnimation)
		{
			PlaySingleIdleAnimation(idle, fadeLength);
		}
	}
	
	public IEnumerator PlayIdleAnimationWhenFinished(AnimationClip currentAnimation, float fadeLength)
	{
		yield return new WaitForSeconds(currentAnimation.length - fadeLength);
		
		// If we haven't moved on to playing a different animation, pick an idle animation to play
		if (currentlyPlayingAnimation = currentAnimation) {
			PlayIdleAnimation(fadeLength);
		}
	}
	
	protected void PlaySingleIdleAnimation(string idle, float fadeLength = 0.5f)
	{
		currentlyPlayingAnimation = gameObject.GetComponent<Animation>().GetClip(idle);
		gameObject.GetComponent<Animation>().CrossFade(idle, fadeLength, PlayMode.StopSameLayer);
		HeadLook.enabled = true;
	}
	
	protected void PlayIdleAnimation(float fadeLength = 0.5f)
	{
		okToIdle = true;
		if (idleAnimations.Count > 0) {
			currentlyPlayingAnimation = idleAnimations[Random.Range(0, idleAnimations.Count)];
			
			gameObject.GetComponent<Animation>().CrossFade(currentlyPlayingAnimation.name, fadeLength, PlayMode.StopAll);
			
			StartCoroutine(PlayIdleAnimationWhenFinished(currentlyPlayingAnimation, fadeLength));
		}
	}
	
	public void PlayLipSync(string animation, AudioClip audio = null)
	{
		if (!ContainsClipWithName(animation))
		{
			Debug.Log("Have no idea how to play that....: " + animation);
			
			return;//should do something about it?
		}
		
		if(audio != null) {
			Debug.Log ("I do have audio" );
		}
		
		if(ffx != null) {
			Debug.Log ("FFX gameObject is: "  + ffx.gameObject);
			ffx.PlayAnim(animation, audio);
		}else{
			Debug.Log ("FFX is NULL: " + gameObject.GetComponent<FaceFXControllerScript>());
		}
	}
	
	public void PlayLipSync(LipSyncInfo info)
	{
		if (ContainsClipWithName(info.AnimationName))
		{
			//InterruptAnimation();
			//Let ffx play it even if there is no lipsync.
			ffx.PlayAnim(info.AnimationName, info.Audio);
			//List<AnimationClip> idleAnimationsTemp = new List<AnimationClip>();
			//StartCoroutine(PlayIdleAnimationTesting(idleAnimationsTemp,0.25F));
		}
		else 
		{
			//TODO: do something with the phonemes so that the animation plays....
			AddDebugLine("No lip sync animation with name: " + info.AnimationName);
		}
	}
	
	#endregion
	
	public void InterruptAnimation(int layer = 1)
	{		
		gameObject.GetComponent<Animation>().Stop(currentlyPlayingAnimation.name);
	}

	public void ResumeIdleAnimations(int layer = 1)
	{
		List<AnimationClip> idleAnimationsTemp = new List<AnimationClip>();
		StartCoroutine(PlayIdleAnimationTesting(idleAnimationsTemp,0.25F));
	}
	
	public void InterruptLipSync()
	{
		// Stop the lip synching
		ffx.StopAnim();
		
		// TODO: Do we need to do anything to reset the lip position?
	}
	
	
	public IEnumerator PlayIdleAnimationTesting(List<AnimationClip> idleAnimationsTemp, float fadeLength = 0.5f)
	{
		//Debug.Log ("checkout flag: " + ItemsInCartGUI.checkout_animation_playing);
		if (idleAnimations.Count > 0) {

			if(!ItemsInCartGUI.checkout_animation_playing) {

				//Debug.Log("Size of temp idle animations for "+ transform.name + " is : " + idleAnimationsTemp.Count);
				// Shuffled deck approach to playing the animations
				if(idleAnimationsTemp.Count == 0){

					for(int i =0; i < idleAnimations.Count; i++){
						idleAnimationsTemp.Add(idleAnimations[i]);
					}
				}

				int random_number = Random.Range(0, idleAnimationsTemp.Count);
				currentlyPlayingIdleAnimation = idleAnimationsTemp[random_number];
				idleAnimationsTemp.RemoveAt(random_number);

				//Debug.Log("Currently playing idle animation : " + currentlyPlayingIdleAnimation.name);
				for(int i =0; i < idleAnimationsTemp.Count; i++){
					//Debug.Log (idleAnimationsTemp[i].name);
				}

				yield return new WaitForSeconds(currentlyPlayingIdleAnimation.length - fadeLength);
				
				gameObject.GetComponent<Animation>()[currentlyPlayingIdleAnimation.name].layer = 0;
				//gameObject.animation[currentlyPlayingIdleAnimation.name].wrapMode = WrapMode.Loop;
				
				gameObject.GetComponent<Animation>().CrossFade(currentlyPlayingIdleAnimation.name, fadeLength, PlayMode.StopSameLayer);
			}
			StartCoroutine(PlayIdleAnimationTesting(idleAnimationsTemp, fadeLength));
		}
	}

	public IEnumerator PlaySpeechAnimationTesting(float fadeLength = 0.5f)
	{
		if (talkingAnimations.Count > 0) {

			currentlyPlayingIdleAnimation = idleAnimations[Random.Range(0, idleAnimations.Count)];

			
			yield return new WaitForSeconds(currentlyPlayingIdleAnimation.length - fadeLength);
			
			gameObject.GetComponent<Animation>()[currentlyPlayingIdleAnimation.name].layer = 0;
			gameObject.GetComponent<Animation>()[currentlyPlayingIdleAnimation.name].wrapMode = WrapMode.Loop;
			
			gameObject.GetComponent<Animation>().CrossFade(currentlyPlayingIdleAnimation.name, fadeLength, PlayMode.StopSameLayer);
			
			//StartCoroutine(PlayIdleAnimationTesting(fadeLength));
		}
	}
	
	public void GetAnimationList() {
		AddDebugLine("GetAnimationList() and HasLoaded is: " + HasLoaded);
		
		StartCoroutine(GetAnimationList_Helper());
	}
	
	protected IEnumerator GetAnimationList_Helper() {
		
		while(!HasLoaded) {
			yield return new WaitForSeconds(2.0f);
		}
		
		AddDebugLine("The VH has loaded! Creating list");
		
		List<string> result = new List<string>();
		
		foreach(string anim in animationList) {
			if(!anim.StartsWith("Default") && !anim.StartsWith("facefx")){
				AnimationClip clip = gameObject.GetComponent<Animation>().GetClip(anim);
				
				result.Add(anim + "," + clip.length);
			}
		}
		
		Application.ExternalCall("ReturnAnimationList", result); 
		AddDebugLine("Sent response");
		
	}
}
