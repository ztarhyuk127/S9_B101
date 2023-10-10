package com.onlyone.gameserver.exception;

import lombok.Getter;
import lombok.ToString;
import org.springframework.http.HttpStatus;

@Getter
@ToString
public enum CustomExceptionList {

    RUNTIME_EXCEPTION(HttpStatus.BAD_REQUEST, "E001", "잘못된 요청입니다."),
    INTERNAL_SERVER_ERROR(HttpStatus.INTERNAL_SERVER_ERROR, "E002", "서버 오류 입니다."),
    USER_NOT_FOUND_ERROR(HttpStatus.NOT_FOUND, "E003", "존재하지 않는 회원입니다."),
    JOIN_INFO_NOT_EXIST(HttpStatus.NOT_FOUND, "E004", "가입정보가 유효하지 않습니다."),
    NO_AUTHENTICATION_ERROR(HttpStatus.FORBIDDEN, "E005", "접근 권한이 없습니다."),
    PASSWORD_ENCRYPTION_FAIL_ERROR(HttpStatus.INTERNAL_SERVER_ERROR, "E006", "비밀번호 암호화에 실패하였습니다."),
    REGIST_USER_FAIL_ERROR(HttpStatus.BAD_REQUEST, "E007", "신규 회원 등록에 실패하였습니다."),
    SAVE_RECORD_FAIL_ERROR(HttpStatus.BAD_REQUEST, "E008", "기록 저장에 실패하였습니다." ),
    RECORD_EMPTY_USER_ERROR(HttpStatus.BAD_REQUEST, "E010", "기록할 회원 데이터가 없습니다."),
    RECORD_NOT_EXIST_ERROR(HttpStatus.NOT_FOUND, "E011", "기록 데이터가 존재하지 않습니다."),
    NICKNAME_ALREADY_EXIST_ERROR(HttpStatus.BAD_REQUEST, "E012", "닉네임이 이미 존재합니다."),
    EMAIL_ALREADY_EXIST_ERROR(HttpStatus.BAD_REQUEST, "E013", "가입된 이메일이 존재합니다.");

    private final HttpStatus status;
    private final String code;
    private String message;

    CustomExceptionList(HttpStatus status, String code) {
        this.status = status;
        this.code = code;
    }

    CustomExceptionList(HttpStatus status, String code, String message) {
        this.status = status;
        this.code = code;
        this.message = message;
    }



}
