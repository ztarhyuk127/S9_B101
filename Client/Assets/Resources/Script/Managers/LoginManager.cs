using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// 로그인 씬 제어 매니저
public class LoginManager : HttpController
{
    [SerializeField]
    private TMP_InputField _email;
    [SerializeField]
    private TMP_InputField _password;
    [SerializeField]
    private Button _login;
    [SerializeField]
    private Button _signUp;
    [SerializeField]
    private TMP_Text _loadingText;

    // Start is called before the first frame update

    public void OnClickLogin()
    {
        _email.image.color = Color.white;
        _password.image.color = Color.white;
        if (_email.text == null || _email.text.Length == 0)
        {
            _email.image.color = Color.red;
            _email.Select();
            return;
        }

        if (_password.text == null || _password.text.Length == 0)
        {
            _password.image.color = Color.red;
            _password.Select();
            return;
        }

        LoginRequest data = new LoginRequest() {email = _email.text, password = _password.text };

        _login.gameObject.SetActive(false);
        _signUp.gameObject.SetActive(false);
        _loadingText.text = "로그인 시도중";
        _loadingText.gameObject.SetActive(true);

        StartCoroutine(DoPost<LoginRequest, LoginResponse>(URL + "/user/login", data,
            resp => 
            {
                PlayerPrefs.SetInt("UserId", resp.body.userId);
                PlayerPrefs.SetString("Nickname", resp.body.nickname + "#" + resp.body.userId);

                SceneManager.LoadScene("Lobby");
            },
            error => {
                if (error.code.Equals("E003"))
                {
                    _loadingText.text = "이메일 또는 비밀번호가 잘못되었습니다";
                    _login.gameObject.SetActive(true);
                    _signUp.gameObject.SetActive(true);
                }
                else
                {
                    _loadingText.text = "서버에 연결할 수 없습니다";
                    _login.gameObject.SetActive(true);
                    _signUp.gameObject.SetActive(true);
                }
            }));
    }

    public void OnClickSignUp()
    {
        SceneManager.LoadScene("Regist");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
