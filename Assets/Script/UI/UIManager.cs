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
        public TextMeshProUGUI ConnectedPlayerTMP;
        public static UIManager Instance { get; private set; }

        private Dictionary<string, int> _connectedPlayers = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (GameBootstrapper.Instance.IsPalyerLoading)
            {
                LobbyWindow.SetActive(true);
                ConnectedPlayerList.SetActive(false);
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

        public void GetListOfConnectedPlayers(Dictionary<PlayerRef, NetworkObject> spawnedCharacters)
        {
            _connectedPlayers.Clear();

            foreach (var playerEntry in spawnedCharacters)
            {
                var networkObj = playerEntry.Value;
                var playerHealth = networkObj.GetComponent<PlayerHealth>();

                if (!_connectedPlayers.ContainsKey(playerHealth.PlayerNickname))
                {
                    _connectedPlayers[playerHealth.PlayerNickname] = playerHealth.KillCount;
                    SubscribeToPlayer(playerHealth);
                }
            }

            UpdatePlayerListDisplay();
        }

        public void SubscribeToPlayer(PlayerHealth player)
        {
            player.OnKillCountChanged += UpdateSinglePlayerScore;
        }

        private void UpdateSinglePlayerScore(string nickname, int kills)
        {
            if (_connectedPlayers.ContainsKey(nickname))
            {
                _connectedPlayers[nickname] = kills;
                UpdatePlayerListDisplay();
            }
        }

        public void RemovePlayerFromList(NetworkObject player)
        {
            string playerName = player.GetComponent<PlayerHealth>().PlayerNickname;
            if (_connectedPlayers.ContainsKey(playerName))
            {
                _connectedPlayers.Remove(playerName);
                UpdatePlayerListDisplay();
            }
        }

        private void UpdatePlayerListDisplay()
        {
            ConnectedPlayerTMP.text = string.Empty;

            foreach (var entry in _connectedPlayers)
            {
                ConnectedPlayerTMP.text += $"{entry.Key}: {entry.Value}\n";
            }
        }
    }
}
