using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(CranialNerveModel))]
public class CranialNerveModelGUI : Editor {
	
	public CranialNerveModel _target;
	public GameObject CNModel;
    public GameObject VirtualHuman;
	
	
	void OnEnable()    
    {
        _target = (CranialNerveModel)target;
		CNModel = _target.gameObject;		
    }
	
	public GameObject FindInChildren(GameObject root, string pName) 
	{
		Transform pTransform = root.GetComponent<Transform>();
		
    	foreach (Transform trs in pTransform) {
        	if (trs.gameObject.name == pName){ 
				Debug.LogWarning("FOUND IT!!: " + trs.gameObject.name +  " is: " + pName);
            	return trs.gameObject;
			}	else 
			{
				GameObject o = FindInChildren(trs.gameObject, pName);
				if( o != null) { 
					return o;	
				}
			}				
		}
		return null;
    }

	
	public override void OnInspectorGUI () 
    {
		base.OnInspectorGUI();
		
		if(GUILayout.Button("Setup Model"))
		{
			
			VirtualHuman = CNModel.transform.parent.gameObject;
			if(!VirtualHuman.name.Equals("VirtualHuman"))
			{ //Display warning for diferently named VH
				if(!EditorUtility.DisplayDialog("Are you sure you want to continue?",
					"The parent of the CranialNerveModel is not set to the default 'VirtualHuman', but to: '" + VirtualHuman.name + "' Are you sure you want to continue? This might not work.",
					"Continue","Cancel")) {
					return;	
				}
			}
			
			Debug.Log("Model setup!");	
			GameObject LeftEye = FindInChildren(VirtualHuman, "LeftEye");
			GameObject RightEye = FindInChildren(VirtualHuman, "RightEye");
			
			//TODO: FIX  (Set's up EyeClick )
			EyeClick lEyeClick = LeftEye.GetComponent<EyeClick>();
			if(lEyeClick == null) 
				lEyeClick = LeftEye.AddComponent<EyeClick>();
			
			EyeClick rEyeClick = RightEye.GetComponent<EyeClick>();
			if(rEyeClick == null)
				rEyeClick = RightEye.AddComponent<EyeClick>();
			
			lEyeClick.eyelidOutNerve = FindInChildren(CNModel, "lEyelidMovement").transform;
			rEyeClick.eyelidOutNerve = FindInChildren(CNModel, "rEyelidMovement").transform;
			
			lEyeClick.eyelidNode = FindInChildren(VirtualHuman, "UpperLidL").transform;
			rEyeClick.eyelidNode = FindInChildren(VirtualHuman, "UpperLidR").transform;
			//=========================
			//Set up Eye movement;
			EyeMovement lEyeMovement = LeftEye.GetComponent<EyeMovement>();
			if(lEyeMovement == null)
				lEyeMovement = LeftEye.AddComponent<EyeMovement>();
			
			EyeMovement rEyeMovement = RightEye.GetComponent<EyeMovement>();
			if(rEyeMovement == null)
				rEyeMovement = RightEye.AddComponent<EyeMovement>();
			
			lEyeMovement.innervations = FindInChildren(CNModel, "leyemove").transform;
			rEyeMovement.innervations = FindInChildren(CNModel, "reyemove").transform;
			
			lEyeMovement.eyeSide = EyeSide.Left;
			rEyeMovement.eyeSide = EyeSide.Right;
			
			//Magic matrices to control eyes
			lEyeMovement.modelAdjustment = new Matrix4x4();
			lEyeMovement.modelAdjustment.SetRow(0,new Vector4(0,1,0,0));
			lEyeMovement.modelAdjustment.SetRow(1, new Vector4(1,1,0,-90));
			lEyeMovement.modelAdjustment.SetRow(2, new Vector4(0,0,1,0));
			lEyeMovement.modelAdjustment.SetRow(3, new Vector4(0,0,0,1));
			
			rEyeMovement.modelAdjustment = new Matrix4x4();
			rEyeMovement.modelAdjustment.SetRow(0, new Vector4(0,1,0,0));
			rEyeMovement.modelAdjustment.SetRow(1, new Vector4(1,0,0,-90));
			rEyeMovement.modelAdjustment.SetRow(2, new Vector4(0,0,1,0));
			rEyeMovement.modelAdjustment.SetRow(3, new Vector4(0,0,0,1));

            GameObject LeftEyelid = FindInChildren(VirtualHuman, "UpperLidL");
            GameObject RightEyelid = FindInChildren(VirtualHuman, "UpperLidR");
            if (LeftEyelid == null || RightEyelid == null)
            {
                if (EditorUtility.DisplayDialog("Eyelid not found", "UpperLidL = " + LeftEyelid + " and UpperLidR = " + RightEyelid + " which could lead to errors. Process will not finish correctly.", "Continue", "Cancel"))
                {
                    return;
                }
            }
            else
            {
                EyelidControl LeftLidControl = LeftEyelid.GetComponent<EyelidControl>();
                if (LeftLidControl == null)
                    LeftLidControl = LeftEyelid.AddComponent<EyelidControl>();
                EyelidControl RightLidControl = RightEyelid.GetComponent<EyelidControl>();
                if (RightLidControl == null)
                    RightLidControl = RightEyelid.AddComponent<EyelidControl>();

                LeftLidControl.eyelidValue = FindInChildren(CNModel, "lEyelidMovement").transform;
                RightLidControl.eyelidValue = FindInChildren(CNModel, "rEyelidMovement").transform;

                LeftLidControl.useRotation = RightLidControl.useRotation = false;
                LeftLidControl.negate = RightLidControl.negate = true;

                LeftLidControl.upperRange = RightLidControl.upperRange = 6.0f;
                LeftLidControl.lowerRange = RightLidControl.lowerRange = 5.0f;
                LeftLidControl.initialPos = RightLidControl.initialPos = 0.0f;
            }

			
			//Find pupils
			GameObject LeftPupil = FindInChildren(LeftEye, "LeftEye_Pupil");
			GameObject RightPupil = FindInChildren(RightEye, "RightEye_Pupil");
			
			if(LeftPupil == null || RightPupil == null) 
			{
				if(EditorUtility.DisplayDialog("Pupils not set", "LeftPupil = " + LeftPupil + " and RightPupil = " + RightPupil + " which could lead to errors. Process will not finish correctly.", "Continue", "Cancel")) {
					return;
				}
			}else {
				PupilControl LeftPupilControl = LeftPupil.GetComponent<PupilControl>();
				if(LeftPupilControl == null)
					LeftPupilControl = LeftPupil.AddComponent<PupilControl>();
				
				PupilControl RightPupilControl = RightPupil.GetComponent<PupilControl>();
				if(RightPupilControl == null)
					RightPupilControl = RightPupil.AddComponent<PupilControl>();
				
				LeftPupilControl.pupilValue = FindInChildren(CNModel, "lPupil").transform;
				RightPupilControl.pupilValue = FindInChildren(CNModel, "rPupil").transform;
				
				LeftPupilControl.upperRange 	  =	RightPupilControl.upperRange 		= 2.0f;
				LeftPupilControl.lowerRange       =	RightPupilControl.lowerRange 		= 0.5f;
				LeftPupilControl.dilateSpeed      =	RightPupilControl.dilateSpeed 		= 1.66f;
				LeftPupilControl.contractSpeed    =	RightPupilControl.contractSpeed 	= 5.0f;
				LeftPupilControl.movementExponent = RightPupilControl.movementExponent 	= 1.5f;
				LeftPupilControl.target 		  = RightPupilControl.target 			= 0.0f;
				LeftPupilControl.inTarget 		  = RightPupilControl.inTarget 			= 0.0f;
			}
			
			

				
		}
		
		
	}

}
