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

    try {
        const url = new URL(link)

        switch(url.host) {
            case 'www.xiaohongshu.com': return await XiaoHongShu(url)
            case 'weibo.com': return WeiBo(url)
            case 'www.toutiao.com': return TouTiao(url)
            default: return { url: link, prefix: '. 【】' }
        }
    } catch (e) { return { url: link, prefix: '. 【】' } }
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

    return '. 【】'
}

function sort(data) {
    const mark = {}

    data.forEach(item => {
        const platform = item.platform || '未分类'

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
    const res = await fetch(url.href, {
        headers: {
            accept: 'text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7',
            'accept-language': 'zh-CN,zh;q=0.9',
            'cache-control': 'max-age=0',
            'sec-ch-ua': '"Google Chrome";v="119", "Chromium";v="119", "Not?A_Brand";v="24"',
            'sec-ch-ua-mobile': '?0',
            'sec-ch-ua-platform': '"Windows"',
            'sec-fetch-dest': 'document',
            'sec-fetch-mode': 'navigate',
            'sec-fetch-site': 'same-origin',
            'sec-fetch-user': '?1',
            'upgrade-insecure-requests': '1'
        },
        referrerPolicy: 'strict-origin-when-cross-origin',
        body: null,
        method: 'GET',
        mode: 'cors',
        credentials: 'include'
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
}

async function WeiBo(url) {
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
}

async function TouTiao(url) {
    let frags = url.pathname.split('/')

    if (frags.pop() === '')
        frags.pop()

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
}
