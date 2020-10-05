const electron = require('electron')
const { app, BrowserWindow } = require('electron')
const url = require('url')
const path = require('path')

process.env.NODE_ENV = 'production'

function createWindow() {
  // 创建浏览器窗口
  const win = new BrowserWindow({
    width: 1280,
    height: 720,
    webPreferences: {
      nodeIntegration: true
    }
  })
  win.setMenu(null)

  // win.openDevTools({
  //   mode: 'bottom'
  // })

  if (process.env.NODE_ENV === 'production') {
    const startUrl = url.format({
      pathname: path.join(__dirname, './pages/index.html'),
      protocol: 'file:',
      slashes: true
    });
    win.loadURL(startUrl)
  } else {
    console.log('develop env')
    win.loadURL('http://localhost:8080')
    win.openDevTools({
      mode: 'bottom'
    })
  }
}

app.whenReady().then(createWindow)