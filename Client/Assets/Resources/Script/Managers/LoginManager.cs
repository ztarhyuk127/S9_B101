using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// �α��� �� ���� �Ŵ���
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
        _loadingText.text = "�α��� �õ���";
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
                    _loadingText.text = "�̸��� �Ǵ� ��й�ȣ�� �߸��Ǿ����ϴ�";
                    _login.gameObject.SetActive(true);
                    _signUp.gameObject.SetActive(true);
                }
                else
                {
                    _loadingText.text = "������ ������ �� �����ϴ�";
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
