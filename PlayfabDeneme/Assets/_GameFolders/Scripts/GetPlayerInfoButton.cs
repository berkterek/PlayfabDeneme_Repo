using UnityEngine;
using UnityEngine.UI;

public class GetPlayerInfoButton : MonoBehaviour
{
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
        PlayfabManager.Instance.GetPlayerInfo();
    }

    private void GetReference()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }
}
