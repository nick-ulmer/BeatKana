using TMPro;
using UnityEngine;

public class KanaKeyboard : MonoBehaviour
{
    InputString InputString;
    KanaButtonGuide KanaButtonGuide;

    [SerializeField] KanaButton BigAKanaButton;

    public void SetDakutenSpecialKeys(string[] keys) { this.dakutenSpecialKeys = keys; }
    string[] dakutenSpecialKeys = new string[5];

    public void PlayManagerSetFields(PlayManager.TLFields tlFields)
    { 
        this.InputString = tlFields.InputString;
    }
    public void Initialize()
    {
        BigAKanaButton.gameObject.SetActive(false);

        foreach (var kanaButton in GetComponentsInChildren<KanaButton>())
        {
            kanaButton.Init();
        }

        KanaButtonGuide = gameObject.GetComponentInChildren<KanaButtonGuide>();
        KanaButtonGuide.Initialize();
        KanaButtonGuideDeactivate();
    }


    public void KanaButtonGuideActivate(Vector3 pos, float y_diff, string[] keys) 
    {
        KanaButtonGuide.gameObject.SetActive(true); 
        KanaButtonGuide.SetActivate(pos, y_diff, keys);
        KanaButtonGuide.prevIndex = -1; // to ensure that a change in string index always initially occurs
    }
    public void KanaButtonGuideSetIndex(int index) { KanaButtonGuide.SetIndex(index); }
    public void KanaButtonGuideDeactivate()
    {
        KanaButtonGuide.gameObject.SetActive(false);
    }



    public void InputFieldUpdated() 
    { 
        // Empty and unused
    }


    public void InputToField(string key, string text)
    {
        if (key == "dakuten") 
            text = DakutenProcess(text);

        //inputField.text = inputField.text + text;
        InputString.AddString(text);
        InputFieldUpdated();
    }

    string DakutenProcess(string text)
    {
        if (InputString.IsEmpty()) return "";
        string switchButton = this.dakutenSpecialKeys[0];
        string dakutenButton = this.dakutenSpecialKeys[1];
        string komojiButton = this.dakutenSpecialKeys[2];
        string handakutenButton = this.dakutenSpecialKeys[3];
        string nothingButton = this.dakutenSpecialKeys[4];
        if (text == nothingButton) return "";


        char lastChar = InputString.FinalChar();

        switch (text)
        {
            case var x when x == switchButton:
                Debug.Log("Special key: " + switchButton);
                if (KanaData.SwitchKana(lastChar, out char next))
                {
                    InputString.RemoveFromEnd();
                    return next.ToString();
                }
                break;
            case var x when x == dakutenButton:
                Debug.Log("Special key: " + dakutenButton);
                if (KanaData.ToDakuten(lastChar, out char dakuten))
                {
                    InputString.RemoveFromEnd();
                    return dakuten.ToString();
                }
                break;
            case var x when x == komojiButton:
                Debug.Log("Special key: " + komojiButton);
                if (KanaData.ToKomoji(lastChar, out char komoji))
                {
                    InputString.RemoveFromEnd();
                    return komoji.ToString();
                }
                break;
            case var x when x == handakutenButton:
                Debug.Log("Special key: " + handakutenButton);
                if (KanaData.ToHandakuten(lastChar, out char handakuten))
                {
                    InputString.RemoveFromEnd();
                    return handakuten.ToString();
                }
                break;
        }

        return lastChar.ToString();
    }

    public void BackspaceOnField()
    {
        InputString.RemoveFromEnd();
    }
}
