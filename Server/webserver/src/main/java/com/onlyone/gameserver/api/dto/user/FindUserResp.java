package com.onlyone.gameserver.api.dto.user;

import lombok.Data;

@Data
public class FindUserResp {
    Integer userId;
    String nickname;
}
