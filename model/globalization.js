const puppeteer = require('puppeteer')
let browser = null

module.exports = async function(urls) {
    if (!Array.isArray(urls)) return ''

    browser = await puppeteer.launch({ headless: true })
    const data = await Promise.all(urls.map(distribute))

    await browser.close()
    browser = null

    return sort(data)
}

async function distribute(link) {
    if (typeof link !== 'string') return { url: link }

    const url = new URL(link)

    switch(url.host) {
        case 'www.xiaohongshu.com': return await XiaoHongShu(url)
        case 'weibo.com': return WeiBo(url)
        case 'www.toutiao.com': return TouTiao(url)
        case 'www.douyin.com': return DouYin(url)
        default: return { url: link, prefix: '. 【】' }
    }
}

function getPrefix(content) {
    const prefixs = [
        { content: '. 【速卖通】', keys: ['速卖通', 'aliexpress', 'AliExpress', 'Aliexpress', 'ALIEXPRESS'] },
        { content: '. 【国际站】', keys: ['国际站'] },
        { content: '. 【Lazada】', keys: ['LAZADA', 'Lazada', 'lazada'] },
        { content: '. 【Miravia】', keys: ['MIRAVIA', 'Miravia', 'miravia'] },
        { content: '. 【Daraz】', keys: ['DARAZ', 'Daraz', 'daraz'] },
    ]

    for(const prefix of prefixs) {
        const hasKey = prefix.keys.reduce((result, key) => result || content.includes(key), false)

        if (hasKey) return prefix.content
    }

    return '. '
}

function sort(data) {
    const mark = {}

    data.forEach(item => {
        const platform = item.platform || '未成功抓取'

        if (!mark[platform])
            mark[platform] = []

        mark[platform].push(item)
    })

    Object.keys(mark).forEach(platform => {
        const topics = {}

        mark[platform].forEach(item => {
            if (!topics[item.prefix])
                topics[item.prefix] = []

            topics[item.prefix].push(item)
        })

        mark[platform] = Object.keys(topics)
            .sort((a, b) => topics[b].length - topics[a].length)
            .map(key => topics[key])
            .flat()
    })

    return mark
}

async function XiaoHongShu(url) {
    try {
        const page = await browser.newPage()
        await page.goto(url, { timeout: 0 })

        let info = await page.evaluate(() => {
            let content, username, total, like

            content = ((document.getElementById('detail-title'))?.innerHTML || '') + ' ' + (document.getElementById('detail-desc'))?.innerText
            username = (document.querySelector('span.username'))?.innerText
            total = (document.querySelectorAll('.engage-bar span.count')[2])?.innerHTML
            like = (document.querySelectorAll('.engage-bar span.count')[0])?.innerHTML

            return ({ content, username, total: String(Number(total) || 0), like: Number(like) ? like : undefined })
        })

        result = { ...info, url: url.href, prefix: getPrefix(info.content), platform: '小红书' }
    } catch (e) { result = { url, prefix: '. 【】' } }

    return result

    // try {
    //     const res = await fetch(url.href, {
    //         headers: {
    //             accept: "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
    //             'accept-language': "zh-CN,zh;q=0.9,en;q=0.8",
    //             'cache-control': "max-age=0",
    //             'sec-ch-ua': "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\"",
    //             'sec-ch-ua-mobile': "?0",
    //             'sec-ch-ua-platform': "\"Windows\"",
    //             'sec-fetch-dest': "document",
    //             'sec-fetch-mode': "navigate",
    //             'sec-fetch-site': "same-origin",
    //             'sec-fetch-user': "?1",
    //             'upgrade-insecure-requests': "1",
    //             cookie: "abRequestId=0b41e10d-2509-526f-bef9-32e6856202f0; a1=18c75d820f0xgn82m9kz4r59bxdvt5iozdtm2kxxn50000642182; webId=547e5018b5360a4ffbf58d10d4f77998; web_session=030037a24d38e7028cb8df2a06224a80a7c518; gid=yYSW2fYJ4qudyYSW2fYJ8fTCi8CkIYJ6jTu49CIF2jDCf628h923lJ888K4JyYJ8dfKqJKjS; xsecappid=xhs-pc-web; cache_feeds=[]; unread={%22ub%22:%2265ebdcaf0000000001028f77%22%2C%22ue%22:%2265e41d11000000000102ac13%22%2C%22uc%22:30}; webBuild=4.8.0; websectiga=6169c1e84f393779a5f7de7303038f3b47a78e47be716e7bec57ccce17d45f99; sec_poison_id=bccf813a-7fe0-4ed4-b3d8-dcb94b10c9ec"
    //           },
    //           referrerPolicy: 'strict-origin-when-cross-origin',
    //           body: null,
    //           method: 'GET'
    //     })

    //     const id = url.pathname.split('/').pop()
    //     const text = await res.text()

    //     const raw = text.split('window.__INITIAL_STATE__=')[1].split('</script>')[0]

    //     let info
    //     eval('info = ' + raw)
    //     const note = info.note.noteDetailMap[id].note

    //     const platform = '小红书'
    //     const content = `${note.title} ${note.desc.replaceAll('\n', ' ')}`
    //     const username = note.user.nickname
    //     const total = note.interactInfo.commentCount
    //     const prefix = getPrefix(content)

    //     return { platform, url: url.href, content, username, total, prefix }
    // } catch (e) { return { url, prefix: '. 【】' } }
}

async function WeiBo(url) {
    try {
        const id = url.pathname.split('/').pop()
        const res = await fetch(`https://weibo.com/ajax/statuses/show?id=${id}&locale=zh-CN`, {
            headers: {
                accept: "application/json, text/plain, */*",
                "accept-language": "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7",
                "client-version": "v2.45.17",
                "priority": "u=1, i",
                "sec-ch-ua": "\"Chromium\";v=\"124\", \"Google Chrome\";v=\"124\", \"Not-A.Brand\";v=\"99\"",
                "sec-ch-ua-mobile": "?0",
                "sec-ch-ua-platform": "\"Windows\"",
                "sec-fetch-dest": "empty",
                "sec-fetch-mode": "cors",
                "sec-fetch-site": "same-origin",
                "server-version": "v2024.05.11.3",
                "x-requested-with": "XMLHttpRequest",

                "x-xsrf-token": "hFTQsCJsQYWGrTUgu1l2u9qs",
                "cookie": "SUB=_2AkMSIvmtf8NxqwFRmfoWzmLqbIl_zA_EieKkfgh2JRMxHRl-yT9vqhQ9tRB6OaLXQg03isO03u1JbN1DXAZpGyAdlO4K; SUBP=0033WrSXqPxfM72-Ws9jqgMF55529P9D9WWklbWhS1a1k2yLzSVC0vcw; SINAGLOBAL=3770158050955.421.1702786717229; UOR=,,tophub.today; ULV=1714978233581:9:2:2:4938983557273.718.1714978233418:1714978034240; XSRF-TOKEN=hFTQsCJsQYWGrTUgu1l2u9qs; WBPSESS=V0zdZ7jH8_6F0CA8c_ussdAiOmKM8lvnqKqJpD8AmmKY3c5vhNjRcFdliIXMx06DzAV5hTIPHBOSiKZ-WplY3yVCWEGAC2o5SI28C4JOUZpOCBxX1MppMm-Y79s0Eycd0S5nDa6NKf-R2R-8P4LjTIx7T1H2VNtHd6OSrN6Er4M=",
            },
            referrer: url,
            referrerPolicy: "strict-origin-when-cross-origin",
            body: null,
            method: "GET",
            mode: "cors",
            credentials: "include"
        })

        const info = await res.json()

        const platform = '微博'
        const content = info.text.replaceAll('\n', ' ').replaceAll(/<img.*?>/g, '')
        const username = info.user.screen_name
        const total = String(info.comments_count)
        const prefix = getPrefix(content)

        return { platform, url: url.href, content, username, total, prefix }
    } catch (e) { return { url, prefix: '. ' } }
}

async function TouTiao(url) {
    let frags = url.pathname.split('/')

    if (frags.pop() === '')
        frags.pop()

    try {
        const type = frags.pop()
        const page = await browser.newPage()
        await page.goto(url, { timeout: 0, waitUntil: 'networkidle2' })

        let info
        if (type === 'video') {
            info = await page.evaluate(() => {
                const content = (document.querySelector('h1')?.lastChild).wholeText.replaceAll('\n', ' ')
                const username = (document.querySelector('a.author-name'))?.innerHTML
                const total = (document.querySelector('.actions-list li:nth-child(2) button span'))?.innerHTML

                return ({ content, username, total: String(Number(total) || 0) })
            })
        } else if (type === 'article') {
            info = await page.evaluate(() => {
                const content = document.getElementsByTagName('h1')[0]?.innerHTML
                const username = document.querySelector('span.name a')?.innerHTML
                const total = document.querySelector('#comment-area .title span')?.innerHTML

                return ({ content, username, total: String(Number(total) || 0) })
            })
        } else {
            info = await page.evaluate(() => {
                const content = document.querySelector('.weitoutiao-html')?.innerText.replaceAll('\n', ' ')
                const username = document.querySelector('a.name')?.innerHTML
                const total = document.querySelector('#comment-area .title span')?.innerHTML

                return ({ content, username, total: String(Number(total) || 0) })
            })
        }

        return { ...info, url: url.href, prefix: getPrefix(info.content), platform: '今日头条' }
    } catch (e) { return { url, prefix: '. 【】' } }
}

const douyinQueue = []
const douyinResolve = {}
async function DouYin(url) {
    douyinQueue.push(url)
    const P = new Promise(r => douyinResolve[url] = r)

    DouYinHandle()
    return P
}

let idle = true
async function DouYinHandle(selfRun = false) {
    if (!idle && !selfRun) return
    if (!douyinQueue.length) return idle = true

    idle = false
    const url = douyinQueue.shift()
    let result

    try {
        const page = await browser.newPage()
        await page.goto(url, { timeout: 0, waitUntil: 'networkidle2' })

        let info = await page.evaluate(() => {
            const type = location.pathname.split('/')[1]
            let content, username, total, like, fans

            if (type === 'note') {
                content = (document.querySelector('h1.gfFMmdEm .j5WZzJdp'))?.innerText
                username = (document.querySelector('.UbblxGZr.wfPNSS3V a.hY8lWHgA .j5WZzJdp'))?.innerText
                total = (document.querySelectorAll('.PSi6D1jN .v3nZSid1 .G0CbEcWs')[1])?.innerHTML
                like = (document.querySelectorAll('.PSi6D1jN .v3nZSid1 .G0CbEcWs')[0])?.innerHTML
                fans = (document.querySelector('.KtZzcbT8 .JWilT3lH'))?.innerHTML
            } else {
                content = (document.querySelector('h1.hE8dATZQ'))?.innerText
                username = (document.querySelector('a.hY8lWHgA .teGknu7j'))?.innerText
                total = (document.querySelectorAll('.xi78nG8b ._BMsHw2S .ofo4bP_8')[1])?.innerHTML
                like = (document.querySelectorAll('.xi78nG8b ._BMsHw2S .ofo4bP_8')[0])?.innerHTML
                fans = (document.querySelector('.KtZzcbT8 .JWilT3lH'))?.innerHTML
            }

            return ({ content, username, total: String(Number(total) || 0), like: Number(like) ? like : undefined, fans })
        })

        result = { ...info, url: url.href, prefix: getPrefix(info.content), platform: '抖音' }
    } catch (e) { result = { url, prefix: '. 【】' } }

    douyinResolve[url](result)
    DouYinHandle(true)
}