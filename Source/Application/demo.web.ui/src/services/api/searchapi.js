import axios from 'axios'


const hotelapi = {
    search(query, page, resultsPerPage, onSuccess, onError, onFinally){
        let ax = axios.create({
            baseURL: "https://codesultingkenshodev.search.windows.net/indexes/demo-store-index/docs",
            
        })
        
        ax.get('/', {
            headers: {
                'api-key': '9B48B3AFD5A1DC83DAA4CE34780302AC',
            },
            params: {
                'api-version': '2019-05-06',
                '$count': true,
                '$top': resultsPerPage,
                '$skip': (resultsPerPage * page) - (resultsPerPage),
                '$filter': 'Enabled eq true', 
                'searchFields': 'Name',
                'search': query 
            }
        }).then(onSuccess).catch(onError).finally(onFinally)
    }
}

export {
    hotelapi
}