var path = require('path');

module.exports = {
    chainWebpack: config => {
        config.resolve.alias.set('components', path.resolve('src/components'))
        config.resolve.alias.set('views', path.resolve('src/views'))
        config.resolve.alias.set('services', path.resolve('src/services'))
        config.resolve.alias.set('api', path.resolve('src/services/api'))
    },
};