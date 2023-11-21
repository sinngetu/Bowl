const route = require('express').Router()
const model = require('../model')

route.get('/media', async (req, res) => {
    const data = await model.media.getMedia()

    res.send(data)
    res.end()
})

route.get('/platform', async (req, res) => {
    const data = await model.platform.getPlatform()

    res.send(data)
    res.end()
})

route.get('/keyword', async (req, res) => {
    const data = await model.keyword.getKeyword()

    res.send(data)
    res.end()
})

route.post('/globalization', async (req, res) => {
    const { urls } = req.body || {}

    const data = await model.globalization(urls)

    res.send(data)
    res.end()
})

module.exports = route
