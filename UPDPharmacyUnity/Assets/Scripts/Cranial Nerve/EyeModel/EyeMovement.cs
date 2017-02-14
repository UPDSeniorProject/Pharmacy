using System.Collections;
using System;

public class EyeMovement : UnityEngine.MonoBehaviour {
	
	public UnityEngine.Transform innervations;
	
	public EyeSide eyeSide;
	public UnityEngine.Vector3 hpr;
	
	public UnityEngine.Matrix4x4 modelAdjustment;
	
	// system constants: #these parameters from Raphan 1998
	private const double B = 0.0000747;	// plant viscosity
	private const double K = 0.0004762;	// plant stiffness
	private const double S = 0.00000249;	// tension-innervation ratio	???
	private const double J = 0.0000005;	// moment of inertia of the eye
	
	private Matrix Ins0;
	private Matrix P;
	private Matrix w;
	private Matrix R;
	private Matrix I;
	private double timeBank;
	
	void Start () {
		
        switch(eyeSide) 
		{
		case EyeSide.Right:
            // muscle insertion points on eye when eye in primary orientation
            Ins0 = new Matrix(new double[3, 6] {{-10.08, 9.65, 0,     0,    -2.90, -8.70},	 //Lateral	%data from Miller & Robinson 1984
												{ 6.50,  8.84, 7.63,  8.02, -4.41, -7.18},	//anterior
				 								{ 0,     0,   10.48,-10.24, 11.05 , 0   }});	//superior

            // pulley positions		%fantasy positions but they do correspond to positions that would yield Listing's law
            P = new Matrix(new double[3, 6]{{-13,     13,      0,        0,    15.27,   11.10},
											{-8.3829,-11.909, -9.4647, -10.182, 8.24,   11.34},
											{ 0,       0,     13,      -13,    12.25,  -15.46}});
 
			break;
		case EyeSide.Left:
		default:
            // muscle insertion points on eye when eye in primary orientation
            Ins0 = new Matrix(new double[3, 6] {{10.08, -9.65, 0,     0,     2.90,  8.70},	 //Lateral	%data from Miller & Robinson 1984
												{ 6.50,  8.84, 7.63,  8.02, -4.41, -7.18},	//anterior
				 								{ 0,     0,   10.48,-10.24, 11.05 , 0   }});	//superior

            // pulley positions		%fantasy positions but they do correspond to positions that would yield Listing's law
            P = new Matrix(new double[3, 6]{{ 13,    -13,      0,        0,    -15.27,   -11.10},
											{-8.3829,-11.909, -9.4647, -10.182,  8.24,    11.34},
											{ 0,       0,     13,      -13,     12.25,   -15.46}});	
			break;
		}

		
		
        w = new Matrix(3,1); // starting angular velocity of the eye
        R = new Matrix(3,1); // Orientation vector of the eye (not a rotation vector)
        I = new Matrix(6,1); // nerve inervations

        hpr = new UnityEngine.Vector3();
        timeBank = 0.0;

	}
	
	// # stolen from Ansgar R. Koene https://sites.google.com/site/arkoene/real_muscles_paired.m (the paper is "Properties of 3D rotations and their relation to eye movement control")

	void Update () {
		/*Matrix R = new Matrix(3, 1);
		Matrix Lo = new Matrix(3, 1);
		
		R[0,0] = 0.10387813; R[1,0] = -0.0041644; R[2,0] = -0.08527771;
		
//		Lo[0,0] = 10.08; Lo[1,0] = 6.5; Lo[2,0] = 0;
		Lo[0,0] = 0; Lo[1,0] = 7.63; Lo[2,0] = 10.48;
		
//		UnityEngine.Debug.Log(m_insert3(R,Lo).ToString());
		 */
		
		// get latest innervations
//		CranialNerveModel otherScript = (CranialNerveModel)UnityEngine.GameObject.Find("Model").GetComponent("CranialNerveModel"); 
		float [] inn = new float[6];
		
		inn[0] = innervations.localPosition.x;
		inn[1] = innervations.localPosition.y;
		inn[2] = innervations.localPosition.z;
		inn[3] = innervations.localEulerAngles.x;
		inn[4] = innervations.localEulerAngles.y;
		inn[5] = innervations.localEulerAngles.z;
		
		
		for ( int i = 0; i < inn.Length; i ++ ) {
			I[i,0] = (inn[i] * 115.0) - 15;
		}
		
				 
		
		double dt = UnityEngine.Time.deltaTime;
		
		if (dt == 0)
			return;
		
		if (dt > 1)
			dt = 1;
		
		double deltaT = 0.0265; // this seems to be the maximum step size this code can handle before w goes to infinity
		Matrix rrot = new Matrix(3,1);
		
		while (dt > 0) {
			if (deltaT > dt)
				deltaT = dt;
			
			dt -= deltaT;
			
			timeBank += deltaT;
			if (timeBank >= 0.02) {
				timeBank -= 0.02;
				w = new Matrix(3,1);
			}
			
			double R_norm = R.Norm2();
			
			Matrix n;
			if (R_norm == 0)
				n = R;
			else
				n = R / R_norm;
			
			double phi = R_norm;
			
			Matrix r = n *Math.Tan(phi / 2);
			
			// Torque exerted by the plant (passive tissue stuff)
            Matrix Tp = B*w + K*phi*n;
			
			Matrix UMV = new Matrix(3,6);
			Matrix Ins = new Matrix(3,6);
			
			// UMV of the muscle pairs
            if (r.Norm2() == 0) {
                for (int i = 0; i < 6; i ++) {
                    UMV.SetColumn(i, Ins0.Col(i).CrossProduct(P.Col(i)));
					Matrix UMVCol = UMV.Col(i);
                    UMV.SetColumn(i, UMVCol / UMVCol.Norm2());
				}
			} else {
                for (int i = 0; i < 6; i ++) {
                	Ins.SetColumn(i, m_insert3(r, Ins0.Col(i)));
                    UMV.SetColumn(i, Ins.Col(i).CrossProduct(P.Col(i)));
					Matrix UMVCol = UMV.Col(i);
                    UMV.SetColumn(i, UMVCol / UMVCol.Norm2());
				}
			}
			
			// Torque generated by the muscles Tm
            Matrix Tm = S*UMV*I;
           
            // Total torque acting on the eye
            Matrix Tt = Tm - Tp;
    
            // new orietation of the eye
            Matrix dwdt = Tt/J;
            Matrix w_inc = dwdt*deltaT;
            
            w = w+w_inc;
    
            double w_norm = w.Norm2();
            
			Matrix n_inc;
            if (w_norm == 0) {
                    n_inc = w;
			} else {
                    n_inc = w/w_norm;
			}
            
            double p_inc = w_norm*deltaT;
            
            Matrix R_inc = n_inc*Math.Tan(p_inc/2);
            
            r = (R_inc + r + R_inc.CrossProduct(r)) * (1-r.Transpose()*R_inc).Inverse();
            
            double r_norm = r.Norm2();
            if (r_norm == 0) {
                    n = r;
			} else {
                    n = r/r_norm;
			}
            phi = 2*Math.Atan(r_norm);


            if (n.AnyNaN()){
                n = new Matrix(3,1);
                phi = 0;
			}
                        
            R=phi*n;
    
            rrot = Math.Tan(phi/2)*n;

		}
		
		double degree = 180.0 / Math.PI;

		hpr = new UnityEngine.Vector3 ((float)(rrot[0,0]*degree), (float)(rrot[1,0]*degree), (float)(rrot[2,0]*degree));
		
		hpr = modelAdjustment.MultiplyPoint(hpr);
		
		transform.localRotation = UnityEngine.Quaternion.Euler(hpr);
		
		
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
}
