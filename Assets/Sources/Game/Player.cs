using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace GGJ2024
{
    public class Player : MonoBehaviour
    {

        [Header("La physique")]
        public float speed;
        public float acceleration;
        public float baseAngle;
        public float dashDuration;
        public AnimationCurve dash;

        private Vector3 targetVelocity;
        private Vector3 currentVelocity;
        private Rigidbody2D rigidbody;
        private float dashCounter = 0;

        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * acceleration) * dash.Evaluate(dashCounter);
            dashCounter += Time.deltaTime;
            if (dashCounter > dashDuration)
            {
                dashCounter = 0;
            }
            //if (buffspeed) currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity * buffspeedmultiplier, Time.deltaTime * acceleration);
            rigidbody.velocity = currentVelocity;
        }

        public void Move(InputAction.CallbackContext context)
        {
            Vector2 newVelocity = context.ReadValue<Vector2>();
            // Velocity
            targetVelocity = new Vector3(newVelocity.x, newVelocity.y, 0) * speed;
            float angle;
            if (context.ReadValue<Vector2>().x == 0 && context.ReadValue<Vector2>().y < 0)
            {
                angle = 0;
            }
            else if (context.ReadValue<Vector2>().x == 0)
            {
                angle = -Mathf.PI;
            }
            else if (context.ReadValue<Vector2>().x > 0)
            {
                angle = Mathf.Atan(context.ReadValue<Vector2>().y / context.ReadValue<Vector2>().x) + Mathf.PI / 2;
            }
            else
            {
                angle = Mathf.Atan(context.ReadValue<Vector2>().y / context.ReadValue<Vector2>().x) - Mathf.PI / 2;
            }

            if (Mathf.Abs(context.ReadValue<Vector2>().x) > 0 && Mathf.Abs(context.ReadValue<Vector2>().y) > 0)
                transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + baseAngle);
            //transform.rotation = Quaternion.Euler(0, 0, 1/Mathf.Sqrt(newVelocity.x * newVelocity.x + newVelocity.y * newVelocity.y));
        }

    }

}
