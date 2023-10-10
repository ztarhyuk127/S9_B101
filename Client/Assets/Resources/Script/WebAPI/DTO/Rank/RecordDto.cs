using System.Collections.Generic;

public class RecordDto
{
    public int recordId { get; set; }
    public int recordTime { get; set; }
    public string recordHistory { get; set; }
    public List<RecordUserDto> users { get; set; }
}
