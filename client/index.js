const electron = require('electron')
const { app, BrowserWindow } = require('electron')

function createWindow() {
  // 创建浏览器窗口
  const win = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      nodeIntegration: true
    }
  })
  win.setMenu(null)

  // 并且为你的应用加载index.html
  win.loadURL('http://localhost:8080')
}

app.whenReady().then(createWindow)