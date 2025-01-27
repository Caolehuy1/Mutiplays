using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class PlayerControllerManager : MonoBehaviourPunCallbacks
{
   PhotonView view;
    GameObject controller;

    public int playerTeam;
    private Dictionary<int, int> playerTeams = new Dictionary<int, int>();

    public void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    public void Start()
    {
        if (view.IsMine)
        {
            CreateController();

        }    
    }
    public void CreateController()
    {
      if(  PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            {
            playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log("Player's Team: " + playerTeam);
        }
        AssignPlayerToSpawnArea(playerTeam);
    }
    void AssignPlayerToSpawnArea(int team)
    {
        GameObject SpawnArea1 = GameObject.FindWithTag("SpawnArea1");
        GameObject SpawnArea2 = GameObject.FindWithTag("SpawnArea2");

        if (SpawnArea1 == null || SpawnArea2 == null)
        {
            Debug.LogError("Spawn area not found");
            return;
        }

        Transform spawnPoint = null;

        if (team == 1)
        {
            spawnPoint = SpawnArea1.transform.GetChild(Random.Range(0, SpawnArea1.transform.childCount));
        }
        if (team == 2)
        {
            spawnPoint = SpawnArea2.transform.GetChild(Random.Range(0, SpawnArea2.transform.childCount));
        }
        if (spawnPoint != null)
        {
          controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),spawnPoint.position , spawnPoint.rotation, 0, new object[] { view.ViewID });
            Debug.Log(" Instantiated Player Controller at spawn point");
        }
        else
        {
            Debug.LogError("No avaliable spawn points for team " + team);
        }
            
    }
    void AssignTeamsToAllPlayers()
    {
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Team"))
            {
                int team = (int)player.CustomProperties["Team"];
                playerTeams[player.ActorNumber] = team;
                Debug.Log(player.NickName + "'s Team: " + team);

                AssignPlayerToSpawnArea(team); 
            }
        }
    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AssignTeamsToAllPlayers();
    }

}
