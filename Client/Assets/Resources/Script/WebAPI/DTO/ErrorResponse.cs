public class ErrorResponse
{
    public string message { get; set; }
    public string code { get; set; }

    public string toString()
    {
        return code + " : " + message;
    }
}   
