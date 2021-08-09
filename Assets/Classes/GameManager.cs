using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Web;
using Newtonsoft;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject homePage;

    public InputField urlInput;

    public GameObject plane;

    public Image framePrefab;
    public Text textPrefab;

    ///////Data

    private List<text> textLayers = new List<text>();
    private List<Frame> frameLayers = new List<Frame>();

    public List<Sprite> frameSprites;

    ///// Text input

    public GameObject textInputPage;
    public InputField textInputField;
    private string textInput;

    ///////////// To keep the record of progress

    private bool isFrame;
    private bool isFrameDone;

    private bool isText;
    private bool isTextDone;

    // ERROR

    public GameObject errorPage;
    public Text errorText;

    private void Start()
    {
        isFrame = isFrameDone = isText = isTextDone = false;
    }

    public void fetchData()
    {
        StartCoroutine(fetchDataFromURL());
    }

    private IEnumerator fetchDataFromURL() // Fetching Data
    {

        string HittingUrl = $"{urlInput.text}";
        using (UnityWebRequest Request = UnityWebRequest.Get(HittingUrl))
        {
            Request.method = UnityWebRequest.kHttpVerbGET;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");
            yield return Request.SendWebRequest();
            if (!Request.isNetworkError && !Request.isHttpError)
            {
                if (Request.responseCode.Equals(200))
                {
                    if (Request.downloadHandler.text != null)
                    {
                        string response = Request.downloadHandler.text.TrimStart('"').TrimEnd('"');
                        Layers layerList = Newtonsoft.Json.JsonConvert.DeserializeObject<Layers>(response);

                        foreach (Layer layer in layerList.layers)
                        {
                            if(layer.placement[0].position == null) // If position is null, we throw error
                            {
                                showError("Template invalid, placement not found");
                                break;
                            }

                            if (layer.type.ToString() == "frame")
                            {
                                if(layer.path == null || layer.path == "") // if path of frame is invalid, we throw error
                                {
                                    showError("Template invalid, frame path empty");
                                    break;
                                }
                                Frame temp = new Frame(layer);
                                frameLayers.Add(temp);
                            }
                            else if (layer.type.ToString() == "text")
                            {
                                text temp = new text(layer);
                                textLayers.Add(temp);
                            }

                        }

                        if (frameLayers.Count > 0)
                        {
                            isFrame = true;
                            StartCoroutine(getSprite());
                        }

                        if (textLayers.Count > 0)
                        {
                            isText = true;
                            getTextInput();
                        }

                    }
                    else
                    {
                        showError(Request.downloadHandler.text);
                    }
                }
                else
                {

                    showError(Request.downloadHandler.text);
                }
            }
            else
            {
                showError(Request.downloadHandler.text);
            }
        }
    }

    private IEnumerator getSprite() // getting sprites from the frame path
    {
        foreach (Frame frame in frameLayers)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(frame.path);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                showError(request.error);
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                frameSprites.Add(sprite);
            }
        }

        isFrameDone = true;

        if (isText && isTextDone || !isText)
        {
            setData();
        }
    }

    private void getTextInput() // go to text input page
    {
        textInputPage.SetActive(true);
    }

    public void textInputComplete() 
    {
        textInput = textInputField.text;
        textInputPage.SetActive(false);
        isTextDone = true;
        if (isFrame && isFrameDone || !isFrame)
        {
            setData();
        }
    }

    public void setData()
    {
        if (isFrame)
        {
            for (int i = 0; i < frameLayers.Count; i++)
            {
                frameLayers[i].init(plane, framePrefab, frameSprites[i]);
            }
        }
        
        if(isText)
        {
            for (int i = 0; i < textLayers.Count; i++)
            {
                textLayers[i].init(plane, textPrefab, textInput);
            }
        }

        plane.SetActive(true);
        homePage.SetActive(false);
    }
 
    public void showError(string msg)
    {
        errorText.text = msg;
        errorPage.SetActive(true);
        homePage.SetActive(false);
        textInputPage.SetActive(false);
    }

    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
