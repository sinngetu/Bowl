const db = require('./db')

const table = () => db('work_platform')

exports.getPlatform = async function getPlatform() { return await table().select('*') }
