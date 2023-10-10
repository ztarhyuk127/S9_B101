package com.onlyone.gameserver.api.dto.record;

import lombok.Data;

import java.util.List;

@Data
public class PagedRecordsResp {

    int page;
    int size;
    int totalPage;
    int totalSize;
    List<FindRecordResp> records;

}
