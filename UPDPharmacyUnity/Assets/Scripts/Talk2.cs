using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class Talk2 : MonoBehaviour {

    private DictationRecognizer m_DictationRecognizer;
    //public Text speechText;
    public string patientName;
    private VPF2ApiAccess apiAcces;
  //  private VPF2Communicator _Comm;

    void Start()
    {
        apiAcces = this.GetComponent<VPF2ApiAccess>();
        m_DictationRecognizer = new DictationRecognizer();
        
        m_DictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        m_DictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        m_DictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        m_DictationRecognizer.DictationError += DictationRecognizer_DictationError;

        //_Comm = gameObject.GetComponent<VPF2Communicator>();

        m_DictationRecognizer.Start();
    }

    private void outputQuestionAndResponseToUI(string text, bool isInput)
    {
        string whoSaidIt;
        if (isInput)
        {
            whoSaidIt = "You";
        }
        else
        {
            whoSaidIt = patientName;
        }
        //speechText.text = speechText.text + whoSaidIt + ": " + text + "\n\n";
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.LogFormat("Dictation result: {0}", text);
        outputQuestionAndResponseToUI(text, true);

        StartCoroutine(apiAcces.FindResponse(text, (result) =>
        {
            Debug.Log("In API coroutine");
            JSONObject obj = new JSONObject(result);
            Debug.LogFormat("Response: {0}", obj["SpeechText"].str);
            //outputQuestionAndResponseToUI(obj["SpeechText"].str, false);

           /* if (_Comm == null)
                _Comm = this.GetComponent<VPF2Communicator>();

            _Comm.TriggerEvent(result);*/
        }));
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.LogFormat("Dictation hypothesis: {0}", text);
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("ended");
        if (cause != DictationCompletionCause.Complete)
            Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", cause);
        m_DictationRecognizer.Stop();
        m_DictationRecognizer.Start();
        Debug.Log("Started");
    }

    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
    }
}
