const mysql = require('mysql2')
const config = require('./config')

const pool = mysql.createPool(config.db)

exports.close = function close() { pool.end() }

/**
interface Condition {
 hash?: string | string[]
    date?: {
        start: string
        end: string
    }
}
*/

exports.condition = function condition(condition) {
    if (typeof condition.hash === 'string')
        condition.hash = [condition.hash]

    const hash = condition.hash ? `(${condition.hash.map(hash => `hash=${mysql.escape(hash)}`).join(' OR ')})` : ''
    const date = condition.date ? `date BETWEEN ${mysql.escape(condition.date.start)} AND ${mysql.escape(condition.date.end)}` : ''

    const judge = [hash, date].filter(str => !!str).join(' AND ')

    return judge ? ` WHERE ${judge}` : ''
}

exports.query = async function query(condition) {
    const sql = "SELECT *, DATE_FORMAT(date, '%Y-%m-%d %H:%i:%s') AS time FROM `work_news`" + condition + ' ORDER BY date DESC'

    return new Promise((resolve, reject) => {
        pool.query(sql, (err, result) => {
            err ? reject(err) : resolve(result)
        })
    })
}

exports.add = function add(data) {
    data = Array.isArray(data) ? data : [data]

    const keys = Object.keys(data[0])
    const values = data.map((item) => `(${keys.map(key => mysql.escape(item[key])).join(',')})`).join(',')

    const sql = `INSERT INTO work_news(${keys.map(key => `\`${key}\``).join(',')}) VALUES${values}`

    return new Promise((resolve, reject) => {
        pool.query(sql, (err, result) => {
            err ? reject(err) : resolve(result)
        })
    })
}
