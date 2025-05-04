using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        GameBootstrapper.SetRunner(_runner);
    }
}
