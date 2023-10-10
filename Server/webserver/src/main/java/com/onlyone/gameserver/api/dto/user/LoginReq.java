package com.onlyone.gameserver.api.dto.user;

import lombok.Data;

@Data
public class LoginReq {
    String email;
    String password;
}
