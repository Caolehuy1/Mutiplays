using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UsernameTeamDisplay : MonoBehaviour
{
    
    public Text usernameText;
    public Text teamText;

    public PhotonView view;

    public void Start()
    {
        if(view.IsMine)
        {
            gameObject.SetActive(false);
        }

        usernameText.text = view.Owner.NickName;


        if(view.Owner.CustomProperties.ContainsKey("Team"))
        {
            int team = (int)view.Owner.CustomProperties["Team"];
            teamText.text = "Team " + team;
        }

    }
}
