const route = require('express').Router()
const dayjs = require('dayjs')

const model = require('../model')
const utils = require('../utils')

async function daddyDeduplicate(data) {
    const existHash = (await model.daddy.getInfo.ByHash(data.reduce((result, item) => result.concat(item.hash), []))).map(i => i.hash)
    const markHash = new Set()

    return data.filter(({ hash }) => {
        if (existHash.includes(hash)) return false
        if (markHash.has(hash)) return false

        markHash.add(hash)
        return true
    })
}

async function newsDeduplicate(data) {
    const existHash = (await model.news.getByHash(data.reduce((result, item) => result.concat(item.hash), []))).map(i => i.hash)
    const markHash = new Set()

    return data.filter(({ hash }) => {
        if (existHash.includes(hash)) return false
        if (markHash.has(hash)) return false

        markHash.add(hash)
        return true
    })
}

route.get('/', async (req, res) => {
    const start = dayjs(Number(req.query.start)).format('YYYY-MM-DD HH:mm:00')
    const end = dayjs(Number(req.query.end)).format('YYYY-MM-DD HH:mm:00')

    const data = await model.daddy.getInfo.ByDate(start, end)

    res.send(data)
    res.end()
})

route.post('/', async (req, res) => {
    if (req.protocol !== 'https') {
        res.send({ success: false, msg: 'Please using https protocal' })
        res.end()
        return
    }

    const { data = [], token, type } = req.body || {}

    if (token === 'e92b585d0b9e640eb98af95e2f064c8e') {
        let info = []
        let success = false

        if (type !== 1) {
            info = await daddyDeduplicate(data.map((item) => ({
                ...item,
                type,
                hash: utils.md5(item.link),
                date: dayjs().format('YYYY-MM-DD HH:mm:00'),
            })))

            success = await model.daddy.saveInfo(info)
        } else {
            info = await newsDeduplicate(data.map(item => ({
                link: item.link,
                title: item.content,
                medium: 99999,
                status: 1,
                date: dayjs().format('YYYY-MM-DD HH:mm:00'),
                hash: utils.md5(`99999-${item.link}`),
                tags: '',
                keyword: '--',
            })))

            success = await model.news.saveNews(info)
        }

        res.send({ success: Boolean(success), num: info.length })
    } else res.send({ success: false, msg: 'Token validated was failed' })

    res.end()
})

// fetch('https://local.giphy.com/daddy', {
//     method: 'POST',
//     body: JSON.stringify({ data, token: 'e92b585d0b9e640eb98af95e2f064c8e' }),
//     headers: { 'Content-Type': 'application/json' }
// }).then(res => res.json()).then(data => console.log(data.success))

module.exports = route
