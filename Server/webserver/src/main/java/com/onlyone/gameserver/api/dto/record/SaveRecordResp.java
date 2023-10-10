package com.onlyone.gameserver.api.dto.record;

import lombok.Data;

import java.util.List;

@Data
public class SaveRecordResp {

    Integer myRecordId;
    List<RankedRecordDto> records;
}
