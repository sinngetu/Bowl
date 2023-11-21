module.exports = {
    port: 6464,
    DEV: process.argv.includes('--dev'),
    db: {
        host: 'localhost',
        user: 'root',
        password: '',
        database: 'work'
    }
}
