package com.onlyone.gameserver.db.repository;

import com.onlyone.gameserver.db.domain.Record;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface RecordRepo extends JpaRepository<Record, Integer> {

    @Query(value =
            "WITH ranked as (SELECT r.record_id as id, ROW_NUMBER() OVER (ORDER BY r.record_time ASC, r.record_id ASC) as number " +
            "FROM record r) " +
            "SELECT CONCAT(ranked.id,' ',ranked.number) " +
            "FROM ranked " +
            "WHERE ranked.number >= GREATEST((SELECT ranked.number FROM ranked WHERE ranked.id = :recordId), 3) - 2 " +
            "AND " +
            "ranked.number <= (SELECT ranked.number FROM ranked WHERE ranked.id = :recordId) + 2 ",
            nativeQuery = true
    )
    List<String> findRecordsRank(Integer recordId);

    Page<Record> findAllByRecordIdInOrderByRecordHistoryDesc(List<Integer> recordIds, Pageable pageable);

    List<Record> findAllByRecordIdInOrderByRecordTimeAscRecordIdAsc(List<Integer> recordIds);

    Record findTop1ByRecordIdInOrderByRecordTimeAscRecordIdAsc(List<Integer> recordIds);

}
