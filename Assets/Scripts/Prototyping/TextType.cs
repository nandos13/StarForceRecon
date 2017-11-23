using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextType : MonoBehaviour {

    public float delay = 0.1f;
    public string fullText;
    private string currentText = "";

	// Use this for initialization
	void Start () {
        StartCoroutine(ShowText());
	}

    IEnumerator ShowText()
    {
        for(int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0,i);
            this.GetComponent<Text>().text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
