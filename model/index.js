
exports.close = async () => await require('./db').destroy()

exports.news = require('./news')
exports.media = require('./media')
exports.daddy = require('./daddy')
exports.hotlist = require('./hotlist')
exports.keyword = require('./keyword')
exports.platform = require('./platform')
exports.globalization = require('./globalization')