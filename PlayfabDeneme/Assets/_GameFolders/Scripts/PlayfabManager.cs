using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using JsonObject = PlayFab.Json.JsonObject;

public class PlayfabManager : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] GameObject _loginObject;

    string _playerID;
    public static PlayfabManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    IEnumerator Start()
    {
        Login();

        yield return new WaitForSeconds(2f);

        GetPlayerInfo();
    }

    void Login()
    {
        // var request = new LoginWithCustomIDRequest()
        // {
        //     CustomId = SystemInfo.deviceUniqueIdentifier,
        //     CreateAccount = true,
        //
        // };
        //
        // PlayFabClientAPI.LoginWithCustomID(request, HandleOnSuccess, HandleOnError);

        //if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email)) return;
#if UNITY_EDITOR

        var request = new LoginWithCustomIDRequest()
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        
        PlayFabClientAPI.LoginWithCustomID(request, HandleOnLoginSuccess, HandleOnError);

#elif UNITY_ANDROID

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false).AddOauthScope("profile").Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
        {
            var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            Debug.Log(serverAuthCode);

            var request = new LoginWithGoogleAccountRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = serverAuthCode,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithGoogleAccount(request, HandleOnLoginSuccess, HandleOnLoginError);
        });

#elif UNITY_IOS
        var request = new LoginWithIOSDeviceIDRequest()
        {
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        
        PlayFabClientAPI.LoginWithIOSDeviceID(request, HandleOnLoginSuccess,HandleOnLoginError);
#endif
    }

    void HandleOnError(PlayFabError result)
    {
        Debug.Log(result.GenerateErrorReport());
    }

    void HandleOnLoginSuccess(LoginResult result)
    {
        Debug.Log("You can successful login");

        _playerID = result.PlayFabId;
        Debug.Log(_playerID);
        var request = new GetPlayerProfileRequest()
        {
            PlayFabId = _playerID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, HandleOnGetProfileSuccess, HandleOnError);
    }

    void HandleOnGetProfileSuccess(GetPlayerProfileResult result)
    {
        var playerProfile = result.PlayerProfile;
        string displayName = playerProfile.DisplayName;

        if (string.IsNullOrEmpty(displayName))
        {
            Debug.Log("player display name not exist");
            var request = new GetUserDataRequest()
            {
                PlayFabId = _playerID,
                Keys = new List<string>()
            };

            request.Keys.Add("player_url");

            PlayFabClientAPI.GetUserData(request, HandleOnGetPlayerDataSuccess, HandleOnError);
        }
        else
        {
            Debug.Log($"player display name exist => {displayName}");
            var request = new GetUserDataRequest()
            {
                PlayFabId = _playerID,
                Keys = new List<string>()
            };

            request.Keys.Add("player_url");

            PlayFabClientAPI.GetUserData(request, HandleOnGetUserDataSuccess, HandleOnError);
        }
    }

    void HandleOnGetUserDataSuccess(GetUserDataResult result)
    {
        Debug.Log(result.Data["player_url"].Value);
        StartCoroutine(CloseLoginPageAsync());
    }

    void HandleOnGetPlayerDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey("player_url"))
        {
            Debug.Log("Player has no url data");
        }
        else
        {
            Debug.Log("Player has url data");
            Debug.Log(result.Data["player_url"].Value);
        }
    }

    private IEnumerator CloseLoginPageAsync()
    {
        yield return new WaitForSeconds(2f);
        _loginObject.SetActive(false);
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    void HandleOnLoginError(PlayFabError result)
    {
        Debug.Log(result.GenerateErrorReport());
    }

    public void SendLeaderboardDataWithCloudScript(int newScore)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "updatePlayerScore",
            FunctionParameter = new { score = newScore },
            GeneratePlayStreamEvent = true
        }, HandleOnCloudSendLeaderboardDataSuccess, HandleOnError);
    }

    void HandleOnCloudSendLeaderboardDataSuccess(ExecuteCloudScriptResult context)
    {
        JsonObject jsonResult = (JsonObject)context.FunctionResult;
        object messageValue = null;
        jsonResult.TryGetValue("messageValue", out messageValue);
        Debug.Log((string)messageValue);
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest()
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, HandleOnSuccessGetLeaderboard, HandleOnFailedGetLeaderboard);
    }

    void HandleOnSuccessGetLeaderboard(GetLeaderboardResult result)
    {
        Debug.Log("Successful get leaderboard");

        foreach (var entry in result.Leaderboard)
        {
            Debug.Log($"{entry.Position}. Name:{entry.PlayFabId} Score:{entry.StatValue}");
            //Debug.Log($"{entry.Position}. Name:{entry.DisplayName} Score:{entry.StatValue}");
        }
    }

    void HandleOnFailedGetLeaderboard(PlayFabError result)
    {
        Debug.Log("Failed get leaderboard");
        Debug.Log(result.GenerateErrorReport());
    }

    [ContextMenu(nameof(GetPlayerLeaderboard))]
    public void GetPlayerLeaderboard()
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            MaxResultsCount = 1,
            StatisticName = "Score"
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, HandleOnSuccessGetPlayerLeaderboard, HandleOnError);
    }

    void HandleOnSuccessGetPlayerLeaderboard(GetLeaderboardAroundPlayerResult result)
    {
        foreach (var entry in result.Leaderboard)
        {
            //Debug.Log($"{entry.Position}. Name:{entry.PlayFabId} Score:{entry.StatValue}");
            Debug.Log($"{entry.Position + 1}. Name:{entry.DisplayName} Score:{entry.StatValue}");
        }
    }

    string _url;

    public void CreateAccount(string userName, string url)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(url)) return;

        _url = url;

        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = userName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, HandleOnUpdateUserNameSuccess, HandleOnError);
    }

    void HandleOnUpdateUserNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Player can successful changing display name => " + result.DisplayName);

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "player_url", _url }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, HandleOnUpdateDataSuccess, HandleOnError);
    }

    void HandleOnUpdateDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Player can successful update data");
    }

    public void GetPlayerInfo()
    {
        var request = new GetPlayerProfileRequest()
        {
            PlayFabId = _playerID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, HandleOnGetProfileSuccess, HandleOnError);
    }
}