using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Linq;
using System.Text;
using System.IO;
using System;
using UnityEngine.Networking;
using Vuforia;
using System.Threading;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity.InputModule;

//This class implements IInputClickHandler to handle the tap gesture.

public class GoBackToMainMenu : MonoBehaviour, IInputClickHandler
{
    public void OnInputClicked(InputClickedEventData eventData)
    {
        SceneManager.LoadScene("1_MainMenu");
    }
}