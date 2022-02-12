using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using SubmarineStandoff;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    
    private NetworkRunner _runner;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(30,30,400,80), "Host"))
            {
                StartGame(GameMode.Host);
            }
            
            if (GUI.Button(new Rect(30,110,400,80), "Client"))
            {
                StartGame(GameMode.Client);
            }
        }
    }
    
    async void StartGame(GameMode mode)
    {
        Debug.Log("Starting game with mode=" + mode);
        
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        float x = 0f;
        float y = 0f;
        if (Input.GetKey(KeyCode.W))
            y = 1f;

        if (Input.GetKey(KeyCode.S))
            y = -1f;

        if (Input.GetKey(KeyCode.A))
            x = -1f;

        if (Input.GetKey(KeyCode.D))
            x = 1f;

        data.direction = new Vector2(x, y);
        input.Set(data);
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined (spawning).  LocalPlayer={runner.LocalPlayer}, isServer={runner.IsServer}, isClient={runner.IsClient}");
        
        Vector3 spawnPosition = new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers)*3,1,0);
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        _spawnedCharacters.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left.");
        
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            Debug.Log($"Despawning player {player}");
            
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to Server, LocalPlayer=" + runner.LocalPlayer);
        
    }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
