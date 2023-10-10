package com.onlyone.gameserver.api.service;

import com.onlyone.gameserver.api.dto.common.CommonResponse;
import org.springframework.stereotype.Service;

@Service
public class ResponseService {

    public <T> CommonResponse<T> getResponse(T body) {
        CommonResponse<T> response = new CommonResponse<T>();
        response.setBody(body);
        response.setMessage("success");
        return response;
    }

    public <T> CommonResponse<T> getResponse(T body, boolean isSuccess) {
        CommonResponse<T> response = new CommonResponse<T>();
        response.setBody(body);
        if (isSuccess)
            response.setMessage("success");
        else
            response.setMessage("fail");
        return response;
    }

}
