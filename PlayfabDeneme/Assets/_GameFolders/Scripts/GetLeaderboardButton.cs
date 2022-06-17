using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLeaderboardButton : MonoBehaviour
{
    Button _button;
    
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
        PlayfabManager.Instance.GetLeaderboard();
    }

    private void GetReference()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }
}
