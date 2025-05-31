using BattleArena.Parameters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BattleArena.Loader;
using Fusion;

namespace BattleArena.UI
{
    public class UIManager : MonoBehaviour
    {
        public GameObject LobbyWindow;
        public GameObject GameResultWindow;
        public GameObject ConnectedPlayerList;
        public TextMeshProUGUI GameResultTMP;
        public TextMeshProUGUI TimerTMP;

        private TextMeshProUGUI _connectedPlayerTMP;

        private void Awake()
        {
            _connectedPlayerTMP = Resources.Load<TextMeshProUGUI>("ConnectedPlayerTMP");
        }

        private void Update()
        {
            if (GameBootstrapper.Instance.IsPalyerLoading)
            {
                LobbyWindow.SetActive(true);
                TimerTMP.gameObject.SetActive(false);
            }
            else
            {
                LobbyWindow.SetActive(false);
            }
        }

        public void UpdateTimerUI(float remainingTime)
        {
            if (TimerTMP != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                TimerTMP.text = $"{minutes:00}:{seconds:00}";
            }
        }

        public void UpdateGameResults(string winnerName, int winnerPoints)
        {
            GameResultTMP.text = $"Winner is {winnerName}!\nPoints scored: {winnerPoints}";
        }

        public void GetListOfConnectedPlayers()
        {
            foreach (var playerEntry in GameBootstrapper.Instance.SpawnedCharacters)
            {
                var networkObj = playerEntry.Value;
                var playerHealth = networkObj.GetComponent<PlayerHealth>();

                var textInstance = Instantiate(_connectedPlayerTMP, Vector3.zero, Quaternion.identity, ConnectedPlayerList.transform);
                textInstance.name = $"{playerHealth.PlayerNickname}TMP";
                textInstance.text = $"{playerHealth.PlayerNickname}: {playerHealth.KillCount}";

                SubscribeToPlayer(playerHealth);
            }
        }

        public void SubscribeToPlayer(PlayerHealth player)
        {
            player.OnKillCountChanged += UpdateSinglePlayerScore;
        }

        private void UpdateSinglePlayerScore(string nickname, int kills)
        {
            Transform child = ConnectedPlayerList.transform.Find($"{nickname}TMP");
            if (child != null)
            {
                child.GetComponent<TextMeshProUGUI>().text = $"{nickname}: {kills}";
            }
        }

        public void RemovePlayerFromList(PlayerRef player)
        {
            for (int i = 0; i < ConnectedPlayerList.transform.childCount; i++)
            {
                var playerInfo = ConnectedPlayerList.transform.GetChild(i);
                var playerPref = GameBootstrapper.Instance.SpawnedCharacters[player];
                string playerName = playerPref.GetComponent<PlayerHealth>().PlayerNickname;
                if (playerInfo.name == $"{playerName}TMP")
                {
                    Destroy(playerInfo.gameObject);
                }
            }

        }
    }
}
