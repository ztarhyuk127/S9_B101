package com.onlyone.gameserver.db.domain;

import lombok.*;
import org.hibernate.annotations.ColumnDefault;
import org.mapstruct.Named;

import javax.persistence.*;
import java.util.ArrayList;
import java.util.List;

@Getter
@Setter
@Entity
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer userId;

    @Column(length = 12, unique = true, nullable = false)
    private String nickname;

    @Column(length = 30, unique = true, nullable = false)
    private String email;

    private String password;

}