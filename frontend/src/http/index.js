import axios from 'axios'

axios.defaults.baseURL = '/';

/**
 * 封装get方法
 * @param url
 * @param data
 * @returns {Promise}
 */

export function get(url, params = {}) {
  return new Promise((resolve) => {
    axios.get(url, {
      params: params
    })
      .then(response => {
        resolve(response.data);
      })
      .catch(err => {
        throw(err)
      })
  })
}


/**
 * 封装post请求
 * @param url
 * @param data
 * @returns {Promise}
 */

export function post(url, data = {}) {
  return new Promise((resolve) => {
    axios.post(url, data)
      .then(response => {
        resolve(response.data);
      }, err => {
        throw(err)
      })
  })
}

/**
* 封装patch请求
* @param url
* @param data
* @returns {Promise}
*/

export function patch(url, data = {}) {
  return new Promise((resolve) => {
    axios.patch(url, data)
      .then(response => {
        resolve(response.data);
      }, err => {
        throw(err)
      })
  })
}

/**
* 封装put请求
* @param url
* @param data
* @returns {Promise}
*/

export function put(url, data = {}) {
  return new Promise((resolve) => {
    axios.put(url, data)
      .then(response => {
        resolve(response.data);
      }, err => {
        throw(err)
      })
  })
}