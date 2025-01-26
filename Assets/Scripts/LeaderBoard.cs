using UnityEngine;
using System;
using System.Collections;
using Dan.Models;
using TMPro;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;
using Dan.Demo;
using Unity.VisualScripting;

namespace LeaderboardCreatorDemo
{
    public class LeaderBoard : Singleton<LeaderBoard>
    {
        [Header("Gameplay:")]
        [SerializeField] private TextMeshProUGUI _playerScoreText;
        
        [Header("Leaderboard Essentials:")]
        [SerializeField] private TMP_InputField _playerUsernameInput;
        [SerializeField] private Transform _entryDisplayParent;
        [SerializeField] private EntryDisplay _entryDisplayPrefab;
        [SerializeField] private CanvasGroup _leaderboardLoadingPanel;

        [Header("Search Query Essentials:")]
        [SerializeField] private TMP_Dropdown _timePeriodDropdown;
        [SerializeField] private TMP_InputField _pageInput, _entriesToTakeInput;
        [SerializeField] private int _defaultPageNumber = 1, _defaultEntriesToTake = 100;

        [Header("Personal Entry:")]
        [SerializeField] private RectTransform _personalEntryPanel;
        [SerializeField] private TextMeshProUGUI _personalEntryText;

        private int _playerScore;
        
        private Coroutine _personalEntryMoveCoroutine;

        public void AddPlayerScore()
        {
            _playerScore++;
            _playerScoreText.text = $"Your score: {_playerScore}";
        }

        public void PlayerScoreCheck()
        {
            _playerScore = (int)Mathf.Round(GameManager.Instance.bestScore * 100);
            _playerScoreText.text = (Mathf.Round(GameManager.Instance.bestScore * 100)/100).ToString()+"M";
        }
        
        public void Load()
        {
            var timePeriod = 
                _timePeriodDropdown.value == 1 ? Dan.Enums.TimePeriodType.Today :
                _timePeriodDropdown.value == 2 ? Dan.Enums.TimePeriodType.ThisWeek :
                _timePeriodDropdown.value == 3 ? Dan.Enums.TimePeriodType.ThisMonth :
                _timePeriodDropdown.value == 4 ? Dan.Enums.TimePeriodType.ThisYear : Dan.Enums.TimePeriodType.AllTime;

            var pageNumber = int.TryParse(_pageInput.text, out var pageValue) ? pageValue : _defaultPageNumber;
            pageNumber = Mathf.Max(1, pageNumber);
            _pageInput.text = pageNumber.ToString();
            
            var take = int.TryParse(_entriesToTakeInput.text, out var takeValue) ? takeValue : _defaultEntriesToTake;
            take = Mathf.Clamp(take, 1, 100);
            _entriesToTakeInput.text = take.ToString();
            
            var searchQuery = new LeaderboardSearchQuery
            {
                Skip = (pageNumber - 1) * take,
                Take = take,
                TimePeriod = timePeriod
            };
            
            _pageInput.image.color = Color.white;
            _entriesToTakeInput.image.color = Color.white;
            
            Leaderboards.FGJBubbleLeaderBoard.GetEntries(searchQuery, OnLeaderboardLoaded, ErrorCallback);
            ToggleLoadingPanel(true);
        }

        public void ChangePageBy(int amount)
        {
            var pageNumber = int.TryParse(_pageInput.text, out var pageValue) ? pageValue : _defaultPageNumber;
            pageNumber += amount;
            if (pageNumber < 1) return;
            _pageInput.text = pageNumber.ToString();
        }
        
        private void OnLeaderboardLoaded(Entry[] entries)
        {
            foreach (Transform t in _entryDisplayParent) 
                Destroy(t.gameObject);

            foreach (var t in entries) 
                CreateEntryDisplay(t);
            
            ToggleLoadingPanel(false);
        }
        
        private void ToggleLoadingPanel(bool isOn)
        {
            _leaderboardLoadingPanel.alpha = isOn ? 1f : 0f;
            _leaderboardLoadingPanel.interactable = isOn;
            _leaderboardLoadingPanel.blocksRaycasts = isOn;
        }

        public void MovePersonalEntryMenu(float xPos)
        {
            if (_personalEntryMoveCoroutine != null) 
                StopCoroutine(_personalEntryMoveCoroutine);
            _personalEntryMoveCoroutine = StartCoroutine(MoveMenuCoroutine(_personalEntryPanel, 
                new Vector2(xPos, _personalEntryPanel.anchoredPosition.y)));
        }

        private IEnumerator MoveMenuCoroutine(RectTransform rectTransform, Vector2 anchoredPosition)
        {
            const float duration = 0.25f;
            var time = 0f;
            var startPosition = rectTransform.anchoredPosition;
            while (time < duration)
            {
                time += Time.deltaTime;
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, anchoredPosition, time / duration);
                yield return null;
            }
            
            rectTransform.anchoredPosition = anchoredPosition;
            _personalEntryMoveCoroutine = null;
        }
        
        private void CreateEntryDisplay(Entry entry)
        {
            var entryDisplay = Instantiate(_entryDisplayPrefab.gameObject, _entryDisplayParent);
            entryDisplay.GetComponent<EntryDisplay>().SetEntry(entry);
        }

        private IEnumerator LoadingTextCoroutine(TMP_Text text)
        {
            var loadingText = "Loading";
            for (int i = 0; i < 3; i++)
            {
                loadingText += ".";
                text.text = loadingText;
                yield return new WaitForSeconds(0.25f);
            }
            
            StartCoroutine(LoadingTextCoroutine(text));
        }

        private void InitializeComponents()
        {
            StartCoroutine(LoadingTextCoroutine(_leaderboardLoadingPanel.GetComponentInChildren<TextMeshProUGUI>()));

            _pageInput.onValueChanged.AddListener(_ => _pageInput.image.color = Color.yellow);
            _entriesToTakeInput.onValueChanged.AddListener(_ => _entriesToTakeInput.image.color = Color.yellow);

            _pageInput.placeholder.GetComponent<TextMeshProUGUI>().text = _defaultPageNumber.ToString();
            _entriesToTakeInput.placeholder.GetComponent<TextMeshProUGUI>().text = _defaultEntriesToTake.ToString();
        }
        
        private void Start()
        {
            InitializeComponents();
            Load();
        }

        public void Submit()
        {       
            LeaderboardCreator.RequestUserGuid(guid => {
                string newGuid = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("LEADERBOARD_CREATOR___LOCAL_GUID", newGuid);
                PlayerPrefs.Save();
                
            });
            Leaderboards.FGJBubbleLeaderBoard.UploadNewEntry(_playerUsernameInput.text, _playerScore, Callback, ErrorCallback);
        }
        
        public void DeleteEntry()
        {
            Leaderboards.FGJBubbleLeaderBoard.DeleteEntry(Callback, ErrorCallback);
        }

        public void ResetPlayer()
        {
            LeaderboardCreator.ResetPlayer();
        }

        public void GetPersonalEntry()
        {
            Leaderboards.FGJBubbleLeaderBoard.GetPersonalEntry(OnPersonalEntryLoaded, ErrorCallback);
        }

        private void OnPersonalEntryLoaded(Entry entry)
        {
            _personalEntryText.text = $"{entry.RankSuffix()}. {entry.Username} : {entry.Score}";
            MovePersonalEntryMenu(0f);
        }
        
        private void Callback(bool success)
        {
            if (success)
                Load();
        }
        
        private void ErrorCallback(string error)
        {
            Debug.LogError(error);
        }
    }
}