/// <summary>
/// Item.cs
/// This is the base class for all items in our game. In order for an item to be placed in to our
/// inventory, it will have to be of a type that is dirived from this class
/// 
/// Ths script does not get attached to anyhing
/// </summary>
using UnityEngine;

public class Item {
	private string _name;
	private float _price;
    private int _quantity;
    private string _standpos;
	private float _unitprice;
	//public Texture2D _icon;

	
	public Item() {
		_name = "";
		_price = 0;
		_unitprice = 0;
        _quantity = 1;
        _standpos = "";
	}
	
	public Item(string name, float price, int quantity, string standpos) {
		_name = name;
		_price = price;
        _quantity = quantity;
        _standpos = standpos;
	}
	
	public Item(string name, float price, int quantity, string standpos, float unitprice) {
		_name = name;
		_price = price;
        _quantity = quantity;
        _standpos = standpos;
		_unitprice = unitprice;
	}
	
	public string Name {
		get { return _name;  }
		set { _name = value; }
	}
	
	public float Price {
		get { return _price; }
		set { _price = value; }
	}
	
	public float Unitprice {
		get { return _unitprice; }
		set { _unitprice = value; }
	}
	
    public int Quantity
    {
        get { return _quantity; }
        set { _quantity = value; }
    }

    public string Standpos
    {
        get { return _standpos;  }
        set { _standpos = value; }
    }
	
	
	/*public Texture2D Icon {
		get { return _icon; }
		set { _icon = value; }
	}*/
	
	public virtual string ToolTip() {
		return Name + "\n" +
				"Price " + Price;
	}
}
