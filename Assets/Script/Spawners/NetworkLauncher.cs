using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleArena.Loader
{
    public class NetworkLauncher : MonoBehaviour
    {
        public NetworkRunner RunnerPrefab;
        public GameBootstrapper GameBootstrapper;

        private NetworkRunner _runner;

        public void StartHost()
        {
            StartGame(GameMode.Host);
        }

        public void JoinGame()
        {
            StartGame(GameMode.Client);
        }

        private async void StartGame(GameMode mode)
        {
            _runner = Instantiate(RunnerPrefab);
            _runner.ProvideInput = true;
            _runner.AddCallbacks(GameBootstrapper);

            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestSession",
                Scene = new NetworkSceneInfo(),
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
            if (_runner.IsStarting)
            {
                GameBootstrapper.IsPalyerLoading = true;
            }
            else
            {
                GameBootstrapper.IsPalyerLoading = false;
            }

            GameBootstrapper.SetRunner(_runner);
        }
    }
}
