using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemName
{

    public class Item : MonoBehaviour
    {
        [Tooltip("Ÿ�� ����")]
        public enum Type { Healpack, MeleeUp, HealthUp, SpeedUp, SpecialItem };
        public Type type;

        [Tooltip("�������� ������ ���� ������ ���� ����")]
        public int value;
    }
}

