using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

	private static List<Item> _cartItems = new List<Item>();
	public static List<Item> CartItems{
		get { return _cartItems; }
	}
	
	/*private static int _twentyDollar;
	public static int TwentyDollar{
		get { return Random(0,3);}
		set {	_twentyDollar = value; }
	}
	private static int _tenDollar;
	public static int TenDollar{
		get { return Random(0,3);}
		set {	_tenDollar = value; }
	}
	private static int _fiveDollar;
	public static int FiveDollar{
		get { return Random(0,3);}
		set {	_fiveDollar = value; }
	}
	private static int _oneDollar;
	public static int OneDollar{
		get { return Random(0,3);}
		set {	_oneDollar = value; }
	}
	
	private static float _totalInPurse;
	public static float TotalInPurse{
		get { return (20*_twentyDollar+10*_tenDollar+5*_fiveDollar+1*_oneDollar);}
	}*/
}
