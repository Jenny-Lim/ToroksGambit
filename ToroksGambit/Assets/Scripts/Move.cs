using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Move : MonoBehaviour
    {
        public int startX;
        public int startY;

        public int endX;
        public int endY;

        public GameObject startObject;
        public GameObject endObject;

        public Move(int x1, int y1, int x2, int y2, GameObject object1, GameObject object2)
        {
            startX = x1;
            startY = y1;
            endX = x2;
            endY = y2;
            startObject = object1;
            endObject = object2;
        }

    }
