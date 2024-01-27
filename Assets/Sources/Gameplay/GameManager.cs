using System.Collections.Generic;
using System.Linq;
using Unity.MultiPlayerGame.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ2024
{
    public class GameManager : MonoBehaviour
    {
        private PlayerInputManager playerInputManager;
        [Header("Animation")]
        [SerializeField] private Sprite[] chargeSprites;
        [SerializeField] private Sprite[] dashSprites;
        [SerializeField] private Sprite[] stopSprites;

        // Start is called before the first frame update
        void Start()
        {
            if (PlayerInstance.currentPlayerNumber == 0)
            {
                PlayerInstance.players[0] = new PlayerInstance();
                PlayerInstance.players[0].InputDevice = Keyboard.current;
                PlayerInstance.players[0].skin = 0;
            }

            int i = 0;
            playerInputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<PlayerInputManager>();
            bool keyboardTaken = false;
            foreach (PlayerInstance p in PlayerInstance.players)
            {
                if (p is not null)
                {
                    if (p.InputDevice is Keyboard && !keyboardTaken)
                    {
                        if (p.InputDevice is Keyboard && !keyboardTaken)
                        {
                            GameObject player = playerInputManager.JoinPlayer(i, -1, "Keyboard-Mouse", p.InputDevice).gameObject;
                            player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                            AddPlayer(player.GetComponent<PlayerController>());
                            keyboardTaken = true;
                            i++;
                        }
                    }
                    else if (p.InputDevice is Gamepad)
                    {
                        GameObject player = playerInputManager.JoinPlayer(i, -1, "Gamepad", p.InputDevice).gameObject;
                        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                        AddPlayer(player.GetComponent<PlayerController>());
                        i++;
                    }
                }
            }
        }

        public void AddPlayer(PlayerController playerController)
        {
            int id = playerController.ID;
            playerController.SetSprites(chargeSprites[id], dashSprites[id], stopSprites[id]);
        }

        public void QuitGame()
        {
            PlayerInstance.players = new List<PlayerInstance>(Enumerable.Repeat<PlayerInstance>(null, 4));
            PlayerInstance.has2PlayerKeyboard = false;
            PlayerInstance.currentPlayerNumber = 0;
        }
    }
}