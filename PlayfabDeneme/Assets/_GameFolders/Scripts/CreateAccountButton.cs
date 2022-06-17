using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountButton : MonoBehaviour
{
    [SerializeField] GameObject _loginCanvas;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] TMP_InputField _userNameField;
    [SerializeField] TMP_InputField _urlField;
    [SerializeField] Button _button;

    void Awake()
    {
        GetReference();
    }

    void OnValidate()
    {
        GetReference();
    }

    void OnEnable()
    {
        _button.onClick.AddListener(HandleOnButtonClicked);
    }
    
    void OnDisable()
    {
        _button.onClick.RemoveListener(HandleOnButtonClicked);
    }

    void HandleOnButtonClicked()
    {
        string userName = _userNameField.text;
        string url = _urlField.text;
        
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(url))
        {
            Debug.Log("Username and url can not be empty");
        }
        else
        {
            PlayfabManager.Instance.CreateAccount(userName, url);
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _loginCanvas.SetActive(false);
        }
    }

    private void GetReference()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }
}