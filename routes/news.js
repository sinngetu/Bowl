const route = require('express').Router()
const dayjs = require('dayjs')
const model = require('../model')

route.get('/', async (req, res) => {
    const params = req.query

    params.tags = (params.tags || []).map(tag => Number(tag))
    params.start = dayjs(Number(params.start)).format('YYYY-MM-DD HH:mm:00')
    params.end = dayjs(Number(params.end)).format('YYYY-MM-DD HH:mm:00')
    params.status = Number(params.status) || 0

    const data = await model.news.getNews(params)

    res.send(data)
    res.end()
})

route.post('/tag', async (req, res) => {
    const { hash, tags } = req.body

    const data = await model.news.updateNews(hash, { tags: tags.join(',') })

    res.send({ success: Boolean(data) })
    res.end()
})

route.post('/tag/add', async (req, res) => {
    const { content, color } = req.body

    if (content.trim()) {
        const data = await model.keyword.addKeyword(content, 1, JSON.stringify(color))

        res.send({ id: data[0] })
    } else res.send({ id: -1 })

    res.end()
})

route.post('/tag/del', async (req, res) => {
    const { id } = req.body

    if (id) {
        const data = await model.keyword.delKeyword(id)
    
        res.send({ success: Boolean(data) })
    } else res.send({ success: false })

    res.end()
})

route.post('/keyword/add', async (req, res) => {
    const { content, type } = req.body

    if (type !== undefined) {
        const data = await model.keyword.addKeyword(content, type)

        res.send({ id: data[0] })
    } else res.send({ id: -1 })

    res.end()
})

route.post('/keyword/del', async (req, res) => {
    const { id } = req.body
    const data = await model.keyword.delKeyword(id)

    res.send({ success: Boolean(data) })
    res.end()
})

route.post('/search/add', async (req, res) => {
    const { word, url } = req.body
    const data = await model.keyword.addKeyword(word, 3, url)

    res.send({ id: data[0] })
    res.end()
})

route.post('/search/edit', async (req, res) => {
    const { id, word, url } = req.body
    const data = await model.keyword.editKeyword(id, word, url)

    res.send({ success: Boolean(data) })
    res.end()
})

route.post('/search/del', async (req, res) => {
    const { id } = req.body
    const data = await model.keyword.delKeyword(id)

    res.send({ success: Boolean(data) })
    res.end()
})

module.exports = route
