
public class LoginResponse
{
    public int userId {  get; set; }
    public string nickname { get; set; }

    public string toString()
    {
        return "userId: " + userId + ", nickname: " + nickname;
    }
}
