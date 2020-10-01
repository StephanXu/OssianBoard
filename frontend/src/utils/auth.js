// import Cookies from 'js-cookie'

const TokenKey = 'Ossian-Token'

export function getToken() {
  return localStorage.getItem(TokenKey)
  // return Cookies.get(TokenKey)
}

export function setToken(token) {
  return localStorage.setItem(TokenKey,token)
  // return Cookies.set(TokenKey, token, { expires: 7 })
}

export function removeToken() {
  return localStorage.removeItem(TokenKey)
  // return Cookies.remove(TokenKey)
}
