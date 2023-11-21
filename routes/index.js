const bodyParser = require('body-parser')

module.exports = function(app) {
    app.use(bodyParser.json())
    app.use(bodyParser.urlencoded({ extended: false }))

    app.all('*', function(req, res, next) {
        if (req.headers.origin) {
            res.setHeader('Access-Control-Allow-Origin', req.headers.origin)
            res.setHeader('Access-Control-Allow-Credentials', true)
            res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
            res.setHeader('Access-Control-Allow-Headers', 'content-type, authorization, accept-language, content-type')
        }

        next()
    })

    app.use('/news', require('./news'))
    app.use('/daddy', require('./daddy'))
    app.use('/common', require('./common'))
    app.use('/hotlist', require('./hotlist'))
}
