package com.onlyone.gameserver.api.dto.record;

import com.onlyone.gameserver.api.dto.user.RecordUserDto;
import lombok.AllArgsConstructor;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
@AllArgsConstructor
public class FindRecordResp {

    private Integer recordId;
    private Integer recordTime;
    private LocalDateTime recordHistory;
    private List<RecordUserDto> users;

}
