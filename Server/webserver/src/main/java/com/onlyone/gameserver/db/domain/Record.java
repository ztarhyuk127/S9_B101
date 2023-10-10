package com.onlyone.gameserver.db.domain;

import lombok.*;
import org.hibernate.annotations.BatchSize;

import javax.persistence.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Getter
@Setter
@Entity
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Record {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer recordId;

    private Integer recordTime;

    private LocalDateTime recordHistory;

    @BatchSize(size = 100)
    @OneToMany(mappedBy = "record",
            cascade = CascadeType.ALL,
            orphanRemoval = true)
    private List<RecordUser> recordUsers;

    @Override
    public String toString() {
        return "Record{" +
                "recordId=" + recordId +
                ", recordTime=" + recordTime +
                ", recordHistory=" + recordHistory +
                '}';
    }
}
