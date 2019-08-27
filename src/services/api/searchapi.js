import axios from 'axios'


const hotelapi = {
    search(query, page, resultsPerPage, onSuccess, onError, onFinally){
        let ax = axios.create({
            baseURL: "https://codesulting-dev.search.windows.net/indexes/cosmosdb-index/docs",
            
        })
        
        ax.get('/', {
            headers: {
                'api-key': 'A444DE17B5BD1D1157FC0D80C0B95A4B',
            },
            params: {
                'api-version': '2019-05-06',
                '$count': true,
                '$top': resultsPerPage,
                '$skip': (resultsPerPage * page) - (resultsPerPage),
                'search': query
            }
        }).then(onSuccess).catch(onError).finally(onFinally)
    }
}

export {
    hotelapi
}