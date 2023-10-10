package com.onlyone.gameserver.api.controller;

import com.onlyone.gameserver.api.dto.common.CommonResponse;
import com.onlyone.gameserver.api.dto.record.FindRecordResp;
import com.onlyone.gameserver.api.dto.record.RankedRecordDto;
import com.onlyone.gameserver.api.dto.record.SaveRecordReq;
import com.onlyone.gameserver.api.dto.record.SaveRecordResp;
import com.onlyone.gameserver.api.service.RecordService;
import com.onlyone.gameserver.api.service.ResponseService;
import com.onlyone.gameserver.exception.CustomException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@Slf4j
@RequiredArgsConstructor
@RequestMapping("/api/record")
public class RecordController {

    private final RecordService recordService;
    private final ResponseService responseService;

    @PostMapping("/save")
    public ResponseEntity<CommonResponse<Integer>> saveRecord(@RequestBody SaveRecordReq saveRecordReq) throws CustomException {
        log.info("SAVE RECORD API Called!!!");

        Integer response = recordService.saveRecord(saveRecordReq.getRecordTime(), saveRecordReq.getUserIds());

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @GetMapping("/findbyuser")
    public ResponseEntity<CommonResponse<List<FindRecordResp>>> findRecordsByUserId(@RequestParam("userId") Integer userId, @RequestParam("page") Integer page, @RequestParam("size") Integer size) throws CustomException {
        log.info("Find Record By UserId API Called!!");

        List<FindRecordResp> response = recordService.getRecordsByUserIdInPagination(userId, page, size);

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @GetMapping("/rank")
    public ResponseEntity<CommonResponse<List<FindRecordResp>>> findRecords(@RequestParam("page") Integer page, @RequestParam("size") Integer size) {
        log.info("Find Records by Pagination API Called!!!");

        List<FindRecordResp> response = recordService.getRecordsInPagination(page, size);

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @GetMapping("/myrank/{userId}")
    public ResponseEntity<CommonResponse<SaveRecordResp>> findMyBestRecord(@PathVariable Integer userId) throws CustomException {
        log.info("Find My Best Record API Called!!");

        SaveRecordResp response = recordService.getMyBestRecordWithNear(userId);

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }

    @GetMapping("/rank/{recordId}")
    public ResponseEntity<CommonResponse<SaveRecordResp>> findRecordsRank(@PathVariable Integer recordId) throws CustomException {
        log.info("Find Record's Rank API Called!!");

        SaveRecordResp response = recordService.getRecordsNearMyRecord(recordId);

        return new ResponseEntity<>(responseService.getResponse(response), HttpStatus.OK);
    }
}
