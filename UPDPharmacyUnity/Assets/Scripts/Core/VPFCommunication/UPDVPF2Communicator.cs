using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UPDVPF2Communicator : VPF2Communicator {

    bool Space = false;

#if UNITY_EDITOR

    // for animations, online bulid needs it
    string LOW_TENSE_CRY = "Low_Tense_Cry",
            LOW_TENSE_DEEPBREATH = "Low_Tense_DeepBreath",
            LOW_TENSE_NONOD = "Low_Tense_NoNod",
            LOW_TENSE_WHY = "Low_Tense_Why",
            LOW_TENSE_YESNOD = "Low_Tense_YesNod",

            MED_TENSE_DEEPBREATH = "Med_Tense_DeepBreath",
            MED_TENSE_NONOD = "Med_Tense_NoNod",
            MED_TENSE_WHY = "Med_Tense_Why",
            MED_TENSE_YESNOD = "Med_Tense_YesNod",
            MED_TENSE_CRY = "Med-Tense-Cry";

    protected string EventPrefix = "{ \"IsCritical\":false, \"IsOptional\":false, \"IsActionOnly\":false, \"ScenarioID\":18241, \"CharacterID\":22509, \"SpeechID\":91454, \"SpeechText\":\"I get anxious and I have a panic attack!\", \"AudioFileName\":\"286491_I_get_anxious_and_I_have_a_panic.wav\", \"AnimationID\":null, \"Animation\":\"\", \"AnimationList\":[],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[\"";
    protected string EventSuffix = "\"],\"Topics\":[],\"Discoveries\":[],\"MiscXML\":\"\",\"ReflectiveMoment\":null}";

    protected string AssessSituation;
    protected string AskIfArmed;
    protected string AskIfUnderInfluenceDrugs;
    protected string AskIfUnderInfluenceAlcohol;
    protected string Reactive;
    protected string Empathetic;
    protected string Empathy_IntroduceYourself;
    protected string Empathy_DeepBreaths;
    protected string Empathy_SetLimits;
    protected string Empathy_Paraphrase;
    protected string NoTag;
    protected string BetterLocation;
    protected string TalkToDoctor;
    protected string GetHome;
    protected string History_HowLong;
    protected string History_Source;
    protected string History_SeesDoctor;

    protected string DummyValue;

    protected bool[] check = { false, false, false, false, false, false, false, false, false, false, false, false, false };


#else
    protected string EventPrefix = "";
    protected string EventSuffix = "";
#endif

    // ~~~~~~~~~~~~~~~~~
    // lots of functionality, so lots of private variables

    // objects
    Camera CVSCamera;
    Camera PrivateRoomCamera;
    GameObject privateRoomHolder;

    // scripts
    ToggleCameras tc;
    CVSTutorial cvsTut;
    PrivateRoom pr;
    EmpathyCoach ec;
    VHAnimationManager animationManager;
    GameObject vh;

    // classes
    FeedbackStats fs;

    float time = Time.time;
    bool PharmacyScene = true;
    bool PrivateRoomScene = false;
    bool CVSTimeGotten = false;
    bool onEmpathy = false;
    int EmpathyTask = 0;
    int TimeSinceLastEmpathySuccess = 0;

    protected override void Start()
    {
        // cover all regular functionality
        base.Start();

        // initialize all private variables above
        InitializeEverything();

    }

    protected override void Update()
    {
        // cover regular functionality
        base.Update();
        // tc.UpdateTime();

#if UNITY_EDITOR
        CheckKeyStrokes();
#endif

    }

    void InitializeEverything()
    {

#if UNITY_EDITOR
        // build all tags
        AssessSituation = BuildTag("AssessSituation");
        AskIfArmed = BuildTag("AskIfArmed");
        AskIfUnderInfluenceDrugs = BuildTag("AskIfUnderInfluenceDrugs");
        AskIfUnderInfluenceAlcohol = BuildTag("AskIfUnderInfluenceAlcohol");
        Reactive = BuildTag("Reactive");
        Empathetic = BuildTag("Empathetic");
        NoTag = BuildTag("");
        BetterLocation = BuildTag("MovePrivateLocation");
        TalkToDoctor = BuildTag("TalkToDoctor");
        GetHome = BuildTag("GetHome");

        Empathy_IntroduceYourself = BuildTag("Empathy_IntroduceYourself");
        Empathy_DeepBreaths = BuildTag("Empathy_DeepBreaths");
        Empathy_SetLimits = BuildTag("Empathy_SetLimits");
        Empathy_Paraphrase = BuildTag("Empathy_Paraphrase");

        History_HowLong = BuildTag("History_HowLong");
        History_Source = BuildTag("History_Source");
        History_SeesDoctor = BuildTag("History_SeesDoctor");

        DummyValue = BuildTag("DummyValue");

#endif

        // initialize the global variables
        GameObject CVSCameraObject = GameObject.Find("CVS Pharmacy Camera");
        CVSCamera = CVSCameraObject.GetComponent<Camera>();
        fs = (CVSCameraObject.GetComponent("DialogueTracker") as DialogueTracker).GetFeedbackStats();
        tc = GameObject.Find("Cameras").GetComponent("ToggleCameras") as ToggleCameras;
        cvsTut = (GameObject.Find("Pharmacy Tutorial")).GetComponent("CVSTutorial") as CVSTutorial;
        privateRoomHolder = GameObject.Find("Private Room");
        pr = privateRoomHolder.GetComponent("PrivateRoom") as PrivateRoom;
        PrivateRoomCamera = GameObject.Find("Private Room Camera").GetComponent<Camera>();
        ec = GameObject.Find("EmpathyCoach").GetComponent<EmpathyCoach>();
        vh = cvsTut.GetVirtualHuman();
        animationManager = vh.GetComponent<VHAnimationManager>();

        // feed any necessary information into here
        pr.SetFeedbackStats(fs);
    }

#if UNITY_EDITOR
    public void checkNext()
    {
        if (!check[0])
            TriggerEvent(AssessSituation);

        else if (!check[1])
            TriggerEvent(AskIfArmed);

        // Ask If Under Influence
        else if (!check[2])
            TriggerEvent(AskIfUnderInfluenceDrugs);
        else if (!check[3])
            TriggerEvent(AskIfUnderInfluenceAlcohol);

        // Three empathetic statements
        else if (!check[4])
        {
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);

            // the following two shouldn't fire
            TriggerEvent(TalkToDoctor);
            TriggerEvent(GetHome);
        }

        // Ask To Leave
        else if (!check[5])
            TriggerEvent(BetterLocation);

        // ~~~~~~~~~~~~~
        // now in private room

        // establishing further rapport
        // - would be good in future if this was based on finding out relevant information
        else if (!check[6])
        {
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);
        }

        // talk to doctor
        else if (!check[7])
            TriggerEvent(TalkToDoctor);

        // go home
        else if (!check[8])
            TriggerEvent(GetHome);

        else if (!check[10])
        {
            pr.MakeFeedbackScreen();
        }

        // check next thing
        for (int i = 0; i < check.Length; i++)
        {
            if (!check[i])
            {
                check[i] = true;
                break;
            }
        }

    }
#endif

    public void Animate(string Animation)
    {
        if (animationManager != null)
            animationManager.CrossFadeAnimation(Animation);
    }

    public override JSONObject TriggerEvent(string s)
    {
        JSONObject obj;

        // base.TriggerEvent(s) does all the VPF stuff. If you're running
        // local, none of this will work so we're just making empty objects

#if UNITY_EDITOR
        obj = new JSONObject(s);
#else
        obj = base.TriggerEvent(s);
#endif

        int NoEmpathySuccess = 0;
        
        foreach (JSONObject o in obj["Tags"].list)
        {
            if (o.str != null)
            {
                // the following two tags could happen both
                // in a setting of the CVS Tutorial and in 
                // the private room

                fs.OnDialogue();

                if (o.str == "Empathetic")
                {
                    // if user asks her to take a breath, trigger breath animation. not random one.
                    if (obj["Tags"].list.Contains("Empathy_DeepBreaths"))
                        TriggerEmpathetic(false);
                    else
                        TriggerEmpathetic();

                    return obj;
                }

                else if (o.str == "Reactive")
                {
                    TriggerReactive();
                    return obj;
                }


                // pharmacy options
                if (o.str == "AssessSituation")
                    cvsTut.AssessSituation();
                else if (o.str == "MovePrivateLocation")
                    TriggerMoveToPrivateRoom();
                else if (o.str == "AskIfArmed")
                    cvsTut.AskIfArmed();
                else if (o.str == "AskIfUnderInfluenceDrugs")
                    cvsTut.AskIfUnderInfluenceDrugs();
                else if (o.str == "AskIfUnderInfluenceAlcohol")
                {
                    if (cvsTut.AskIfUnderInfluenceAlcohol())
                        onEmpathy = true;

                    TimeSinceLastEmpathySuccess = 0;
                    ec.ShowBlueBrain(EmpathyTask);

                }

                if (onEmpathy)
                {
                    if (o.str == "Empathy_IntroduceYourself")
                    {
                        cvsTut.Empathy_IntroduceYourself();
                        TimeSinceLastEmpathySuccess = 0;

                        if (EmpathyTask < 1)
                            EmpathyTask = 1;

                        cvsTut.ShowDeepBreaths();
                        ec.ShowBlueBrain(EmpathyTask);

                    }
                    else if (o.str == "Empathy_DeepBreaths")
                    {
                        cvsTut.Empathy_DeepBreaths();
                        TimeSinceLastEmpathySuccess = 0;

                        if (EmpathyTask < 2)
                            EmpathyTask = 2;

                        // we want her to take a breath if this is called
                        if (CVSCamera.isActiveAndEnabled)
                            Animate("Med_Tense_DeepBreath");
                        else
                            Animate("Low_Tense_DeepBreath");
                        cvsTut.ShowParaphrase();
                        ec.ShowBlueBrain(EmpathyTask);

                    }
                    else if (o.str == "Empathy_Paraphrase")
                    {
                        cvsTut.Empathy_Paraphrase();
                        TimeSinceLastEmpathySuccess = 0;

                        if (EmpathyTask < 3)
                            EmpathyTask = 3;

                        cvsTut.ShowClearLimits();
                        ec.ShowBlueBrain(EmpathyTask);

                    }
                    else if (o.str == "Empathy_SetLimits")
                    {
                        cvsTut.Empathy_SetLimits();
                        onEmpathy = false;
                        TimeSinceLastEmpathySuccess = 0;
                        EmpathyTask = 4;
                    }
                    else
                    {
                        NoEmpathySuccess++;
                    }

                    cvsTut.UpdateEmpathyIndicator(EmpathyTask);

                }

                // private room options
                // empathy applies here
                else if (o.str == "TalkToDoctor")
                    pr.TalkToDoctor();
                else if (o.str == "GetHome")
                    pr.GetHome();

                else if (o.str == "History_HowLong")
                    pr.History_HowLong();
                else if (o.str == "History_Source")
                    pr.History_Source();
                else if (o.str == "History_SeesDoctor")
                    pr.History_SeesDoctor();
                    
            }
        }

        /*if (onEmpathy && NoEmpathySuccess == obj["Tags"].list.Count)
        {
            TimeSinceLastEmpathySuccess++;
            if (TimeSinceLastEmpathySuccess == 3)
            {
                ec.ShowBlueBrain(EmpathyTask);
            }
        }*/

        return obj;
    }

    string BuildTag(string Tag)
    {
#if UNITY_EDITOR
        EventPrefix = "{ \"IsCritical\":false, \"IsOptional\":false, \"IsActionOnly\":false, \"ScenarioID\":18241, \"CharacterID\":22509, \"SpeechID\":91454, \"SpeechText\":\"I get anxious and I have a panic attack!\", \"AudioFileName\":\"286491_I_get_anxious_and_I_have_a_panic.wav\", \"AnimationID\":null, \"Animation\":\"\", \"AnimationList\":[],\"AppActions\":[],\"VideoURL\":null,\"ImageTransition\":null,\"Tags\":[\"";
        EventSuffix = "\"],\"Topics\":[],\"Discoveries\":[],\"MiscXML\":\"\",\"ReflectiveMoment\":null}";
        return EventPrefix + Tag + EventSuffix;
#else
        return "";
#endif
    }

#if UNITY_EDITOR
    void RunCheckingSimulation()
    {
        if (time == 0)
            Debug.Log("Started!");

        else if (time % 20 == 0)
            Debug.Log((time / 20) + " seconds have passed");

        // Assess Situation
        if (time == 200)
            TriggerEvent(AssessSituation);

        // Ask If Armed
        else if (time == 220)
            TriggerEvent(AskIfArmed);

        // Ask If Under Influence
        else if (time == 240)
        {
            TriggerEvent(AskIfUnderInfluenceDrugs);
            TriggerEvent(AskIfUnderInfluenceAlcohol);
        }

        // Not implemented: Reactive statement
        // else if (time == 360)
        //     TriggerEvent(Reactive);

        // Three empathetic statements
        else if (time == 260)
        {
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);

            // the following two shouldn't fire
            TriggerEvent(TalkToDoctor);
            TriggerEvent(GetHome);
        }

        // Ask To Leave
        else if (time == 280)
            TriggerEvent(BetterLocation);

        // ~~~~~~~~~~~~~
        // now in private room

        // establishing further rapport
        // - would be good in future if this was based on finding out relevant information
        else if (time == 320)
        {
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);
            TriggerEvent(Empathetic);
        }

        // talk to doctor
        else if (time == 340)
            TriggerEvent(TalkToDoctor);

        // go home
        else if (time == 360)
            TriggerEvent(GetHome);

        // increment time
        time++;
    }
#endif

    public void TriggerEmpathetic(bool andAnimate = true)
    {
        if (CVSCamera.isActiveAndEnabled)
        {
            // cvsTut.StoreEmpathetic("test");
            if (andAnimate)
                RandomAnimate("Med_Tense_DeepBreath", "Med_Tense_YesNod");
        }

        else if (PrivateRoomCamera.isActiveAndEnabled)
        {
            // pr.StoreEmpathetic("test");
            if (andAnimate)
                RandomAnimate("Low_Tense_DeepBreath", "Low_Tense_YesNod");
        }
    }

    public void TriggerReactive()
    {
        if (CVSCamera.isActiveAndEnabled)
        {
            // cvsTut.StoreReactive("test");
            RandomAnimate("Med-Tense-Cry", "Med_Tense_NoNod", "Med_Tense_Why");
        }


        else if (PrivateRoomCamera.isActiveAndEnabled)
        {
            // pr.StoreReactive("test");
            RandomAnimate("Low_Tense_Cry", "Low_Tense_Why", "Low_Tense_NoNod");
        }
    }

    public void RandomAnimate(params string[] Animations)
    {
        int RandomNumber = UnityEngine.Random.Range(0, Animations.Length);
        Animate(Animations[RandomNumber]);
    }

    public void TriggerMoveToPrivateRoom()
    {
        if (CVSCamera.isActiveAndEnabled)
        {
            StartCoroutine(WaitToMovePrivateRoom());
        }
    }

    IEnumerator WaitToMovePrivateRoom()
    {
        // This is a coroutine, so that we can wait
        // until Megan finishes her dialogue. I recorded it
        // and her dialogue lasts 6.55 seconds. I'm going to wait
        // 8 seconds just to be on the safe side.

        yield return new WaitForSeconds(8f); // wait 8 seconds

        bool MovedToPrivateRoom = cvsTut.GoToPrivateRoom();
        if (MovedToPrivateRoom)
        {
            PharmacyScene = false;
            PrivateRoomScene = true;
            ec.hide();
        }
    }

#if UNITY_EDITOR
    void CheckKeyStrokes()
    {
        // runs sample dialogue to check all the functionality
        if (Space)
            RunCheckingSimulation();

        if (Input.GetKey(KeyCode.M))
        {
            if (Input.GetKeyDown(KeyCode.C))
                Animate("Med-Tense-Cry");

            else if (Input.GetKeyDown(KeyCode.B))
                Animate("Med_Tense_DeepBreath");

            else if (Input.GetKeyDown(KeyCode.N))
                Animate("Med_Tense_NoNod");

            else if (Input.GetKeyDown(KeyCode.W))
                Animate("Med_Tense_Why");

            else if (Input.GetKeyDown(KeyCode.Y))
                Animate("Med_Tense_YesNod");
        }

        else if (Input.GetKey(KeyCode.L))
        {
            if (Input.GetKeyDown(KeyCode.B))
                Animate("Low_Tense_DeepBreath");

            if (Input.GetKeyDown(KeyCode.N))
                Animate("Low_Tense_NoNod");

            if (Input.GetKeyDown(KeyCode.W))
                Animate("Low_Tense_Why");

            if (Input.GetKeyDown(KeyCode.Y))
                Animate("Low_Tense_YesNod");

            if (Input.GetKeyDown(KeyCode.C))
                Animate("Low_Tense_Cry");
        }

        else {
            if (Input.GetKeyUp(KeyCode.Space))
                checkNext();

            // assess sit
            if (Input.GetKeyUp(KeyCode.Alpha1))
                TriggerEvent(AssessSituation);

            // armed
            if (Input.GetKeyUp(KeyCode.Alpha2))
                TriggerEvent(AskIfArmed);

            // under influence
            if (Input.GetKeyUp(KeyCode.Alpha3))
                TriggerEvent(AskIfUnderInfluenceAlcohol);

            if (Input.GetKeyUp(KeyCode.Alpha4))
                TriggerEvent(AskIfUnderInfluenceDrugs);

            // empathy types
            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                TriggerEvent(Empathetic);
                TriggerEvent(Empathy_IntroduceYourself);
            }

            if (Input.GetKeyUp(KeyCode.Alpha6))
            {
                TriggerEvent(Empathetic);
                TriggerEvent(Empathy_DeepBreaths);
            }

            if (Input.GetKeyUp(KeyCode.Alpha7))
            {
                TriggerEvent(Empathetic);
                TriggerEvent(Empathy_Paraphrase);
            }

            if (Input.GetKeyUp(KeyCode.Alpha8))
            {
                TriggerEvent(Empathetic);
                TriggerEvent(Empathy_SetLimits);
            }

            if (Input.GetKeyUp(KeyCode.Alpha9))
            {
                TriggerEvent(BetterLocation);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                TriggerEvent(History_HowLong);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                TriggerEvent(History_Source);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                TriggerEvent(History_SeesDoctor);
            }

            // other things
            if (Input.GetKeyUp(KeyCode.E))
                TriggerEvent(Empathetic);

            if (Input.GetKeyUp(KeyCode.R))
                TriggerEvent(Reactive);

            if (Input.GetKeyUp(KeyCode.T))
                TriggerEvent(NoTag);

            if (Input.GetKeyUp(KeyCode.B))
                ec.ToggleMessageBox();

            if (Input.GetKeyUp(KeyCode.D))
                TriggerEvent(TalkToDoctor);

            if (Input.GetKeyUp(KeyCode.H))
                TriggerEvent(GetHome);

            if (Input.GetKeyUp(KeyCode.P))
                TriggerEvent(BetterLocation);

            if (Input.GetKey(KeyCode.U) && Input.GetKeyUp(KeyCode.D))
                TriggerEvent(AskIfUnderInfluenceDrugs);

            if (Input.GetKey(KeyCode.U) && Input.GetKeyUp(KeyCode.A))
                TriggerEvent(AskIfUnderInfluenceAlcohol);

            if (Input.GetKeyUp(KeyCode.Z))
                TriggerEvent(DummyValue);

            if (Input.GetKeyUp(KeyCode.F))
                tc.PlayPoliceCallAudio();

            if (Input.GetKeyUp(KeyCode.G))
                tc.PlayPsychiatristAudio();
        }
    }

#endif
}
