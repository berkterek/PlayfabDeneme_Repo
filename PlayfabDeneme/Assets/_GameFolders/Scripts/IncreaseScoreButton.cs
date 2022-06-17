using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class IncreaseScoreButton : MonoBehaviour
{
    [SerializeField] PlayerDataContainerSO _playerDataContainer;
    [SerializeField] TMP_Text _text;
    [SerializeField] Button _button;
    [SerializeField] int _increaseValue = 1;

    void Awake()
    {
        GetReference();
    }

    void OnValidate()
    {
        GetReference();
    }

    void Start()
    {
        _text.text = _playerDataContainer.Score.ToString();
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
        _playerDataContainer.Score += _increaseValue;
        _text.text = _playerDataContainer.Score.ToString();
    }

    private void GetReference()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }
}