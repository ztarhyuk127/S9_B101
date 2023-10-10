using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistManager : HttpController
{
    [SerializeField]
    private TMP_InputField _email;
    [SerializeField]
    private TMP_InputField _password;
    [SerializeField]
    private TMP_InputField _passwordCheck;
    [SerializeField]
    private TMP_InputField _nickname;
    [SerializeField]
    private TMP_Text _loadingText;
    [SerializeField]
    private Button _registButton;

    private bool validEmail = false;
    private bool validNickname = false;

    
    // �ڷ�ƾ ��� ���� ���� ������ StartCoroutine���� �ҷ���
    public void OnClickRegist()
    {
        // StopCoroutine�� ����ϱ� ���� �޼ҵ带 ������ �Ҵ�
        StartCoroutine(OnClickRegistCoroutine());
    }

    // ���� ����
    private IEnumerator OnClickRegistCoroutine()
    {
        validEmail = validNickname = true;
        _email.image.color = Color.white;
        _password.image.color = Color.white;
        _passwordCheck.image.color = Color.white;
        _nickname.image.color = Color.white;
        _loadingText.text = "�ű� ȸ���� ��� �� �Դϴ�";
        _loadingText.gameObject.SetActive(true);


        if (!checkEmail())
        {
            _email.image.color = Color.red;
            yield break;
        }

        // �̸��� �ߺ� üũ �ڷ�ƾ�� ��ٸ�
        yield return StartCoroutine(checkEmailDuple());
        if (!validEmail)
        {
            _email.image.color = Color.red;
            yield break;
        }


        if (!checkPassword())
        {
            _password.image.color = Color.red;
            yield break;
        }
        
        if (!checkPasswordCheck())
        {
            _passwordCheck.image.color = Color.red;
            yield break;
        }

        if (!checkNickname())
        {
            _nickname.image.color = Color.red;
            yield break;
        }

        yield return StartCoroutine(checkNicknameDuple());
        if (!validNickname)
        {
            _nickname.image.color = Color.red;
            yield break;
        }

        
        RegistRequest data = new RegistRequest {nickname = _nickname.text, email = _email.text, password = _password.text};

        yield return StartCoroutine(DoPost<RegistRequest, string>(URL + "/user/regist", data,
            resp =>
            {
                _loadingText.text = "ȸ�� ��Ͽ� �����Ͽ����ϴ�!\n�α��� ȭ������ ���ư��ּ���.";
                _registButton.gameObject.SetActive(false);
            },
            err =>
            {
                _loadingText.text = "ȸ�� ��Ͽ� �����߽��ϴ�.\n�����ڿ��� ������ �ּ���.";
            }));
        
    }

    public void OnClickCancel()
    {
        SceneManager.LoadScene("Login");
    }

    private bool checkEmail()
    {
        if (_email.text == null || _email.text.Equals(""))
        {
            _loadingText.text = "�̸����� �Է��� �ּ���!!";
            return false;
        }


        if (!IsValidEmail(_email.text))
        {
            _loadingText.text = "�̸��� ������ �ùٸ��� �ʽ��ϴ�!";
            return false;
        }
        return true;
    }

    IEnumerator checkEmailDuple()
    {
        yield return StartCoroutine(DoGet<string>(URL + "/user/check/email/" + _email.text,
            resp => {
            },
            err =>
            {
                _loadingText.text = "�̹� ���Ե� �̸��� �Դϴ�!";
                validEmail = false;
            }));
    }

    private bool checkPassword()
    {
        if (_password.text == null || _password.text.Equals(""))
        {
            _loadingText.text = "��й�ȣ�� �Է��� �ּ���!!";
            return false;
        }

        if (_password.text.Length < 6)
        {
            _loadingText.text = "��й�ȣ�� 6�ڸ� �̻����� �Է��� �ּ���!!";
        }

        return true;
    }

    private bool checkPasswordCheck()
    {
        if (_passwordCheck.text == null || _passwordCheck.text.Equals(""))
        {
            _loadingText.text = "��й�ȣ Ȯ���� �Է��� �ּ���!!";
            return false;
        }

        if (!_password.text.Equals(_passwordCheck.text))
        {
            _loadingText.text = "�Է��� ��й�ȣ�� �ٸ��ϴ�!!";
            return false;
        }

        return true;
    }

    private bool checkNickname()
    {
        if (_nickname.text == null || _nickname.text.Equals(""))
        {
            _loadingText.text = "�г����� �Է��� �ּ���!!";
            return false;
        }

        if (_nickname.text.Length > 15)
        {
            _loadingText.text = "�г����� 15�� ���Ϸ� �����ּ���!!";
            return false;
        }

        return true;
    }

    IEnumerator checkNicknameDuple()
    {
        yield return StartCoroutine(DoGet<CommonResponse<string>>(URL + "/user/check/email/" + _nickname.text,
            resp => {
            },
            err =>
            {
                _loadingText.text = "�̹� ������� �г��� �Դϴ�!";
                validNickname = false;
            }));
    }

    // �̸��� ���� üũ Regex
    private bool IsValidEmail(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
        return valid;
    }

}
