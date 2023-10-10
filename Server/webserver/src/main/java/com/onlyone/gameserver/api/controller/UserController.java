package com.onlyone.gameserver.api.controller;

import com.onlyone.gameserver.api.dto.common.CommonResponse;
import com.onlyone.gameserver.api.dto.user.*;
import com.onlyone.gameserver.api.service.ResponseService;
import com.onlyone.gameserver.api.service.UserService;
import com.onlyone.gameserver.exception.CustomException;
import com.onlyone.gameserver.mapper.UserMapper;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;

@RestController
@Slf4j
@RequiredArgsConstructor
@RequestMapping("/api/user")
public class UserController {

    private final UserService userService;
    private final ResponseService responseService;

    @PostMapping("/regist")
    public ResponseEntity<CommonResponse<String>> registUser(@RequestBody RegistUserReq newUserInfo) throws CustomException {
        log.info("Regist User API called");

        userService.registUser(UserMapper.INSTANCE.toEntity(newUserInfo));

        return new ResponseEntity<>(responseService.getResponse(null), HttpStatus.OK);
    }

    @PostMapping("/login")
    public ResponseEntity<CommonResponse<UserResp>> login(@RequestBody LoginReq userInfo) throws NoSuchAlgorithmException, CustomException {
        log.info("Login API called");

        UserResp response = UserMapper.INSTANCE.toUserResp(userService.loginUser(userInfo.getEmail(), userInfo.getPassword()));

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @GetMapping("/find/{nickname}")
    public ResponseEntity<CommonResponse<ArrayList<FindUserResp>>> findUsers(@PathVariable String nickname) {
        log.info("Find Nickname API called");

        ArrayList<FindUserResp> response = UserMapper.INSTANCE.toUserRespList(userService.getUsers(nickname));

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @PostMapping("/change/nickname")
    public ResponseEntity<CommonResponse<UserResp>> changeNickname(@RequestBody ChangeNicknameReq changeNicknameReq) throws CustomException {
        log.info("Change Nickname API called");

        UserResp response = UserMapper.INSTANCE.toUserResp(userService.changeNickname(changeNicknameReq.getUserId(), changeNicknameReq.getNickname()));

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @GetMapping("/check/nickname/{nickname}")
    public ResponseEntity<CommonResponse<String>> checkNicknameDuple(@PathVariable String nickname) throws CustomException {
        log.info("Check Nickname Duplicate API called");

        userService.nicknameDuplicateCheck(nickname);

        return new ResponseEntity<>(responseService.getResponse(null), HttpStatus.OK);
    }

    @GetMapping("/check/email/{email}")
    public ResponseEntity<CommonResponse<String>> checkEmailDuple(@PathVariable String email) throws CustomException {
        log.info("Check Email Duplicate API called");

        userService.emailDuplicateCheck(email);

        return new ResponseEntity<>(responseService.getResponse(null), HttpStatus.OK);
    }

}
