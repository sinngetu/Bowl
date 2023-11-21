const route = require('express').Router()
const dayjs = require('dayjs')
const model = require('../model')

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

module.exports = route
