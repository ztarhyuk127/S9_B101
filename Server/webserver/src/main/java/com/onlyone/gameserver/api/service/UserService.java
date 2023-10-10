package com.onlyone.gameserver.api.service;

import com.onlyone.gameserver.db.domain.User;
import com.onlyone.gameserver.db.repository.UserRepo;
import com.onlyone.gameserver.exception.CustomException;
import com.onlyone.gameserver.exception.CustomExceptionList;
import com.onlyone.gameserver.utils.Encryptor;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.stereotype.Service;

import javax.transaction.Transactional;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;

@Service
@Slf4j
@RequiredArgsConstructor
public class UserService {

    private final UserRepo userRepository;

    public User getUser(Integer userId) throws CustomException {
        return userRepository.findById(userId)
                .orElseThrow(() -> new CustomException(CustomExceptionList.USER_NOT_FOUND_ERROR));
    }

    public User loginUser(String email, String password) throws NoSuchAlgorithmException, CustomException {
        return userRepository.findByEmailAndPassword(email, Encryptor.encrypt(password))
                .orElseThrow(() -> new CustomException(CustomExceptionList.USER_NOT_FOUND_ERROR));
    }

    public void registUser(User userInfo) throws CustomException {
        try {
            userRepository.save(userInfo);
        } catch (RuntimeException e) {
            log.error("registUser Error Occurs");
            e.printStackTrace();
            throw new CustomException(CustomExceptionList.REGIST_USER_FAIL_ERROR);
        }
    }

    public ArrayList<User> getUsers(String nickname) {
        return userRepository.findByNicknameContains(nickname)
                .orElse(new ArrayList<>());
    }

    public User changeNickname(Integer userId, String nickname) throws CustomException {

        nicknameDuplicateCheck(nickname);

        User user = userRepository.findById(userId)
                .orElseThrow(() -> new CustomException(CustomExceptionList.USER_NOT_FOUND_ERROR));

        user.setNickname(nickname);
        return userRepository.save(user);
    }

    public void nicknameDuplicateCheck(String nickname) throws CustomException {
        if (userRepository.existsUserByNickname(nickname)) {
            throw new CustomException(CustomExceptionList.NICKNAME_ALREADY_EXIST_ERROR);
        }
    }

    public void emailDuplicateCheck(String email) throws CustomException {
        if (userRepository.existsUserByEmail(email)) {
            throw new CustomException(CustomExceptionList.EMAIL_ALREADY_EXIST_ERROR);
        }
    }

}
