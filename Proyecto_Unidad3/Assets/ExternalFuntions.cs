using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExternalFuntions : MonoBehaviour
{
    public GameObject com, com2, Connection;

    public void Connect() {
        com.SetActive(true);
        com2.SetActive(true);
        Connection.SetActive(false);
    }
    public void ReConnect()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
