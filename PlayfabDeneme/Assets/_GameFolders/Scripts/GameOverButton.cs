using UnityEngine;
using UnityEngine.UI;

public class GameOverButton : MonoBehaviour
{
    [SerializeField] PlayerDataContainerSO _playerDataContainer;
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
        PlayfabManager.Instance.SendLeaderboardDataWithCloudScript(_playerDataContainer.Score);
    }

    private void GetReference()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }
}