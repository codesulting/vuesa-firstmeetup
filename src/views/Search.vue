<template>
    <div class="overflow-auto">
        <b-input-group prepend="Search" class="mt-3">
            <b-form-input ref="search" v-model="query" v-on:keyup.enter="search"></b-form-input>
            <b-input-group-append>
                <b-button variant="success" v-on:click="search">Go</b-button>
            </b-input-group-append>
        </b-input-group>
        <template v-if="searching">
            <b-spinner label="Loading..."></b-spinner>
        </template>
        <template v-else>
            
            <p class="mt-3">Total Results: {{ totalResults }}</p>
            <b-table
                    id="my-table"
                    :items="items"
                    small
            ></b-table>

            <b-pagination
                    v-model="currentPage"
                    :total-rows="totalResults"
                    :per-page="perPage"
                    aria-controls="my-table"
            ></b-pagination>
        </template>


    </div>
</template>

<script>
    import {hotelapi} from 'api/searchapi'

    export default {
        data() {
            return {
                searching: false,
                query: null,
                perPage: 5,
                currentPage: 1,
                totalResults: 0,
                items: []
            }
        },
        methods: {
            search() {
                let self = this
                self.searching = true
                
                let searchTerm = self.query ? self.query : "*"
                
                
                hotelapi.search(searchTerm, self.currentPage, self.perPage, (response) => {
                    let data = response.data
                    self.totalResults = data['@odata.count']
                    self.items = data.value.map((x) => {
                        let address = x.Address
                        return {
                            HotelId: x.HotelId,
                            HotelName: x.HotelName,
                            Rating: x.Rating,
                            Address: address.StreetAddress + " " + address.City + " " + address.StateProvince + " " + address.PostalCode + " " + address.Country
                        }
                    })
                }, (error) => {
                    console.error(error)
                }, () => {
                    self.searching = false
                    self.$refs.search.$el.focus()
                })
            }
        },
        watch:{
            currentPage: function(newVal, oldVal){
                this.search()
            },
        },
        computed: {
            
        },
        created() {
            let self = this
            self.search()
        }
    }
</script>