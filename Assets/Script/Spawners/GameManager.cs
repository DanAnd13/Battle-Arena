using BattleArena.Loader;
using BattleArena.Parameters;
using BattleArena.UI;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Loader
{
    public class GameManager : NetworkBehaviour
    {
        [Networked] public float RemainingTime { get; set; }
        //[Networked] public NetworkDictionary<PlayerRef, NetworkObject> SpawnedCharacters => default;
        [HideInInspector]
        public float matchDuration;

        private Dictionary<PlayerRef, PlayerHealth> players = new();
        private bool _matchEnded = false;
        private bool _matchStarted = false;

        public override void Spawned()
        {
            matchDuration = 100f;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_AnnounceNewPlayer(PlayerRef playerRef, NetworkObject playerObject)
        {
            if (!GameBootstrapper.Instance.SpawnedCharacters.ContainsKey(playerRef)) 
            {
                GameBootstrapper.Instance.SpawnedCharacters[playerRef] = playerObject;
            }
            Rpc_UpdatePlayerList();
        }

        public override void FixedUpdateNetwork()
        {
            if (!_matchStarted || _matchEnded)
                return;
            if (Object.HasStateAuthority)
            {
                RemainingTime -= Runner.DeltaTime;

                if (RemainingTime <= 0f)
                {
                    _matchEnded = true;
                    _matchStarted = false;
                    Rpc_StartMatchUI(false);
                    GetPlayersPoints();
                    GetWinner();


                }
            }
            Rpc_UpdateTimerUI(RemainingTime);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_UpdateTimerUI(float time)
        {
            UIManager.Instance.UpdateTimerUI(time);
        }

        public void TryStartMatch()
        {
            if (_matchStarted) return;

            Debug.Log("Match Started!");
            _matchStarted = true;
            _matchEnded = false;
            RemainingTime = matchDuration;
            Rpc_StartMatchUI(true);
            Rpc_UpdatePlayerList();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_StartMatchUI(bool value)
        {
            UIManager.Instance.TimerTMP.gameObject.SetActive(value);
            UIManager.Instance.ConnectedPlayerList.gameObject.SetActive(value);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_UpdatePlayerList()
        {
            UIManager.Instance.GetListOfConnectedPlayers(GameBootstrapper.Instance.SpawnedCharacters);
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
                Rpc_ShowResults(winner.PlayerNickname, winner.KillCount);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_ShowResults(string name, int points)
        {
            StartCoroutine(OpenResultWindow(name, points));
        }

        private IEnumerator OpenResultWindow(string name, int points)
        {
            UIManager.Instance.GameResultWindow.SetActive(true);
            UIManager.Instance.UpdateGameResults(name, points);
            yield return new WaitForSeconds(5f);
            UIManager.Instance.GameResultWindow.SetActive(false);
            GameBootstrapper.Instance.DespawnAllPlayers();
            GameBootstrapper.Instance.RespawnAllPlayers();
        }
    }
}
