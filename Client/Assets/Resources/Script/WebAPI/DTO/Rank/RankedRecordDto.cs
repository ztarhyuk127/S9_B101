using System;
using System.Collections.Generic;

public class RankedRecordDto
{
    public int recordId {get; set;}
    public int recordRank {get; set;}
    public int recordTime { get; set;}
    public string recordHistory { get; set;}
    public List<RecordUserDto> users { get; set;}
}
