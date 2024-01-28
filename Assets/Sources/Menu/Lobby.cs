using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.MultiPlayerGame.Shared;
using UnityEngine.EventSystems;

namespace Unity.MultiPlayerGame.Menu
{
    public class Lobby : MonoBehaviour
    {
        [SerializeField]
        public Button goBtn;

        private void Update()
        {
            if(!goBtn.interactable && PlayerInstance.AreAllPlayersReady())
            {
                goBtn.interactable = true;
            }
            if(goBtn.interactable && !PlayerInstance.AreAllPlayersReady())
            {
                goBtn.interactable = false;
            }
        }


    }
}