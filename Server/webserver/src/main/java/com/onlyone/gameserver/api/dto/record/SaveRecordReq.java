package com.onlyone.gameserver.api.dto.record;

import lombok.Data;

import java.util.List;

@Data
public class SaveRecordReq {

    Integer recordTime;
    List<Integer> userIds;

}
