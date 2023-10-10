package com.onlyone.gameserver.api.dto.user;

import lombok.Data;

@Data
public class RegistUserReq {

    String nickname;
    String email;
    String password;

}
