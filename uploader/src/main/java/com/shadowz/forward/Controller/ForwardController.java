package com.shadowz.forward.Controller;

import com.microsoft.signalr.HubConnection;
import com.microsoft.signalr.HubConnectionBuilder;
import com.shadowz.forward.Utils.GZIPUtils;
import io.reactivex.Single;
import java.util.ArrayList;
import java.util.List;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.multipart.MultipartFile;

@RestController
public class ForwardController {

  final String url = "http://backend:5000/logger";

  @RequestMapping("/api/offline-log")
  public String forward(
    @RequestParam("file") MultipartFile file,
    @RequestParam("name") String name,
    @RequestParam("description") String description,
    @RequestParam("argumentId") String argumentId
  ) {
    if (file.isEmpty()) {
      return "ERR";
    }
    try {
      byte[] bytes = file.getBytes();
      String str = GZIPUtils.uncompressToString(
        bytes,
        GZIPUtils.GZIP_ENCODE_ISO_8859_1
      );
      String[] strs = str.split("\n");
      HubConnection hubConnection = HubConnectionBuilder.create(url).build();
      hubConnection.start().blockingAwait();
      String logId = "";
      Single<String> invoke = hubConnection.invoke(
        String.class,
        "CreateLog",
        name,
        description,
        argumentId
      );
      logId = invoke.blockingGet();
      List<String> curList = new ArrayList<>();
      for (String curStr : strs) {
        curList.add(curStr);
        if (curList.size() == 500) {
          String[] curSend = curList.toArray(new String[0]);
          hubConnection.send("AddLog", logId, curSend);
          curList.clear();
        }
      }
      String[] curSend = curList.toArray(new String[0]);
      hubConnection.send("AddLog", logId, curSend);
    } catch (Exception ex) {
      ex.printStackTrace();
    }
    return "OK";
  }
}
