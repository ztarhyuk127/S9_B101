using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemName
{

    public class Item : MonoBehaviour
    {
        [Tooltip("타입 지정")]
        public enum Type { Healpack, MeleeUp, HealthUp, SpeedUp, SpecialItem };
        public Type type;

        [Tooltip("아이템의 종류와 값을 저장할 변수 선언")]
        public int value;
    }
}

