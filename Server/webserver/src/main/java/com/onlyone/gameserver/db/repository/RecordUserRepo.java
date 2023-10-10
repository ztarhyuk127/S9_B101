package com.onlyone.gameserver.db.repository;

import com.onlyone.gameserver.db.domain.RecordUser;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface RecordUserRepo extends JpaRepository<RecordUser, Integer> {

    @Query(value = "SELECT ru.record_id FROM record_user ru WHERE ru.user_id = :userId",
            nativeQuery = true)
    List<Integer> findRecordsByUserId(Integer userId);

}
