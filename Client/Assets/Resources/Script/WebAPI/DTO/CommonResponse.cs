public class CommonResponse<T>
{
    public string message { get; set; }
    public T body { get; set; }

    public string getMessage()
    {
        return message;
    }
}
