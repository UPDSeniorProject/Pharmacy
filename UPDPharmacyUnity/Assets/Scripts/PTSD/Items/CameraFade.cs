using UnityEngine;
using System;

public class CameraFade : MonoBehaviour
{   
	private static CameraFade mInstance = null;
	
	private static CameraFade instance
	{
		get
		{
			if( mInstance == null )
			{
				mInstance = GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;
				
				if( mInstance == null )
				{
					mInstance = new GameObject("CameraFade").AddComponent<CameraFade>();
				}
			}
			
			return mInstance;
		}
	}
	
	void Awake()
	{
		if( mInstance == null )
		{
			mInstance = this as CameraFade;
			instance.init();
		}
	}
	
	public GUIStyle m_BackgroundStyle = new GUIStyle();						// Style for background tiling
	public Texture2D m_FadeTexture;											// 1x1 pixel texture used for fading
	public Color m_CurrentScreenOverlayColor = new Color(0,0,0,0);			// default starting color: black and fully transparrent
	public Color m_TargetScreenOverlayColor = new Color(0,0,0,0);			// default target color: black and fully transparrent
	public Color m_DeltaColor = new Color(0,0,0,0);							// the delta-color is basically the "speed / second" at which the current color should change
	public int m_FadeGUIDepth = -1000;										// make sure this texture is drawn on top of everything
	
	public float m_FadeDelay = 0;
	public Action m_OnFadeFinish = null;
	
	public Vector2 m_HoleOrigin;
	public Vector2 m_HoleSize;
	
	public Vector2 m_SpritePosition;
	public int m_SpriteWidth;
	public int m_SpriteHeight;
	
	public int m_TaskWindowHeight = 0;

	public float alpha = 1.0f;

	// Initialize the texture, background-style and initial color:
	public void init()
	{		
		/*instance.m_FadeTexture = new Texture2D(1, 1);        
		instance.m_BackgroundStyle.normal.background = instance.m_FadeTexture;
		
		UISprite helpSprite = GameObject.Find("HelpSprite").GetComponent<UISprite>();
		instance.m_SpriteWidth = (int)(helpSprite.relativeSize.x * helpSprite.transform.localScale.x);
		instance.m_SpriteHeight = (int)(helpSprite.relativeSize.y * helpSprite.transform.localScale.y);
		//UICamera uic = GameObject.Find("Camera").GetComponent<UICamera>();
		instance.m_SpritePosition = Camera.main.WorldToScreenPoint (helpSprite.transform.position);
		
		Debug.Log (Camera.main.WorldToScreenPoint(helpSprite.transform.position));
		//instance.m_SpritePosition = GUIUtility.ScreenToGUIPoint (instance.m_SpritePosition);
		//Debug.Log (GUIUtility.ScreenToGUIPoint (instance.m_SpritePosition));
		/*Debug.Log (Camera.main.pixelWidth);
		Debug.Log (instance.m_SpritePosition);
		Ray ray = Camera.main.ScreenPointToRay(instance.m_SpritePosition);
		Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
		float distance;
		xy.Raycast(ray, out distance);
		Debug.Log ( ray.GetPoint(distance));
*/
	/*	UILabel tutorialLabel = GameObject.Find ("TasksLabel").GetComponent<UILabel> ();
		instance.m_TaskWindowHeight = (int)(tutorialLabel.relativeSize.y * tutorialLabel.transform.localScale.y);
		//Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds (tutorialLabel.transform);
		//Debug.Log (bounds.center + " " + bounds.size.x + " " + bounds.size.y);
		//float width = tutorialLabel.relativeSize.x * tutorialLabel.transform.localScale.x;
		//float height = tutorialLabel.relativeSize.y * tutorialLabel.transform.localScale.y;
		//Debug.Log (width + " " + height);
		
		Vector2 temphole = new Vector2 (10, 10);
		Vector2 tempholesize = new Vector2 (20, 20);
		instance.m_HoleOrigin = temphole;
		instance.m_HoleSize = tempholesize;
		Debug.Log (instance.m_SpriteWidth + " " + instance.m_SpriteHeight + " " + instance.m_SpritePosition);
		//want 280, 365
		//Debug.Log (instance.m_SpritePosition.x - 100);
		//Debug.Log (instance.m_SpriteWidth + 25);
		//Debug.Log (instance.m_HoleOrigin.x + " " + instance.m_HoleSize.x + " " + Screen.width);*/
	}

	public static void startFade()
	{

	}

	// Draw the texture and perform the fade:
	void OnGUI()
	{   
		UISprite helpSprite= GameObject.Find("HelpSpriteOverlay").GetComponent<UISprite>();
		Color c = new Color (helpSprite.color.r, helpSprite.color.g, helpSprite.color.b, instance.alpha);
		helpSprite.color = c;
		instance.alpha -= .1f;
		Debug.Log ("here" + instance.alpha);
		if (instance.alpha <= 0)
						Die ();
		// If delay is over...
		/*if( Time.time > instance.m_FadeDelay )
		{
			// If the current color of the screen is not equal to the desired color: keep fading!
			if (instance.m_CurrentScreenOverlayColor != instance.m_TargetScreenOverlayColor)
			{			
				// If the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
				if (Mathf.Abs(instance.m_CurrentScreenOverlayColor.a - instance.m_TargetScreenOverlayColor.a) < Mathf.Abs(instance.m_DeltaColor.a) * Time.deltaTime)
				{
					instance.m_CurrentScreenOverlayColor = instance.m_TargetScreenOverlayColor;
					SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor);
					instance.m_DeltaColor = new Color( 0,0,0,0 );
					
					if( instance.m_OnFadeFinish != null )
						instance.m_OnFadeFinish();
					
					//Die();
				}
				else
				{
					// Fade!
					SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor + instance.m_DeltaColor * Time.deltaTime);
				}
			}
		}
		// Only draw the texture when the alpha value is greater than 0:
		if (m_CurrentScreenOverlayColor.a > 0)
		{			
			GUI.depth = instance.m_FadeGUIDepth;
			GUI.Label(new Rect((int)instance.m_SpritePosition.x, (int)instance.m_SpritePosition.y, 100,20), instance.m_FadeTexture, instance.m_BackgroundStyle);
			//GUI.Label(new Rect(-10, -10, instance.m_SpritePosition.x - 75, Screen.height + 10), instance.m_FadeTexture, instance.m_BackgroundStyle);
			//GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), instance.m_FadeTexture, instance.m_BackgroundStyle);
			//GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), instance.m_FadeTexture, instance.m_BackgroundStyle);
			//GUI.Label(new Rect(instance.m_SpritePosition.x + instance.m_SpriteWidth - 75, -10, Screen.width + 10 - (instance.m_SpritePosition.x + instance.m_SpriteWidth), Screen.height + 10), instance.m_FadeTexture, instance.m_BackgroundStyle);
			//GUI.Label(new Rect(100, 100, 100, 100), instance.m_FadeTexture, instance.m_BackgroundStyle);
		}*/
	}
	
	
	/// <summary>
	/// Sets the color of the screen overlay instantly.  Useful to start a fade.
	/// </summary>
	/// <param name='newScreenOverlayColor'>
	/// New screen overlay color.
	/// </param>
	private static void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		instance.m_CurrentScreenOverlayColor = newScreenOverlayColor;
		instance.m_FadeTexture.SetPixel(0, 0, instance.m_CurrentScreenOverlayColor);
		instance.m_FadeTexture.Apply();
	}
	
	/// <summary>
	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent.
	/// </summary>
	/// <param name='newScreenOverlayColor'>
	/// Target screen overlay Color.
	/// </param>
	/// <param name='fadeDuration'>
	/// Fade duration.
	/// </param>
	/*public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration )
	{
		if (fadeDuration <= 0.0f)		
		{
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else					
		{
			if( isFadeIn )
			{
				instance.m_TargetScreenOverlayColor = new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 );
				SetScreenOverlayColor( newScreenOverlayColor );
			} else {
				instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
				SetScreenOverlayColor( new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 ) );
			}
			
			instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;	
		}
	}*/
	
	/// <summary>
	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay.
	/// </summary>
	/// <param name='newScreenOverlayColor'>
	/// New screen overlay color.
	/// </param>
	/// <param name='fadeDuration'>
	/// Fade duration.
	/// </param>
	/// <param name='fadeDelay'>
	/// Fade delay.
	/// </param>
	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay )
	{
		//		instance.m_Origin = origin;
		//		instance.m_OriginSize = originSize;
		if (fadeDuration <= 0.0f)		
		{
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else					
		{
			instance.m_FadeDelay = Time.time + fadeDelay;			
			
			if( isFadeIn )
			{
				instance.m_TargetScreenOverlayColor = new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 );
				SetScreenOverlayColor( newScreenOverlayColor );
			} else {
				instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
				SetScreenOverlayColor( new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 ) );
			}
			
			instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
		}
	}
	
	/// <summary>
	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay, with Action OnFadeFinish.
	/// </summary>
	/// <param name='newScreenOverlayColor'>
	/// New screen overlay color.
	/// </param>
	/// <param name='fadeDuration'>
	/// Fade duration.
	/// </param>
	/// <param name='fadeDelay'>
	/// Fade delay.
	/// </param>
	/// <param name='OnFadeFinish'>
	/// On fade finish, doWork().
	/// </param>
	/*	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay, Action OnFadeFinish )
	{
		if (fadeDuration <= 0.0f)		
		{
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else					
		{
			instance.m_OnFadeFinish = OnFadeFinish;
			instance.m_FadeDelay = Time.time + fadeDelay;
			
			if( isFadeIn )
			{
				instance.m_TargetScreenOverlayColor = new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 );
				SetScreenOverlayColor( newScreenOverlayColor );
			} else {
				instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
				SetScreenOverlayColor( new Color( newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0 ) );
			}
			instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
		}
	}*/
	
	void Die()
	{
		mInstance = null;
		Destroy(gameObject);
	}
	
	void OnApplicationQuit()
	{
		mInstance = null;
	}
}

/*Shader "Hole Thing" {
	
	Properties {
		_Center ("Hole Center", Vector) = (.5, .5, 0 , 0)
			_Radius ("Hole Radius", Float) = .25
				_Shape ("Hole Shape", Float) = .25
				_MainTex ("Main Texture", 2D) = ""
	}
	
	SubShader {
		Tags {"Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			struct appdata {
				float4 position : POSITION;
				half2 texCoord : TEXCOORD;
			}; 
			
			struct v2f {
				float4 position_clip : SV_POSITION;
				half2 position_uv : TEXCOORD;
			};
			
			#pragma vertex vert
			uniform half4 _MainTex_ST;
			v2f vert(appdata i) {
				v2f o;
				o.position_clip = mul(UNITY_MATRIX_MVP, i.position);
				o.position_uv = _MainTex_ST.xy * i.texCoord + _MainTex_ST.zw;
				return o;
			}
			
			#pragma fragment frag
			uniform sampler2D _MainTex;
			uniform half2 _Center;
			half _Radius, _Shape;
			fixed4 frag(v2f i) : COLOR {        
				fixed4 fragColor = tex2D(_MainTex, i.position_uv);
				half hole = min(distance(i.position_uv, _Center) / _Radius, 1.);
				fragColor.a *= pow(hole, _Shape);
				return fragColor;
			}
			ENDCG
		}
	}*/
	
