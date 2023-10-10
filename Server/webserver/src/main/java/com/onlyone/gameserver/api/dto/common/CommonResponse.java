package com.onlyone.gameserver.api.dto.common;


import lombok.Getter;
import lombok.Setter;

@Setter
@Getter
public class CommonResponse<T> {
    String message;
    T body;

}
