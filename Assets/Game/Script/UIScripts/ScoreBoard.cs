using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard instance;
    public Text blueTeamText;
    public Text redTeamText;
    public int blueTeamScore = 0;
    public int RedTeamScore = 0;
    private PhotonView view;
    public void Awake()
    {
        view = GetComponent<PhotonView>();
        instance = this;
    }
    public void PlayerDied(int playerTeam)
    {
        if(playerTeam == 2)
        {
            blueTeamScore++;
        }
        if(playerTeam == 1) 
        {
            RedTeamScore++;
        }
        view.RPC("UpdateScores", RpcTarget.All, blueTeamScore,RedTeamScore);
    }

    [PunRPC]

    public void UpdateScores(int blueScore, int redScore)
    {
        blueTeamScore = blueScore;
        RedTeamScore = redScore;

        blueTeamText.text = blueScore.ToString();
        redTeamText.text = redScore.ToString();
    }
}
