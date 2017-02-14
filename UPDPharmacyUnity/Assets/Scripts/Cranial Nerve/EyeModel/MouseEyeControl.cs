using System.Collections;
using System;
using UnityEngine;

public class MouseEyeControl : RenBehaviour
{

    #region Transforms
	public UnityEngine.Transform nerves;
	
	public UnityEngine.Transform eyelidNerve;
	public UnityEngine.Transform lEyelidNerve;
    #endregion
	
	public EyeSide eyeSide;

    public Vector2 ScreenPosition;

    protected GameObject ObjectToFollow = null;
    protected Camera MainCamera;

    public float topY = 6.0f;
    public float midY = 2.0f; 
    public float bottomY = -6.0f;

    public float rightX = -22.5f;
    public float leftX = 22.5f;

    bool HasTriggered = false;
    protected bool[] sections = new bool[6];

    public event ExtraoccularEventDelegate ExtraoccularEvent;
    


    #region Constants and Matrices
	// system constants: #these parameters from Raphan 1998
	private const double B = 0.0000747;	// plant viscosity
	private const double K = 0.0004762;	// plant stiffness
	private const double S = 0.00000249;	// tension-innervation ratio	???
	private const double J = 0.0000005;	// moment of inertia of the eye
	private Matrix Ins0;
	private Matrix P;

    #endregion


    #region MonoBehaviour
    protected override void Start ()
	{
		base.Start();
        //Get a reference to the Main Camera;
        MainCamera = Camera.main;
        
        for(int i = 0;i<sections.Length; i++)
            sections[i] = false;

        switch(eyeSide) {
            case EyeSide.Right:
                // muscle insertion points on eye when eye in primary orientation
			    //Lateral	%data from Miller & Robinson 1984
			    //anterior
			    Ins0 = new Matrix (new double[3, 6] { { -10.08, 9.65, 0, 0, -2.90, -8.70 }, { 6.50, 8.84, 7.63, 8.02, -4.41, -7.18 }, { 0, 0, 10.48, -10.24, 11.05, 0 } });
		    	//superior
	    		// pulley positions		%fantasy positions but they do correspond to positions that would yield Listing's law
    			P = new Matrix (new double[3, 6] { { -13, 13, 0, 0, 15.27, 11.10 }, { -8.3829, -11.909, -9.4647, -10.182, 8.24, 11.34 }, { 0, 0, 13, -13, 12.25, -15.46 } });
                break;
            case EyeSide.Left:
                // muscle insertion points on eye when eye in primary orientation
			    //Lateral	%data from Miller & Robinson 1984
			    //anterior
			    Ins0 = new Matrix (new double[3, 6] { { 10.08, -9.65, 0, 0, 2.90, 8.70 }, { 6.50, 8.84, 7.63, 8.02, -4.41, -7.18 }, { 0, 0, 10.48, -10.24, 11.05, 0 } });
			    //superior
			    // pulley positions		%fantasy positions but they do correspond to positions that would yield Listing's law
			    P = new Matrix (new double[3, 6] { { 13, -13, 0, 0, -15.27, -11.10 }, { -8.3829, -11.909, -9.4647, -10.182, 8.24, 11.34 }, { 0, 0, 13, -13, 12.25, -15.46 } });
                break;
            default:
                AddDebugLine("Please set a Eye Side for your MouseEyeControl.");
                break;
        }
		
	}

	// Update is called once per frame
	protected override void Update ()
	{
		if(isPaused) return;
		base.Update();
		
		
		// figure out the angle from this eye to the mouse cursor

		if(ObjectToFollow != null && !UseFixedScreenPosition){ // Check if we are following an object.

			/*mouseX = (float)((((UnityEngine.Input.mousePosition.x / UnityEngine.Screen.width) - 0.5) * (UnityEngine.Screen.width / (double)UnityEngine.Screen.height)) * -40.0);
			mouseY = (float)(((UnityEngine.Input.mousePosition.y / UnityEngine.Screen.height) - 0.5) * 24.0);*/

            ScreenPosition.x = (float)((((MainCamera.WorldToScreenPoint(ObjectToFollow.transform.position).x / UnityEngine.Screen.width) - 0.5) * (UnityEngine.Screen.width / (double)UnityEngine.Screen.height)) * -40.0);
            ScreenPosition.y = (float)(((MainCamera.WorldToScreenPoint(ObjectToFollow.transform.position).y / UnityEngine.Screen.height) - 0.5) * 24.0);
            //Debug.Log(ScreenPosition);
            UpdateSections();
		}

        

        BlackBoxMethodToUpdateEyeOrientation();	
	}

    #endregion

    protected void UpdateSections()
    {
        if (!HasTriggered)
        {
            if (ScreenPosition.x < rightX)
            {
                if (ScreenPosition.y > topY)
                {
                    sections[0] = true;
                }
                else if (ScreenPosition.y < midY && ScreenPosition.y > -midY)
                {
                    sections[1] = true;
                }
                else if (ScreenPosition.y < bottomY)
                {
                    sections[2] = true;
                }

            }
            else if (ScreenPosition.x > leftX)
            {
                if (ScreenPosition.y > topY)
                {
                    sections[3] = true;
                }
                else if (ScreenPosition.y < midY && ScreenPosition.y > -midY)
                {
                    sections[4] = true;
                }
                else if (ScreenPosition.y < bottomY)
                {
                    sections[5] = true;
                }
            }



            for (int i = 0; i < sections.Length; i++)
            {
                if (!sections[i])
                    return;
            }
            HasTriggered = true;
            OnExtraoccularEvent(new ExtraocularEventArgs());

        }
    }


    bool UseFixedScreenPosition = false;

    public void FollowObject(GameObject obj)
    {
        ObjectToFollow = obj;
        UseFixedScreenPosition = false;
    }

    public void SetFixedScreenPosition(float x, float y)
    {
        UseFixedScreenPosition = true;
        ScreenPosition.x = x;
        ScreenPosition.y = y;
        ObjectToFollow = null;
    }

    public void StopFollowingObject()
    {
        ObjectToFollow = null;
        UseFixedScreenPosition = false;
        ScreenPosition = Vector2.zero;
    }

    /// <summary>
    /// Stops following the objected passed as a parameter.
    /// </summary>
    /// <param name="obj"></param>
    public void StopFollowingObject(GameObject obj)
    {
        if (ObjectToFollow == obj)
            StopFollowingObject();
    }

    public bool IsFollowingObject()
    {
        return ObjectToFollow != null;
    }

    protected void OnExtraoccularEvent(ExtraocularEventArgs args)
    {
        if (ExtraoccularEvent != null)
        {
            ExtraoccularEvent(this, args);
        }
        else
        {
            Debug.Log("No handler");
        }
    }

    #region Matrix Operations for update
    /// <summary>
    /// DJRG: In the original code for NERVE (K-NERVE and Guangyan's version) this was part of the Update function. 
    /// To be quite honest I'm not sure how this works. I extracted this as a method to make the class easier to read. 
    /// There is no other particular reason for doing this.
    /// </summary>
    private void BlackBoxMethodToUpdateEyeOrientation()
    {
        Matrix R = new Matrix(new double[,] { { ScreenPosition.y * Math.PI / 180.0 }, { 0 }, { ScreenPosition.x * Math.PI / 180.0 } });

        double R_norm = R.Norm2();

        Matrix r = Math.Tan(R_norm / 2) * R;

        if (R_norm != 0)
            r /= R_norm;

        Matrix cocon = S * new Matrix(new double[,] { { 45, 45, 45, 45, 45, 45 } });

        Matrix Mstr = new Matrix(new double[,] { { 1, 1, 0.9, 0.9, 0.4, 0.4 } });

        R = r;

        Matrix MRV = new Matrix(3, 6);
        Matrix Ins = new Matrix(3, 6);

        R_norm = R.Norm2();
        if (R_norm == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                MRV.SetColumn(i, Ins0.Col(i).CrossProduct(P.Col(i)));
                Matrix MRVCol = MRV.Col(i);
                MRV.SetColumn(i, MRVCol / MRVCol.Norm2());
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                Ins.SetColumn(i, m_insert3(R, Ins0.Col(i)));
                MRV.SetColumn(i, Ins.Col(i).CrossProduct(P.Col(i)));
                Matrix MRVCol = MRV.Col(i);
                MRV.SetColumn(i, MRVCol / MRVCol.Norm2());
            }
        }

        //Matrix rel_contrr = MRV.Transpose() * R;	// relative contribution
        Matrix F;
        if (R_norm == 0)
        {
            F = new Matrix(3, 1);
        }
        else
        {
            F = K * R / R_norm * 2 * Math.Atan(R_norm);
        }

        Matrix MstrDiag = new Matrix(6, 6);
        for (int i = 0; i < 6; i++)
        {
            MstrDiag[i, i] = Mstr[0, i];
        }

        MRV = MRV * MstrDiag; // <------- addition for making muscles of different strength



        Matrix M1 = MRV.Col(0);
        Matrix M2 = MRV.Col(1);
        Matrix M3 = MRV.Col(2);
        Matrix M4 = MRV.Col(3);
        Matrix M5 = MRV.Col(4);
        Matrix M6 = MRV.Col(5);
        double h1 = (F[0, 0] - cocon[0, 2] * (M3[0, 0] + M4[0, 0]) - cocon[0, 4] * (M5[0, 0] + M6[0, 0]) - cocon[0, 0] * (M1[0, 0] + M2[0, 0])) / (M3[0, 0] - M4[0, 0]);
        double h2 = (F[1, 0] - cocon[0, 2] * (M3[1, 0] + M4[1, 0]) - cocon[0, 4] * (M5[1, 0] + M6[1, 0]) - cocon[0, 0] * (M1[1, 0] + M2[1, 0])) / (M5[1, 0] - M6[1, 0]);
        double h3 = (F[2, 0] - cocon[0, 2] * (M3[2, 0] + M4[2, 0]) - cocon[0, 4] * (M5[2, 0] + M6[2, 0]) - cocon[0, 0] * (M1[2, 0] + M2[2, 0])) / (M1[2, 0] - M2[2, 0]);
        double M12a = M1[0, 0] - M2[0, 0];
        double M12b = M1[1, 0] - M2[1, 0];
        double M12c = M1[2, 0] - M2[2, 0];
        double M34a = M3[0, 0] - M4[0, 0];
        double M34b = M3[1, 0] - M4[1, 0];
        double M34c = M3[2, 0] - M4[2, 0];
        double M56a = M5[0, 0] - M6[0, 0];
        double M56b = M5[1, 0] - M6[1, 0];
        double M56c = M5[2, 0] - M6[2, 0];
        double H = M34c * M56b * M12a - M34c * M56a * M12b - M56b * M34a * M12c + M56a * M34b * M12c - M34b * M56c * M12a + M34a * M56c * M12b;
        double F12 = -(-M34c * M56b * h1 * M34a - M56a * M34b * h3 * M12c + M34c * M56a * h2 * M56b + M34b * M56c * h1 * M34a + M56b * M34a * h3 * M12c - M34a * M56c * h2 * M56b) / H;
        double F56 = -(-M34b * h3 * M12c * M12a + M34b * M12c * h1 * M34a - M34c * M12b * h1 * M34a + M34c * h2 * M56b * M12a - M12c * M34a * h2 * M56b + h3 * M12c * M34a * M12b) / H;
        double F34 = -(M12c * M56b * h1 * M34a - M12c * M56a * h2 * M56b - h3 * M12c * M56b * M12a + h3 * M12c * M56a * M12b - M56c * M12b * h1 * M34a + M56c * h2 * M56b * M12a) / H;
        float[] I = new float[6];
        I[0] = (15 + (float)((cocon[0, 0] + F12) / S)) / 115.0f;
        I[1] = (15 + (float)((cocon[0, 1] - F12) / S)) / 115.0f;
        I[2] = (15 + (float)((cocon[0, 2] + F34) / S)) / 115.0f;
        I[3] = (15 + (float)((cocon[0, 3] - F34) / S)) / 115.0f;
        I[4] = (15 + (float)((cocon[0, 4] + F56) / S)) / 115.0f;
        I[5] = (15 + (float)((cocon[0, 5] - F56) / S)) / 115.0f;

        /*		I[0] =  (float)((cocon[0,0]+F12)/S);
                I[1] =  (float)((cocon[0,1]-F12)/S);
                I[2] =  (float)((cocon[0,2]+F34)/S);
                I[3] =  (float)((cocon[0,3]-F34)/S);
                I[4] =  (float)((cocon[0,4]+F56)/S);
                I[5] =  (float)((cocon[0,5]-F56)/S);*/


        nerves.localPosition = new UnityEngine.Vector3(I[0], I[1], I[2]);
        nerves.localEulerAngles = new UnityEngine.Vector3(I[3], I[4], I[5]);

        eyelidNerve.localPosition = new UnityEngine.Vector3(Math.Min(1.0f, (ScreenPosition.y + 45) / 48.0f), 0, 0);
        lEyelidNerve.localPosition = new UnityEngine.Vector3(Math.Min(1.0f, (ScreenPosition.y + 45) / 48.0f), 0, 0);
    }
	
		
	// stolen from Ansgar R. Koene https://sites.google.com/site/arkoene/m_insert3.m
	// this function determines the spatial location of a point after rotation about an axis that goes through the origin.
	// R and Lo both need to be 3x1 matrices, returns a 3x1 matrix
	protected Matrix m_insert3(Matrix R, Matrix Lo) {
		double R_Norm = R.Norm2();
		Matrix SR = R * ( R.DotProduct(Lo) ) / Math.Pow( R_Norm, 2);
		Matrix PR = Lo - SR;
		
		double theta = 2*Math.Atan(R_Norm);
		
		if (PR.Equals(new Matrix(3, 1))) {
			return SR;
		} else {
			return 2 * (SR + Math.Pow((Math.Cos(theta/2)), 2)*(PR + R.CrossProduct(PR))) - Lo;
		}

    }
    #endregion
}

public class ExtraocularEventArgs : System.EventArgs
{ 
}


public delegate void ExtraoccularEventDelegate(MouseEyeControl ctrl, ExtraocularEventArgs args);