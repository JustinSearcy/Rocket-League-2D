using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject bluePlayerPanel = null;
    [SerializeField] GameObject orangePlayerPanel = null;

    private int playersJoined = 0;

    private void Awake()
    {
        playersJoined = 0;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        playersJoined++;
        if(playersJoined <= 1)
        {
            pi.transform.SetParent(bluePlayerPanel.transform, false);
            pi.gameObject.GetComponent<PlayerMenu>().blueTeam();
        }
        else
        {
            pi.transform.SetParent(orangePlayerPanel.transform, false);
            pi.gameObject.GetComponent<PlayerMenu>().orangeTeam();
        }
    }
}
