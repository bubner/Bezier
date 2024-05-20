using UnityEngine;

namespace DefaultNamespace
{
    public class MouseInteraction : MonoBehaviour
    {
        private bool clicking;
        private bool mouseOver;

        internal void Update()
        {
            if (clicking)
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            }

            if (mouseOver || clicking)
            {
                GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        internal void OnMouseDown()
        {
            clicking = true;
        }

        internal void OnMouseUp()
        {
            clicking = false;
        }

        internal void OnMouseOver()
        {
            mouseOver = true;
        }

        internal void OnMouseExit()
        {
            mouseOver = false;
        }
    }
}