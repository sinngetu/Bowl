const route = require('express').Router()
const dayjs = require('dayjs')
const model = require('../model')

let weiboList = []

route.get('/', async (req, res) => {
    const start = dayjs(Number(req.query.start)).format('YYYY-MM-DD HH:mm:00')
    const end = dayjs(Number(req.query.end)).format('YYYY-MM-DD HH:mm:00')

    const data = await model.hotlist.getList.ByDate(start, end)

    res.send(data)
    res.end()
})

route.post('/keyword/add', async (req, res) => {
    const { content } = req.body
    const data = await model.keyword.addKeyword(content, 0)

    res.send({ id: data[0] })
    res.end()
})

route.post('/keyword/del', async (req, res) => {
    const { id } = req.body
    const data = await model.keyword.delKeyword(id)

    res.send({ success: Boolean(data) })
    res.end()
})

route.get('/weibo', async (req, res) => {
    res.send(weiboList)
    res.end()
})

route.post('/weibo', async (req, res) => {
    const { data = [], token } = req.body

    if (token === '000de90a486803219f0b59daa6e9a8a1') {
        const rawList = (await model.hotlist.getList.ByHash(data))
        const list = data.map(hash => rawList.find(item => item.hash === hash))

        list.forEach(record => record.date = dayjs(record.date).format('YYYY-MM-DD HH:mm:ss'))
        weiboList = list
        res.send({ success: true })
    } else res.send({ success: false, msg: 'Token validated was failed' })

    res.end()
})

module.exports = route
