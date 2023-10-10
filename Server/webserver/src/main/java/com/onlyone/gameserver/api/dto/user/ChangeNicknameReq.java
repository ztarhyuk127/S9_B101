package com.onlyone.gameserver.api.dto.user;

import lombok.Data;

@Data
public class ChangeNicknameReq {
    Integer userId;
    String nickname;
}
