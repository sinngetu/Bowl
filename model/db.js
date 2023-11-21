const DEVConf = require('../config').DEV ? {
    debug: true,
    log: {debug(message) { console.log(message.sql) }}
} : {}

module.exports = require('knex')({
    client: 'mysql2',
    connection: require('../config').db,
    pool: {
        min: 5,
        max: 20,
        idleTimeoutMillis: 10000,
        acquireTimeoutMillis: 10000
    },

    ...DEVConf
})
