using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChanger : MonoBehaviour
{
    public int Level;
    private PlayerSwitch m_PlayerSwitch;
    [SerializeField] private CinemachineVirtualCamera[] m_CameraList = new CinemachineVirtualCamera[2];
    private int m_ActualCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerSwitch = FindObjectOfType<PlayerSwitch>();
        m_ActualCamera = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Golem g))
        {
            if (m_PlayerSwitch.m_CurrentRoom + 1 < m_PlayerSwitch.Rooms.Count)
            {
                if (m_CameraList.Length != 0 && m_ActualCamera + 1 < m_CameraList.Length)
                {
                    m_CameraList[m_ActualCamera].enabled = false;
                    m_ActualCamera += 1;
                    m_CameraList[m_ActualCamera].enabled = true;
                }
                m_PlayerSwitch.m_CurrentRoom += 1;
                if (Level <= m_PlayerSwitch.m_CurrentRoom)
                    Level = SceneManager.GetActiveScene().buildIndex + 1;
                m_PlayerSwitch.m_CurrentGolem = m_PlayerSwitch.Rooms[m_PlayerSwitch.m_CurrentRoom].Golems.Count-1;
                m_PlayerSwitch.GolemSwitch();
            }
            else
            {
                int sceneNumber = 5;
                if (SceneManager.GetActiveScene().buildIndex + 1 > sceneNumber)
                    SceneManager.LoadScene(0);
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
            PlayerPrefs.SetInt("Level", Level);
            gameObject.SetActive(false);
        }
    }
}
