const db = require('./db')

const table = () => db('work_keyword')

exports.getKeyword = async function getKeyword() { return await table().select('*') }
exports.addKeyword = async function addKeyword(word, type, extend) { return await table().insert({ word, type, extend }) }
exports.delKeyword = async function delKeyword(id) { return await table().where('id', id).del() }
exports.editKeyword = async function editKeyword(id, word, extend) { return await table().where('id', id).update({ word, extend }) }