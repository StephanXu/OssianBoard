package com.shadowz.forward.Controller;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.UnsupportedEncodingException;
import javax.servlet.ServletInputStream;
import javax.servlet.http.HttpServletRequest;

public abstract class BaseController {

  /**
   * 获取GZIP解压后的消息体
   *
   * @param request
   * @return
   */
  protected String getRequestBody(HttpServletRequest request) {
    try {
      ServletInputStream inputStream = request.getInputStream();
      BufferedReader reader = new BufferedReader(
        new InputStreamReader(inputStream, "UTF-8")
      );
      StringBuilder sb = new StringBuilder();
      String line;
      while ((line = reader.readLine()) != null) {
        sb.append(line);
      }

      return sb.toString();
    } catch (UnsupportedEncodingException e) {
      e.printStackTrace();
    } catch (IOException e) {
      e.printStackTrace();
    }
    return null;
  }
}
