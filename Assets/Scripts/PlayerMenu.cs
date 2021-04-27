using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerMenu : MonoBehaviour
{
    [Header("Blue Sprites")] //Blue, Cyan, Green, Purple
    [SerializeField] private List<Sprite> blueOctane = null;

    [Header("Orange Sprites")] //Orange, Red, Pink, Yellow
    [SerializeField] private List<Sprite> orangeOctane = null;


    [SerializeField] private GameObject blueOctaneButton = null;
    [SerializeField] private GameObject orangeOctaneButton = null;

    private bool isBlue = false;

    private void Awake()
    {
        blueOctaneButton = GameObject.Find("Blue Octane");
        orangeOctaneButton = GameObject.Find("Orange Octane");
    }

    public void blueTeam()
    {
        isBlue = true;
        gameObject.GetComponent<Image>().sprite = blueOctane[0];
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(blueOctaneButton);
    }

    public void orangeTeam()
    {
        gameObject.GetComponent<Image>().sprite = orangeOctane[0];
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(orangeOctaneButton);
    }
}
