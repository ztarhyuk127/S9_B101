package com.onlyone.gameserver.mapper;

import com.onlyone.gameserver.api.dto.user.RecordUserDto;
import com.onlyone.gameserver.api.dto.user.FindUserResp;
import com.onlyone.gameserver.api.dto.user.RegistUserReq;
import com.onlyone.gameserver.api.dto.user.UserResp;
import com.onlyone.gameserver.db.domain.User;
import com.onlyone.gameserver.exception.CustomException;
import com.onlyone.gameserver.exception.CustomExceptionList;
import com.onlyone.gameserver.utils.Encryptor;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Named;
import org.mapstruct.factory.Mappers;

import java.util.ArrayList;
import java.util.stream.Collectors;

@Mapper
public interface UserMapper {
    UserMapper INSTANCE = Mappers.getMapper(UserMapper.class);


    /*
     * ToEntity Methods
     */
    @Mapping(target = "userId", ignore = true)
    @Mapping(source = "password", target="password", qualifiedByName = "encryptPassword")
    User toEntity(RegistUserReq dto);



    /*
     * ToDTO Methods
     */
    UserResp toUserResp(User entity);
    FindUserResp toFindUserResp(User entity);
    RecordUserDto toRecordUserDto(User entity);

    default ArrayList<FindUserResp> toUserRespList(ArrayList<User> entityList) {

        return entityList.stream()
                .map(this::toFindUserResp)
                .collect(Collectors.toCollection(ArrayList::new));
    }

    /*
     * qualify Methods
     */
    @Named("encryptPassword")
    default String encryptPassword(String password) throws CustomException {
        try {
            return Encryptor.encrypt(password);
        } catch (Exception e) {
            throw new CustomException(CustomExceptionList.PASSWORD_ENCRYPTION_FAIL_ERROR);
        }
    }

}
