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
        [SerializeField] private Sprite chargeSprite1;
        [SerializeField] private Sprite dashSprite1;
        [SerializeField] private Sprite stopSprite1;
        [SerializeField] private Sprite chargeSprite2;
        [SerializeField] private Sprite dashSprite2;
        [SerializeField] private Sprite stopSprite2;

        [Header("Positions")]
        [SerializeField] private List<Transform> positions;


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
                    if (p.InputDevice is Keyboard && !keyboardTaken)
                    {
                        if (p.InputDevice is Keyboard && !keyboardTaken)
                        {
                            GameObject player = playerInputManager.JoinPlayer(i, -1, "Keyboard-Mouse", p.InputDevice).gameObject;
                            MovePlayer(player.transform, i);
                            i++;
                            player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                            AddPlayer(player.GetComponent<PlayerController>(), i);
                            keyboardTaken = true;
                        }
                    } else if (p.InputDevice is Gamepad)
                    {
                        GameObject player = playerInputManager.JoinPlayer(i, -1, "Gamepad", p.InputDevice).gameObject;
                        MovePlayer(player.transform, i);
                        i++;
                        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                        AddPlayer(player.GetComponent<PlayerController>(), i);
                    }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void MovePlayer(Transform player, int pos)
        {
            player.position = positions[pos].position;
        }

        public void AddPlayer(PlayerController playerController, int skin)
        {
            if(skin == 0)
            {
                playerController.SetSprites(chargeSprite1, dashSprite1, stopSprite1);
            }
            else if (skin == 1)
            {
                playerController.SetSprites(chargeSprite2, dashSprite2, stopSprite2);
            }
            else if (skin == 2)
            {
                playerController.SetSprites(chargeSprite1, dashSprite1, stopSprite1);
            }
            else if (skin == 3)
            {
                playerController.SetSprites(chargeSprite1, dashSprite1, stopSprite1);
            }
        }

        public void QuitGame()
        {
            PlayerInstance.players = new List<PlayerInstance>(Enumerable.Repeat<PlayerInstance>(null, 4));
            PlayerInstance.has2PlayerKeyboard = false;
            PlayerInstance.currentPlayerNumber = 0;
        }
    }
}