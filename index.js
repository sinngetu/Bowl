const fs = require('fs')
const http = require('http')
const https = require('https')

const app = require('express')()
const config = require('./config')

require('./routes/')(app)

http.createServer(app).listen(config.port, () => console.log(`listening on port ${config.port}`))

https.createServer({
    key: fs.readFileSync('./key/server.key'),
    cert: fs.readFileSync('./key/server.crt')
}, app).listen(443, () => console.log('daddy info receiving server online!'))

// app.listen(config.port, () => console.log(`listening on port ${config.port}`))
