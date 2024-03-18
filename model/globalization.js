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
        const res = await fetch(url.href, {
            headers: {
                accept: "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
                'accept-language': "zh-CN,zh;q=0.9,en;q=0.8",
                'cache-control': "max-age=0",
                'sec-ch-ua': "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\"",
                'sec-ch-ua-mobile': "?0",
                'sec-ch-ua-platform': "\"Windows\"",
                'sec-fetch-dest': "document",
                'sec-fetch-mode': "navigate",
                'sec-fetch-site': "same-origin",
                'sec-fetch-user': "?1",
                'upgrade-insecure-requests': "1",
                cookie: "xsecappid=xhs-pc-web; gid.sign=tW2tm2DSUThgQ5o8gVF6BdUkbCI=; cache_feeds=[]; abRequestId=636dbeee410e790df3c1cde2875c307f; cacheId=c383c55b-8258-4deb-945e-a01ebd08a5a2; unread={%22ub%22:%22659f44de000000001000f882%22%2C%22ue%22:%2265a20127000000001101c177%22%2C%22uc%22:28}; acw_tc=008647037bf759653850bb9b70b117e3c4d92696f91c7d87a48f2c2facbe1f5e; webBuild=4.6.0; a1=18e50292191d8t9y9hl9jgfs3kvy3cx09g9vmf66650000194159; webId=0a8730db3eb800b3e3b8539ac7f0998a; gid=yYd28JjJ24jfyYd28JjJykiKjyfY9jvjx1jVjKKkiMqThh28vqSC86888yj4y2j8JDj04i8J; web_session=030037a2c48e616a30bd5a9c8f224a7f98178a; websectiga=cf46039d1971c7b9a650d87269f31ac8fe3bf71d61ebf9d9a0a87efb414b816c; sec_poison_id=ef663115-e952-45cd-940d-525edde48c4e"
              },
              referrerPolicy: 'strict-origin-when-cross-origin',
              body: null,
              method: 'GET'
        })

        const id = url.pathname.split('/').pop()
        const text = await res.text()
        const raw = text.split('window.__INITIAL_STATE__=')[1].split('</script>')[0]

        let info
        eval('info = ' + raw)
        const note = info.note.noteDetailMap[id].note

        const platform = '小红书'
        const content = `${note.title} ${note.desc.replaceAll('\n', ' ')}`
        const username = note.user.nickname
        const total = note.interactInfo.commentCount
        const prefix = getPrefix(content)

        return { platform, url: url.href, content, username, total, prefix }
    } catch (e) { return { url, prefix: '. 【】' } }
}

async function WeiBo(url) {
    try {
        const id = url.pathname.split('/').pop()
        const res = await fetch(`https://weibo.com/ajax/statuses/show?id=${id}&locale=zh-CN`, {
            headers: {
                accept: "application/json, text/plain, */*",
                "accept-language": "zh-CN,zh;q=0.9",
                "client-version": "v2.44.23",
                "sec-ch-ua": "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"",
                "sec-ch-ua-mobile": "?0",
                "sec-ch-ua-platform": "\"Windows\"",
                "sec-fetch-dest": "empty",
                "sec-fetch-mode": "cors",
                "sec-fetch-site": "same-origin",
                "server-version": "v2023.11.14.2",
                "x-requested-with": "XMLHttpRequest"
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