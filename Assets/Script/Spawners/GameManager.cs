using BattleArena.Loader;
using BattleArena.Parameters;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float RemainingTime;
    [HideInInspector]
    public float matchDuration;
    //public TextMeshProUGUI timerText;

    private Dictionary<PlayerRef, PlayerHealth> players = new();
    private bool _matchEnded = false;
    private bool _matchStarted = false;

    private void Awake()
    {
        matchDuration = 15f;
    }

    private void Update()
    {
        if (!_matchStarted || _matchEnded)
            return;
        
        RemainingTime -= Time.deltaTime;
        Debug.Log(RemainingTime);
            //UpdateTimerUI();

        if (RemainingTime <= 0f)
        {
            _matchEnded = true;
            _matchStarted = false;
            GetPlayersPoints();
            GetWinner();
            GameBootstrapper.Instance.DespawnAllPlayers();
            GameBootstrapper.Instance.RespawnAllPlayers();

        }
    }

    public void TryStartMatch()
    {
        if (_matchStarted) return;

        Debug.Log("Match Started!");
        _matchStarted = true;
        RemainingTime = matchDuration;
    }

    /*private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(RemainingTime / 60f);
            int seconds = Mathf.FloorToInt(RemainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }*/

    private void GetPlayersPoints()
    {
        players.Clear();
        foreach (var player in GameBootstrapper.Instance.SpawnedCharacters)
        {
            PlayerHealth health = player.Value.GetComponent<PlayerHealth>();
            if (health != null)
            {
                players[player.Key] = health;
            }
        }
    }

    private void GetWinner()
    {
        PlayerHealth winner = null;
        int maxKills = -1;

        foreach (var player in players.Values)
        {
            if (player.KillCount > maxKills)
            {
                maxKills = player.KillCount;
                winner = player;
            }
        }

        if (winner != null)
        {
            Debug.Log($"🏆 Winner is {winner.PlayerNickname} with {winner.KillCount} kills!");
        }
        else
        {
            Debug.Log("❗ No winner - all kill counts are 0.");
        }
    }
}
