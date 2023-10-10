package com.onlyone.gameserver.mapper;

import com.onlyone.gameserver.api.dto.record.FindRecordResp;
import com.onlyone.gameserver.api.dto.record.RankedRecordDto;
import com.onlyone.gameserver.db.domain.Record;
import org.mapstruct.Mapper;
import org.mapstruct.factory.Mappers;

import java.util.List;
import java.util.stream.Collectors;

@Mapper
public interface RecordMapper {

    RecordMapper INSTANCE = Mappers.getMapper(RecordMapper.class);

    default FindRecordResp toFindRecordResp(Record entity) {
        return new FindRecordResp(
                entity.getRecordId(),
                entity.getRecordTime(),
                entity.getRecordHistory(),
                entity.getRecordUsers()
                        .stream()
                        .map((ru) -> UserMapper.INSTANCE.toRecordUserDto(ru.getUser()))
                        .collect(Collectors.toList()));
    }

    default List<FindRecordResp> toFindRecordRespList(List<Record> entityList) {
        return entityList.stream()
                .map(this::toFindRecordResp)
                .collect(Collectors.toList());
    }

    default RankedRecordDto toRankedRecordResp(Record entity, Integer rank) {
        return new RankedRecordDto(
                entity.getRecordId(),
                rank,
                entity.getRecordTime(),
                entity.getRecordHistory(),
                entity.getRecordUsers()
                        .stream()
                        .map((ru) -> UserMapper.INSTANCE.toRecordUserDto(ru.getUser()))
                        .collect(Collectors.toList()));
    }
}
