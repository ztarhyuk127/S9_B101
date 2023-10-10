package com.onlyone.gameserver.db.domain;

import lombok.*;

import javax.persistence.*;

@Getter
@Setter
@Entity
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class RecordUser {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    Integer recordUserId;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "record_id")
    Record record;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name = "user_id")
    User user;

}
