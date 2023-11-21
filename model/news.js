const dayjs = require('dayjs')
const db = require('./db')

const table = () => db('work_news')

exports.saveNews = async (data) => data.length ? await table().insert(data) : []
exports.updateNews = async (hash, info) => info ? await table().where({ hash }).update(info) : []
exports.getNews = async (info) => {
    const current = dayjs().format('YYYY-MM-DD HH:mm:00')
    const { hash, link, medium, title = '', status, tags, start = current, end = current } = info
    const param = { hash, link, medium, status }

    Object.keys(param).forEach(key => {
        const value = param[key]

        if (typeof value === 'undefined') delete param[key]
        if (typeof value === 'string' && value === '') delete param[key]
        if (typeof value === 'number' && value === NaN) delete param[key]
        if (Array.isArray(value) && value.length === 0) delete param[key]
    })

    let data = await table()
        .select('*')
        .limit(1000)
        .where(param)
        .whereILike('title', `%${title}%`)
        .whereBetween('date', [start, end])
        .orderBy('date', 'desc')

    data = data.filter(record => {
        const _tags = (record.tags || '')
            .split(',')
            .map(tag => Number(tag))
            .filter(tag => !!tag)

        let hasTag = true

        if (tags && tags.length)
            hasTag = tags.reduce((result, tag) => result || _tags.includes(tag), false)

        if (!hasTag) return false

        record.tags = _tags
        record.date = dayjs(record.date).format('YYYY-MM-DD HH:mm:ss')
        return true
    })

    return data
}
