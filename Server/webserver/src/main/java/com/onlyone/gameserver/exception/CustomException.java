package com.onlyone.gameserver.exception;

import lombok.Getter;

@Getter
public class CustomException extends Exception {
    private final CustomExceptionList exception;

    public CustomException(CustomExceptionList e) {
        super(e.getMessage());
        this.exception = e;
    }
}
