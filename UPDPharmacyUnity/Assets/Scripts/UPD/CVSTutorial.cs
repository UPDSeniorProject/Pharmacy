using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CVSTutorial : MonoBehaviour {

    // public
    public GameObject[] TutorialList;
    public GameObject TutorialBox;
    public GameObject PharmacySidebar;
    public GameObject EmpathyBar;
    public GameObject[] EmpathyBarLevels;
    public Text[] CheckList;
    public GameObject UnderInfluenceContainer;
    public GameObject[] UnderInfluenceTextContainer;
    public Text[] UnderInfluenceTexts;
    public GameObject EmpathyContainer;
    public Text[] EmpathyContainerTexts;
    public GameObject[] EmpathyContainerTextObjects;
    public GameObject[] CheckListOfGameObjects;
    public Camera CVSCam;
    public GameObject NextButton;
    public GameObject VirtualHuman;
    public GameObject EmpathyCoach;

    // menus
    public GameObject TypeHere;
    public GameObject SingleInstructionContainer;
    public GameObject[] SingleInstructionTasks;

    // private
    int EmpatheticCounter = 0;
    int ReactiveCounter = 0;
    int SingleInstCount = 0;
    int EmpathyVSReactive = 0;
    bool[] AskedIfUnderInfluence = { false, false };
    int EmpathyTask = 0;

    // const
    const int ASSESS_SITUATION = 4;
    const int ARMED = 7;
    const int UNDER_INFLUENCE = 9;
    const int EMPATHY = 10;
    const int UNDER_INFLUENCE_DRUGS = 0;
    const int UNDER_INFLUENCE_ALCOHOL = 1;
    const int EMPATHY_PARAPHRASE = 2;
    const int EMPATHY_DEEP_BREATH = 1;
    const int EMPATHY_INTRODUCE_YOURSELF = 0;
    const int EMPATHY_SET_LIMITS = 3;


    Color gray = new Color((127f / 255f), (140f / 255f), (141f / 255f)),
            yellow = new Color((247f / 255f), (187f / 255f), 0);

    int currentCheckListItem = 0;
    int TutorialCounter = 0;
    float zoomSpeed = 0.05f;
    FeedbackStats feedbackStats;

    int currentTutorialListOption = 0;
    int currentCheckListOption = 0;

    const int CheckListOptions_Assess = 0;
    const int CheckListOptions_Armed = 1;
    const int CheckListOptions_UnderInfluence = 2;
    const int CheckListOptions_CalmVirtualHuman = 3;
    const int CheckListOptions_GoToPrivateRoom = 4;

    bool tutorial = false;

    // Use this for initialization
    void Start() {

        // Checking if the patient is armed is the first thing to do in the scenario
        MakeCurrentCheckListOption(currentCheckListOption);

        // get Dialogue Tracker instance
        GameObject cam = GameObject.Find("CVS Pharmacy Camera");
        feedbackStats = (cam.GetComponent("DialogueTracker") as DialogueTracker).GetFeedbackStats(); ;
    }

    void Update() {
        // FIRST: wait for the fade in to occur
        // THEN: move forward
        // AFTER: start tutorial            

        if (!CVSCam.isActiveAndEnabled || !tutorial) {
            return;
        }


        if (initialWait-- < 1) {
            // if moving
            if (StepsForward-- > 0) {
                CVSCam.transform.Translate(Vector3.forward * zoomSpeed);
            }

            else if (pauseAfterZoom-- > 0) {
                // do nothing
            }

            // if finished moving, fires once only
            else if (TutorialCounter == 0 && !PharmacySidebar.activeInHierarchy) {
                EmpathyCoach.SetActive(true);
                PharmacySidebar.SetActive(true);
                StartTutorial();
            }
        }
    }

    int StepsForward, initialWait, pauseAfterZoom;
    public void onActiveCamera() {
        initialWait = 20;
        StepsForward = 75;
        pauseAfterZoom = 20;

        // Play intro animation
        VirtualHuman.GetComponent<VHAnimationManager>().CrossFadeAnimation("Intro, v2");

        // this boolean stops the update from occurring
        tutorial = true;
    }

    public GameObject GetVirtualHuman()
    {
        return VirtualHuman;
    }

    public void StartTutorial() {
        ShowTutorialBox();
        ContinueTutorial();
    }

    public void ShowTutorialBox() {
        TutorialBox.SetActive(true);
    }

    public void HideTutorialBox() {
        TutorialBox.SetActive(false);
    }

    void ClearMenu()
    {

        for (int i = 0; i < CheckList.Length; i++)
            CheckList[i].color = Color.yellow;

    }

    public bool ReadyToGoToPrivateLocation()
    {
        // this loop doesn't check the final element in the checklist, as
        // that would naturally not be checked because we haven't moved
        // to private location yet
        for (int i = 0; i < CheckList.Length - 1; i++)
            if (CheckList[i].color != gray)
                return false;

        return true;
    }

    public void ContinueTutorial()
    {
        if (TutorialCounter == TutorialList.Length || !tutorial)
        {
            ClearTutorialItems();
        }

        // regular item
        else
        {
            switch (TutorialCounter) {
                case 3: // what is problem
                case 6: // armed
                case 9: // drugs
                    GoToNextCheckListOption();
                    break;

                case 10: // everything else

                    // done with tutorial
                    HideTutorialBox();
                    break;

                default:
                    break;

            }

            DisplayNextTutorialItem();
        }
    }

    public void GoToNextCheckListOption()
    {
        // turns them green
        switch (currentCheckListOption)
        {
            case CheckListOptions_Assess:
                MakeCurrentCheckListOption(CheckListOptions_Armed);
                break;
            case CheckListOptions_Armed:
                MakeCurrentCheckListOption(CheckListOptions_UnderInfluence);
                break;
            case CheckListOptions_UnderInfluence:
                MakeCurrentCheckListOption(CheckListOptions_GoToPrivateRoom);
                break;
            case CheckListOptions_GoToPrivateRoom:
                MakeCurrentCheckListOption(CheckListOptions_CalmVirtualHuman);
                break;
        }
    }

    public void ClearTutorialItems()
    {
        tutorial = false;

        for (int i = 0; i < TutorialList.Length; i++)
            TutorialList[i].SetActive(false);
    }

    void CheckCurrentMenuItem()
    {
        // turn current item green
        CheckList[currentCheckListOption].color = gray;
    }

    void CheckItem(int c)
    {
        try
        {
            CheckList[c].color = gray;
            CheckListOfGameObjects[c + 1].SetActive(true);
            FindAndHighlightNextAvailableOption();
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(e.ToString());
        }
    }

    void UncheckItem(int c)
    {
        try
        {
            CheckList[c].color = yellow;
            FindAndHighlightNextAvailableOption();
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void FindAndHighlightNextAvailableOption()
    {
        bool Highlighted = false;

        for (int i = 0; i < CheckList.Length; i++)
        {
            if ((!Highlighted) && (CheckList[i].color != gray))
            {
                CheckList[i].color = yellow;
                currentCheckListItem = i;
                Highlighted = true;
            }

            else if ((Highlighted) && CheckList[i].color == yellow)
            {
                CheckList[i].color = yellow;
            }

        }
    }

    public void MakeCurrentCheckListOption(int option)
    {
        // specify the item
        if (option >= CheckList.Length)
        {
            return;
        }

        CheckListOfGameObjects[option].SetActive(true);

        if (option != CheckListOptions_Assess) {
            CheckCurrentMenuItem();
        }

        currentCheckListOption = option;
    }

    void MakeNextCheckListOption(int option)
    {
        // the next TODO
        CheckList[(int)option].color = Color.blue;
    }

    public void DisplayNextTutorialItem()
    {
        // disable current item
        if (TutorialCounter != 0) {
            TutorialList[TutorialCounter - 1].SetActive(false);
        }

        // looking in invalid spot
        if (TutorialCounter >= TutorialList.Length)
        {
            return;
        }

        // enable next item
        TutorialList[TutorialCounter].SetActive(true);

        TypeHere.SetActive(TutorialCounter == ASSESS_SITUATION); // only true if we're on ASSESS_SITUATION

        // some items shouldn't have a next button
        switch (TutorialCounter)
        {
            case ASSESS_SITUATION:
            case ARMED:
            case UNDER_INFLUENCE:
            case 11: // EMPATHY
                // single instruction logic
                if (SingleInstCount != 0)
                    SingleInstructionTasks[SingleInstCount - 1].SetActive(false);
                SingleInstructionTasks[SingleInstCount].SetActive(true);
                SingleInstCount++;

                SingleInstructionContainer.SetActive(true);
                TutorialBox.SetActive(false);
                break;

            default:
                TutorialBox.SetActive(true);
                SingleInstructionContainer.SetActive(false);
                break;
        }

        // loop to next tutorial item
        TutorialCounter++;
    }

    int GetNextCheckListOption()
    {
        // TODO: Implement more reusably.
        // http://stackoverflow.com/questions/642542/how-to-get-next-or-previous-enum-value-in-c-sharp

        switch (currentCheckListOption)
        {
            case CheckListOptions_Assess:
                return CheckListOptions_Armed;
            case CheckListOptions_Armed:
                return CheckListOptions_UnderInfluence;
            case CheckListOptions_UnderInfluence:
                return CheckListOptions_GoToPrivateRoom;
            case CheckListOptions_GoToPrivateRoom:
                return CheckListOptions_CalmVirtualHuman;
            case CheckListOptions_CalmVirtualHuman:
                return CheckListOptions_Armed;

            default:
                return CheckListOptions_Armed;
        }
    }


    public void ShowDeepBreaths()
    {
        EmpathyContainerTextObjects[1].SetActive(true);
    }

    public void ShowParaphrase()
    {
        EmpathyContainerTextObjects[2].SetActive(true);
    }


    public void ShowClearLimits()
    {
        EmpathyContainerTextObjects[3].SetActive(true);
    }

    // ~~~~~~~~~~~~~~~~~~~~~~
    // specialized functions

    public bool GoToPrivateRoom()
    {
        // check to see if conditions are met
        if (!ReadyToGoToPrivateLocation())
        {
            return false;
        }

        PrivateRoomLogic();
        return true;
    }

    void PrivateRoomLogic()
    {
        // first, move Megan
        GameObject vh = GameObject.Find("VirtualHuman");

        // move her to new room
        vh.transform.position = new Vector3(4.65f, 0.1f, 7.33f);

        // rotate her
        vh.transform.rotation = Quaternion.Euler(0f, 83.38f, 0f);

        // then swap cameras
        GameObject c = GameObject.Find("Cameras");
        ToggleCameras t = c.GetComponent("ToggleCameras") as ToggleCameras;

        // swap cameras
        t.SwapCameras(tutorial);
    }

    public void AssessSituation()
    {
        currentCheckListItem = 1;
        CheckItem(CheckListOptions_Assess);

        while (TutorialCounter < (ASSESS_SITUATION + 2))
            DisplayNextTutorialItem();
    }

    public void AskIfArmed()
    {
        currentCheckListItem = 2;
        CheckItem(CheckListOptions_Armed);

        while (TutorialCounter < (ARMED + 2))
            DisplayNextTutorialItem();

        UnderInfluenceContainer.SetActive(true);
    }

    public void UnderInfluenceLogic()
    {
        currentCheckListItem = 3;
        CheckItem(CheckListOptions_UnderInfluence);

        while (TutorialCounter < (UNDER_INFLUENCE + 2))
            DisplayNextTutorialItem();

        EmpathyBar.SetActive(true);
        EmpathyContainer.SetActive(true);

        UnderInfluenceContainer.SetActive(false);
    }

    public void AskIfUnderInfluenceDrugs()
    {
        UnderInfluenceTexts[UNDER_INFLUENCE_DRUGS].color = gray;
        AskedIfUnderInfluence[UNDER_INFLUENCE_DRUGS] = true;

        UnderInfluenceTextContainer[UNDER_INFLUENCE_ALCOHOL].SetActive(true);

        NextSingleInstruction();
    }

    public bool AskIfUnderInfluenceAlcohol()
    {
        if (UnderInfluenceTextContainer[UNDER_INFLUENCE_ALCOHOL].activeInHierarchy)
        {
            UnderInfluenceTexts[UNDER_INFLUENCE_ALCOHOL].color = gray;
            AskedIfUnderInfluence[UNDER_INFLUENCE_ALCOHOL] = true;

            UnderInfluenceLogic();

            EmpathySingleInstruction();

            return true;
        }

        return false;
    }

    void EmpathySingleInstruction()
    {
        // de-activate the instructions we're not using
        SingleInstructionTasks[2].SetActive(false);
        SingleInstructionTasks[3].SetActive(false);

        // activate the empathy instruction
        SingleInstructionTasks[3].SetActive(true);

        // update the counter
        SingleInstCount = 4;
    }

    public void NextSingleInstruction()
    {

        if (SingleInstructionContainer.activeInHierarchy && SingleInstCount < SingleInstructionTasks.Length)
        {
            if (SingleInstructionTasks[SingleInstCount - 1].activeInHierarchy)
            {
                SingleInstructionTasks[SingleInstCount - 1].SetActive(false);
                
                if (SingleInstCount < SingleInstructionTasks.Length)
                {
                    SingleInstructionTasks[SingleInstCount].SetActive(true);
                    SingleInstCount++;
                }
            }
        }
    }

    public void Empathy_IntroduceYourself()
    {
        EmpathyContainerTexts[EMPATHY_INTRODUCE_YOURSELF].color = gray;
    }

    public void Empathy_Paraphrase()
    {
        EmpathyContainerTexts[EMPATHY_PARAPHRASE].color = gray;
    }

    public void Empathy_DeepBreaths()
    {
        EmpathyContainerTexts[EMPATHY_DEEP_BREATH].color = gray;
    }

    public void Empathy_SetLimits()
    {
        EmpathyContainerTexts[EMPATHY_SET_LIMITS].color = gray;
    }


    public void StoreEmpathetic(string s)
    {
        EmpathyVSReactive++;
        if (currentCheckListItem == 3)
            // UpdateEmpathyIndicator();
        feedbackStats.StoreEmpathetic(s);
        CheckEmpathetic();
    }

    public void StoreReactive(string s)
    {
        EmpathyVSReactive--;
        if (currentCheckListItem == 3)
        {
            // UpdateEmpathyIndicator();
        }
        feedbackStats.StoreReactive(s);
        CheckEmpathetic();
    }

    public void UpdateEmpathyIndicator(int Task)
    {
        this.EmpathyTask = Task;

        for (int i = 0; i < EmpathyBarLevels.Length; i++)
            EmpathyBarLevels[i].SetActive(false);

        if (Task > 4)
            EmpathyBarLevels[4].SetActive(true);
        else if (Task < 0)
            EmpathyBarLevels[0].SetActive(true);
        else
            EmpathyBarLevels[Task].SetActive(true);

        CheckEmpathetic();
            
    }

    public void CheckEmpathetic()
    {
        
        // update empathetic counter
        EmpatheticCounter = (feedbackStats.GetEmpatheticCount());

        // update reactive counter
        ReactiveCounter = (feedbackStats.GetReactiveCount());

        // change sidebar colors, depending on whether each action is completed
        if (this.EmpathyTask == 4)
        {
            CheckItem(CheckListOptions_CalmVirtualHuman);
            // keep for now -- would make them feel good that they accomplished it

            if (EmpathyContainer.activeInHierarchy)
            {
                SingleInstructionTasks[SingleInstCount - 1].SetActive(false);
                SingleInstructionTasks[SingleInstCount].SetActive(true);
                SingleInstCount++;

                EmpathyContainer.SetActive(false);
            }
        }
    }

    public void MoveToPrivateLocation()
    {
        SingleInstructionContainer.SetActive(false);
        PharmacySidebar.SetActive(false);
    }
}
