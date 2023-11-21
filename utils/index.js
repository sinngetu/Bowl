const crypto = require('crypto')

exports.setResHeader = function setResHeader(req, res) {
    if (req.headers.origin) {
        res.setHeader('Access-Control-Allow-Origin', req.headers.origin)
        res.setHeader('Access-Control-Allow-Credentials', true)
        res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
        res.setHeader('Access-Control-Allow-Headers', 'content-type, authorization, accept-language')
    }

    res.status(200)
}

exports.md5 = function md5(content) {
    return crypto.createHash('md5').update(content).digest('hex')
}
