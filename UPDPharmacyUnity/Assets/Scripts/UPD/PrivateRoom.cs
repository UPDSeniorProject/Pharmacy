using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PrivateRoom : MonoBehaviour {
    // public
    public GameObject Sidebar;
    public Text[] SidebarTexts;
    public GameObject[] SidebarTextsGameObject;
    public GameObject TutorialPopup;
    public GameObject[] TutorialPanels;
    public Camera PrivateRoomCamera;
    public GameObject TutorialButton;
    public GameObject VirtualHuman;
    public GameObject MedicalHistoryBar;
    public GameObject[] MedicalHistoryLevels;
    public GameObject FeedbackScreen;
    public GameObject DoctorPostcard;

    public GameObject HistoryTextContainer;
    public Text[] HistoryText;
    public GameObject[] HistoryTestObject;
    public GameObject cellPhone;

    // private
    int SidebarAt = -1;
    int TutorialAt = -1;
    int EmpatheticCount = 0;
    int ReactiveCount = 0;
    int MedicalHistoryCount = 0;
    int TasksCompleted = 0;

    const int HISTORY_HOWLONG = 0;
    const int HISTORY_SOURCE = 1;
    const int HISTORY_SEESDOCTOR = 2;

    FeedbackStats fs;
    ToggleCameras tc;

    int SidebarOptions_FurtherRapport = 0;
    int SidebarOptions_SpeakWithDoctor = 0;
    int SidebarOptions_GetMeganHome = 0;

    Color gray = new Color((127f / 255f), (140f / 255f), (141f / 255f)),
            yellow = new Color((247f / 255f), (187f / 255f), 0);
        
    void Start () { }
	
	void Update () { }

    public void Init()
    {

        // make both Sidebar and Tutorial appear
        Sidebar.SetActive(true);

        // mset next Sidebar and Tutorial items
        FirstSidebarPrompt();

        // change animation to less tense one
        VirtualHuman.GetComponent<VHAnimationManager>().PlayAnimation("Least-Tense", 0, WrapMode.Loop);
        tc = GameObject.Find("Cameras").GetComponent("ToggleCameras") as ToggleCameras;
    }

    public void CheckTasksCompleted()
    {
        if (TasksCompleted > 2)
        {
            MakeFeedbackScreen();
        }
    }

    public void UpdateMedicalHistoryBar()
    {
        // if all check list options are showing, AND the last one is completed
        if (HistoryTestObject[2].activeInHierarchy &&
            HistoryText[2].color == gray)
        {

            // move to next sidebar prompt
            HistoryTextContainer.SetActive(false);
            SidebarTextsGameObject[1].SetActive(true);
            SidebarTexts[0].color = gray;
        }


        // clear all of them. don't want two levels showing at once.
        for (int i = 0; i < MedicalHistoryLevels.Length; i++)
            MedicalHistoryLevels[i].SetActive(false);


        // completed
        if (SidebarTextsGameObject[1].activeInHierarchy)
            MedicalHistoryLevels[3].SetActive(true);

        // third task
        else if (HistoryTestObject[2].activeInHierarchy)
            MedicalHistoryLevels[2].SetActive(true);

        // second task
        else if (HistoryTestObject[1].activeInHierarchy)
            MedicalHistoryLevels[1].SetActive(true);

        // first task
        else
            MedicalHistoryLevels[0].SetActive(true);
    }


    public void SetFeedbackStats(FeedbackStats fs)
    {
        this.fs = fs;
    }

    public void Hide()
    {
        // make both Sidebar and Tutorial disappear
        Sidebar.SetActive(false);
        TutorialPopup.SetActive(false);
    }

    public void History_HowLong()
    {
        HistoryText[HISTORY_HOWLONG].color = gray;
        HistoryTestObject[HISTORY_SOURCE].SetActive(true);
        HistoryCheck();
    }

    public void History_Source()
    {
        HistoryText[HISTORY_SOURCE].color = gray;
        HistoryTestObject[HISTORY_SEESDOCTOR].SetActive(true);
        HistoryCheck();
    }

    public void History_SeesDoctor()
    {
        // no need to check anything, this should trigger the end of the
        // history section

        HistoryText[HISTORY_SEESDOCTOR].color = gray;
        HistoryCheck();
    }

    public void HistoryCheck()
    {
        MedicalHistoryCount++;
        UpdateMedicalHistoryBar();
    }

    public void FirstSidebarPrompt()
    {
        SidebarTextsGameObject[0].SetActive(true);
    }

    public void NextSidebar()
    {
        if (SidebarAt >= SidebarTexts.Length)
            return;

        if (SidebarAt > -1)
            SidebarTexts[SidebarAt].color = gray;

        SidebarAt++;

        if (SidebarAt < SidebarTexts.Length)
            SidebarTexts[SidebarAt].color = yellow;
    }

    // triggered from conversation
    public void TalkToDoctor()
    {
        if (PrivateRoomCamera.isActiveAndEnabled && 
            SidebarTextsGameObject[1].activeInHierarchy)
        {

            TasksCompleted++;

            CheckItem(SidebarOptions_SpeakWithDoctor);
            OpenPostcard();

            SidebarTextsGameObject[1].SetActive(true);
            SidebarTexts[1].color = yellow;

        }
    }

    public void GetHome()
    {
        TasksCompleted++;

        if (PrivateRoomCamera.isActiveAndEnabled)
            CheckItem(SidebarOptions_GetMeganHome);

        CheckTasksCompleted();

    }

    public void NextTutorialItem()
    {
        if (TutorialAt > TutorialPanels.Length)
            return;

        if (TutorialAt > -1)
            TutorialPanels[TutorialAt].SetActive(false);

        TutorialAt++;

        // show next tutorial item
        if (TutorialAt < TutorialPanels.Length) 
            TutorialPanels[TutorialAt].SetActive(true);
        
        // hide
        else
            TutorialPopup.SetActive(false);

        // hide button when on the last screen
        if (TutorialAt == TutorialPanels.Length - 1)
            TutorialButton.SetActive(false);
    }

    void CheckItem(int s)
    {
        try { 
            SidebarTexts[s].color = gray;
            FindAndHighlightNextAvailableOption();
        } catch (KeyNotFoundException e)
        { }
    }

    void UncheckItem(int s)
    {
        try
        {
            SidebarTexts[s].color = yellow;
            FindAndHighlightNextAvailableOption();
        }
        catch (KeyNotFoundException e)
        { }
    }

    public void FindAndHighlightNextAvailableOption()
    {
        bool Highlighted = false;

        for (int i = 0; i < SidebarTexts.Length; i++)
        {
            if ((!Highlighted) && (SidebarTexts[i].color != gray))
            {
                SidebarTexts[i].color = yellow;
                SidebarAt = i;
                Highlighted = true;
            }

            else if ((Highlighted) && SidebarTexts[i].color == yellow)
            {
                SidebarTexts[i].color = yellow;
            }
        }
    }

    void CheckEmpathy()
    {
        if (EmpatheticCount - ReactiveCount > 2)
            CheckItem(SidebarOptions_FurtherRapport);
        else
            UncheckItem(SidebarOptions_FurtherRapport);
    }

    public void StoreEmpathetic(string s)
    {
        fs.StoreEmpathetic(s);
        EmpatheticCount++;
        CheckEmpathy();
    }

    public void StoreReactive(string s)
    {
        fs.StoreReactive(s);
        ReactiveCount++;
        CheckEmpathy();
    }

    public void MakeFeedbackScreen()
    {
        FeedbackScreen.SetActive(true);
        tc.UpdateFeedbackTime();
    }

    public void OpenPostcard()
    {
        DoctorPostcard.SetActive(true);
        Sidebar.SetActive(false);

        // play psychiatrist audio
        tc.PlayPsychiatristAudio();   
    }

    public void ClosePostcard()
    {
        DoctorPostcard.SetActive(false);
        Sidebar.SetActive(true);

        SidebarTextsGameObject[2].SetActive(true);
        SidebarTexts[1].color = gray;

        // stop psychiatrist audio
        tc.StopPsychiatristAudio();

        VirtualHuman.GetComponent<VHAnimationManager>().CrossFadeAnimation("idle_answerPhone", 0.5f, WrapMode.Once);
        cellPhone.SetActive(true);
    }
}
