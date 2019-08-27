<template>
    <div class="overflow-auto">
        <template v-if="searching">
            <b-spinner label="Loading..."></b-spinner>
        </template>
        <template v-else>
            <p class="mt-3">Current Page: {{ currentPage }}</p>
            <b-table
                    id="my-table"
                    :items="items"
                    small
            ></b-table>

            <b-pagination
                    v-model="currentPage"
                    :total-rows="rows"
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
                perPage: 3,
                currentPage: 1,
                rows: 0,
                items: [
                    {id: 1, first_name: 'Fred', last_name: 'Flintstone'},
                    {id: 2, first_name: 'Wilma', last_name: 'Flintstone'},
                    {id: 3, first_name: 'Barney', last_name: 'Rubble'},
                    {id: 4, first_name: 'Betty', last_name: 'Rubble'},
                    {id: 5, first_name: 'Pebbles', last_name: 'Flintstone'},
                    {id: 6, first_name: 'Bamm Bamm', last_name: 'Rubble'},
                    {id: 7, first_name: 'The Great', last_name: 'Gazzoo'},
                    {id: 8, first_name: 'Rockhead', last_name: 'Slate'},
                    {id: 9, first_name: 'Pearl', last_name: 'Slaghoople'}
                ]
            }
        },
        methods: {
            search() {
                let self = this
                self.searching = true
                hotelapi.search("*", self.currentPage, self.perPage, (response) => {
                    let data = response.data
                    self.rows = data['@odata.count']
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
                })
            }
        },
        watch:{
            currentPage: function(newVal, oldVal){
                this.search()
            }
        },
        computed: {
            rows() {
                return this.items.length
            }
        },
        created() {
            let self = this
            self.search()
        }
    }
</script>