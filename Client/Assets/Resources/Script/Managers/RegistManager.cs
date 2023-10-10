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

    
    // 코루틴 제어를 위해 메인 로직을 StartCoroutine으로 불러옴
    public void OnClickRegist()
    {
        // StopCoroutine을 사용하기 위해 메소드를 변수로 할당
        StartCoroutine(OnClickRegistCoroutine());
    }

    // 메인 로직
    private IEnumerator OnClickRegistCoroutine()
    {
        validEmail = validNickname = true;
        _email.image.color = Color.white;
        _password.image.color = Color.white;
        _passwordCheck.image.color = Color.white;
        _nickname.image.color = Color.white;
        _loadingText.text = "신규 회원을 등록 중 입니다";
        _loadingText.gameObject.SetActive(true);


        if (!checkEmail())
        {
            _email.image.color = Color.red;
            yield break;
        }

        // 이메일 중복 체크 코루틴을 기다림
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
                _loadingText.text = "회원 등록에 성공하였습니다!\n로그인 화면으로 돌아가주세요.";
                _registButton.gameObject.SetActive(false);
            },
            err =>
            {
                _loadingText.text = "회원 등록에 실패했습니다.\n관리자에게 문의해 주세요.";
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
            _loadingText.text = "이메일을 입력해 주세요!!";
            return false;
        }


        if (!IsValidEmail(_email.text))
        {
            _loadingText.text = "이메일 형식이 올바르지 않습니다!";
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
                _loadingText.text = "이미 가입된 이메일 입니다!";
                validEmail = false;
            }));
    }

    private bool checkPassword()
    {
        if (_password.text == null || _password.text.Equals(""))
        {
            _loadingText.text = "비밀번호를 입력해 주세요!!";
            return false;
        }

        if (_password.text.Length < 6)
        {
            _loadingText.text = "비밀번호는 6자리 이상으로 입력해 주세요!!";
        }

        return true;
    }

    private bool checkPasswordCheck()
    {
        if (_passwordCheck.text == null || _passwordCheck.text.Equals(""))
        {
            _loadingText.text = "비밀번호 확인을 입력해 주세요!!";
            return false;
        }

        if (!_password.text.Equals(_passwordCheck.text))
        {
            _loadingText.text = "입력한 비밀번호가 다릅니다!!";
            return false;
        }

        return true;
    }

    private bool checkNickname()
    {
        if (_nickname.text == null || _nickname.text.Equals(""))
        {
            _loadingText.text = "닉네임을 입력해 주세요!!";
            return false;
        }

        if (_nickname.text.Length > 15)
        {
            _loadingText.text = "닉네임은 15자 이하로 정해주세요!!";
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
                _loadingText.text = "이미 사용중인 닉네임 입니다!";
                validNickname = false;
            }));
    }

    // 이메일 형식 체크 Regex
    private bool IsValidEmail(string email)
    {
        bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
        return valid;
    }

}
