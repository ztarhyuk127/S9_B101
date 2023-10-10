using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldSystem : MonoBehaviour
{

    [SerializeField] TMP_InputField[] _inputs;
    [SerializeField] Button _trigger;

    // Start is called before the first frame update
    void Start()
    {
        _inputs[0].Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            for(int i=0; i< _inputs.Length - 1; i++)
            {
                if (_inputs[i].isFocused)
                {
                    //_inputs[i].Select();
                    _inputs[i+1].Select();
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _trigger.onClick.Invoke();
        }
    }

}
