using UnityEngine;

namespace GGJ2024 {

    public class GameManager : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private Sprite chargeSprite1;
        [SerializeField] private Sprite dashSprite1;
        [SerializeField] private Sprite stopSprite1;
        [SerializeField] private Sprite chargeSprite2;
        [SerializeField] private Sprite dashSprite2;
        [SerializeField] private Sprite stopSprite2;
        private int players = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void addPlayer(PlayerController playerController)
        {
            players++;
            if(players == 1)
            {
                playerController.SetSprites(chargeSprite1, dashSprite1, stopSprite1);
            }
            else if (players == 2)
            {
                playerController.SetSprites(chargeSprite2, dashSprite2, stopSprite2);
            }
            else if (players == 3)
            {
                playerController.SetSprites(chargeSprite1, dashSprite1, stopSprite1);
            }
            else if (players == 4)
            {
                playerController.SetSprites(chargeSprite1, dashSprite1, stopSprite1);
            }
        }
    }
}