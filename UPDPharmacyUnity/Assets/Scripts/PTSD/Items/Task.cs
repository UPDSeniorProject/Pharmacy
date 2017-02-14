using UnityEngine;
using System.Collections.Generic;


public class Task {

    public struct Options
    {
        string _name;
        bool _value;
    }

    public enum TaskTypes
    {
        None,
        Nutrition_task,
        Search_task,
        Price_task,
		Unitprice_task,
		PriceComparison_task,
		UnitPriceComparison_task,
        Change_making
    }


	private string _description;
	//private Options[] _options;
	private List<Options> _options;
	private TaskTypes _type;
	private string _goal;
	private string _goalcomparison;
    private bool _result;
	private float _finishTime;
	
	public Task() {
		_description = "";
		_options = new List<Options>();
		_type = TaskTypes.None;
		_goal = "";
        _result = false;
        _finishTime = 0f;
	}
	
	/*public Task(string name, float price) {
		_name = name;
		_price = price;
	}*/
	
	
	public string Description {
		get { return _description;  }
		set { _description = value; }
	}
	
	public List<Options> TaskOptions {
		get { return _options; }
	}
	
	
	public TaskTypes TaskType {
		get { return _type; }
		set { _type = value; }
	}
	
	public string TaskGoal{
		get { return _goal; }
		set { _goal = value; }
	}
	
	public string TaskGoalComparison{
		get { return _goalcomparison; }
		set { _goalcomparison = value; }
	}
    
	public bool TaskResult
    {
        get { return _result; }
        set { _result = value; }
    }


	public float TaskFinishTime{
        get { return _finishTime; }
        set { _finishTime = value; }
	}

}

