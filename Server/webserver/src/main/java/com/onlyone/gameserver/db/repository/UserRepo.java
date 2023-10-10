package com.onlyone.gameserver.db.repository;

import com.onlyone.gameserver.db.domain.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.ArrayList;
import java.util.Optional;

@Repository
public interface UserRepo extends JpaRepository<User, Integer> {

    public Optional<User> findByEmailAndPassword(String email, String password);

    public Optional<ArrayList<User>> findByNicknameContains(String nickname);

    public boolean existsUserByNickname(String nickname);

    public boolean existsUserByEmail(String email);

}
