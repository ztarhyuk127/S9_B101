package com.onlyone.gameserver.api.service;

import com.onlyone.gameserver.api.dto.record.FindRecordResp;
import com.onlyone.gameserver.api.dto.record.RankedRecordDto;
import com.onlyone.gameserver.api.dto.record.SaveRecordResp;
import com.onlyone.gameserver.db.domain.Record;
import com.onlyone.gameserver.db.domain.RecordUser;
import com.onlyone.gameserver.db.domain.User;
import com.onlyone.gameserver.db.repository.RecordRepo;
import com.onlyone.gameserver.db.repository.RecordUserRepo;
import com.onlyone.gameserver.db.repository.UserRepo;
import com.onlyone.gameserver.exception.CustomException;
import com.onlyone.gameserver.exception.CustomExceptionList;
import com.onlyone.gameserver.mapper.RecordMapper;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;

import javax.transaction.Transactional;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.StringTokenizer;
import java.util.stream.Collectors;

@Service
@Slf4j
@RequiredArgsConstructor
public class RecordService {

    private final RecordRepo recordRepository;
    private final UserRepo userRepository;
    private final RecordUserRepo recordUserRepository;

    public Integer saveRecord(Integer recordTime, List<Integer> userIds) throws CustomException {

        if (userIds == null || userIds.isEmpty()) {
            log.error("RECORDING USER IS EMPTY");
            throw new CustomException(CustomExceptionList.RECORD_EMPTY_USER_ERROR);
        }

        try {
            Record record = Record.builder()
                    .recordTime(recordTime)
                    .recordHistory(LocalDateTime.now())
                    .recordUsers(new ArrayList<>())
                    .build();

            List<User> users = userRepository.findAllById(userIds);

            if (users.size() == 0) {
                throw new CustomException(CustomExceptionList.RECORD_EMPTY_USER_ERROR);
            }

            users.forEach((user) ->
                record.getRecordUsers().add(
                        RecordUser.builder()
                                .user(user)
                                .record(record)
                                .build()
                )
            );

            recordRepository.save(record);

            return record.getRecordId();

        } catch (RuntimeException e) {
            log.error("saveRecord Error Occurs");
            e.printStackTrace();
            throw new CustomException(CustomExceptionList.SAVE_RECORD_FAIL_ERROR);
        }
    }

    public List<FindRecordResp> getRecordsByUserIdInPagination(Integer userId, int page, int size) throws CustomException {

        try {
            List<Integer> recordIds = recordUserRepository.findRecordsByUserId(userId);

            Pageable pageable = PageRequest.of(page, size, Sort.by(Sort.Direction.ASC, "recordTime"));

            List<Record> records = recordRepository.findAllByRecordIdInOrderByRecordHistoryDesc(recordIds, pageable).getContent();

            return RecordMapper.INSTANCE.toFindRecordRespList(records);

        } catch (RuntimeException e) {
            log.error("getRecordsByUserId Error Occurs");
            e.printStackTrace();
            throw new CustomException(CustomExceptionList.USER_NOT_FOUND_ERROR);
        }
    }

    public SaveRecordResp getRecordsNearMyRecord(Integer recordId) throws CustomException {
        try {

            SaveRecordResp response = new SaveRecordResp();
            List<RankedRecordDto> rankedRecordDtoList = new ArrayList<>();
            List<String> idRankList = recordRepository.findRecordsRank(recordId);

            List<Integer> ids = new ArrayList<>();
            List<Integer> ranks = new ArrayList<>();

            for(String idRank : idRankList) {
                StringTokenizer st = new StringTokenizer(idRank);

                ids.add(Integer.parseInt(st.nextToken()));
                ranks.add(Integer.parseInt(st.nextToken()));
            }

            List<Record> records = recordRepository.findAllByRecordIdInOrderByRecordTimeAscRecordIdAsc(ids);

            for(int i=0; i<records.size(); i++) {
                rankedRecordDtoList.add(RecordMapper.INSTANCE.toRankedRecordResp(records.get(i), ranks.get(i)));
            }

            response.setMyRecordId(recordId);
            response.setRecords(rankedRecordDtoList);
            return response;

        } catch (RuntimeException e) {
            log.error("getRecordsNearMyRecord Error Occurs");
            e.printStackTrace();
            throw new CustomException(CustomExceptionList.RECORD_NOT_EXIST_ERROR);
        }
    }

    public List<FindRecordResp> getRecordsInPagination(int page, int size) {
        Pageable pageable = PageRequest.of(page, size, Sort.by(Sort.Direction.ASC, "recordTime"));
        List<Record> records = recordRepository.findAll(pageable).getContent();

        return RecordMapper.INSTANCE.toFindRecordRespList(records);
    }

    public SaveRecordResp getMyBestRecordWithNear(Integer userId) throws CustomException {

        List<Integer> recordIds = recordUserRepository.findRecordsByUserId(userId);

        Record myBestRecord = recordRepository.findTop1ByRecordIdInOrderByRecordTimeAscRecordIdAsc(recordIds);

        return getRecordsNearMyRecord(myBestRecord.getRecordId());
    }

}
