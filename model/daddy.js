const dayjs = require('dayjs')
const db = require('./db')

const table = () => db('work_boss')

exports.saveInfo = async (data) => data.length ? await table().insert(data) : []
exports.getInfo = {
    ByHash: async (hash) => hash.length ? await table().whereIn('hash', hash).select('*') : [],
    ByContent: async (keyword) => keyword ? await table().whereLike('content', keyword).select('*') : [],
    ByDate: async (start, end) => {
        if(dayjs(end).valueOf() <= dayjs(start).valueOf()) return []

        const data = await table().select('*').whereBetween('date', [start, end]).orderBy('date', 'desc')

        data.forEach(record => record.date = dayjs(record.date).format('YYYY-MM-DD HH:mm:ss'))
        return data
    }
}
