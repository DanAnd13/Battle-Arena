using BattleArena.Loader;
using BattleArena.Parameters;
using BattleArena.UI;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float RemainingTime;
    [HideInInspector]
    public float matchDuration;
    [HideInInspector]
    public UIManager UI;

    private Dictionary<PlayerRef, PlayerHealth> players = new();
    private bool _matchEnded = false;
    private bool _matchStarted = false;

    private void Awake()
    {
        matchDuration = 100f;
        UI = GetComponent<UIManager>();
    }

    private void Update()
    {
        if (!_matchStarted || _matchEnded)
            return;
        
        RemainingTime -= Time.deltaTime;
        UI.UpdateTimerUI(RemainingTime);

        if (RemainingTime <= 0f)
        {
            _matchEnded = true;
            _matchStarted = false;
            UI.TimerTMP.gameObject.SetActive(false);
            GetPlayersPoints();
            GetWinner();
            

        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartMatchUI()
    {
        UI.TimerTMP.gameObject.SetActive(true);
        UI.GetListOfConnectedPlayers(); // можливо, перенеси в окремий RPC, якщо потрібно
    }

    public void TryStartMatch()
    {
        if (_matchStarted) return;

        Debug.Log("Match Started!");
        _matchStarted = true;
        _matchEnded = false;
        RemainingTime = matchDuration;
        Rpc_StartMatchUI();
    }

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
            StartCoroutine(OpenResultWindow(winner.PlayerNickname, winner.KillCount));
        }
    }

    private IEnumerator OpenResultWindow(string name, int points)
    {
        UI.GameResultWindow.SetActive(true);
        UI.UpdateGameResults(name, points);
        yield return new WaitForSeconds(5f);
        UI.GameResultWindow.SetActive(false);
        GameBootstrapper.Instance.DespawnAllPlayers();
        GameBootstrapper.Instance.RespawnAllPlayers();
    }
}
