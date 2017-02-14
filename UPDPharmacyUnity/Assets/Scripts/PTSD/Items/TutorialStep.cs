using UnityEngine;
using System.Collections.Generic;


public class TutorialStep {
	
	public struct Options
	{
		string _name;
		bool _value;
	}
	
	public enum TutorialTaskTypes
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

	public enum TutorialGroup
		{
		Initial,
		Task,
		Task_Follow,
		Checkout,
		Final
		}

	public TutorialStep() {
		Description = "";
		TaskOptions = new List<Options>();
		TaskType = TutorialTaskTypes.None;
		TaskGoal = "";
		TaskResult = false;
		TaskFinishTime = 0f;
		SpriteName = "";
		OverlayName = "";
		TutorialGrouping = TutorialGroup.Initial;
		StepGoal = "";
		ProgressionText = "";
		PlaySound = true;
	}
	
	public string Description { get; set; }
	public List<Options> TaskOptions { get; set; }
	public TutorialTaskTypes TaskType { get; set; }
	public string TaskGoal{ get; set; }
	public string TaskGoalComparison { get; set; }
	public bool TaskResult { get; set; }
	public float TaskFinishTime{ get; set; }
	public string SpriteName { get; set; }
	public string OverlayName { get; set; }
	public TutorialGroup TutorialGrouping { get; set; }
	public string StepGoal { get; set; }
	public string ProgressionText { get; set; }
	public bool PlaySound { get; set; }
}