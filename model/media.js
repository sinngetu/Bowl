const db = require('./db')

const table = () => db('work_media')

exports.getMedia = async function getMedia() { return await table().select('*') }
