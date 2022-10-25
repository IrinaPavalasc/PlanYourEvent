
function initMap() {
    const CONFIGURATION = {
        "mapOptions": { "center": { "lat": 37.4221, "lng": -122.0841 }, "fullscreenControl": true, "mapTypeControl": false, "streetViewControl": true, "zoom": 11, "zoomControl": true, "maxZoom": 22, "mapId": "" },
        "mapsApiKey": "AIzaSyDCiTeZ3sedN1zGMhEwi-ijhpRFdQKnVuI",
        "capabilities": { "addressAutocompleteControl": true, "mapDisplayControl": false, "ctaControl": true }
    };
    const componentForm = [
        'country',
        'locality',
        'location',
        'street_number',
    ];

    const getFormInputElement = (component) => document.getElementById(component + '-input');
    const autocompleteInput = getFormInputElement('location');
    const autocompleteInputCountry = getFormInputElement('country');

    const autocomplete = new google.maps.places.Autocomplete(autocompleteInput, {
        fields: ["address_components", "geometry", "name"],
        types: ["address"],
    });

    const autocompleteCountry = new google.maps.places.Autocomplete(autocompleteInputCountry, {
        fields: ["address_components", "geometry", "name"],
        types: ["country", "locality"], 
    });

    autocomplete.addListener('place_changed', function () {
        const place = autocomplete.getPlace();
        if (!place.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            window.alert('No details available for input: \'' + place.name + '\'');
            return;
        }
        fillInAddress(place);
    });

    autocompleteCountry.addListener('place_changed', function () {
        const placeCountry = autocompleteCountry.getPlace();
        if (!placeCountry.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            window.alert('No details available for input: \'' + placeCountry.name + '\'');
            return;
        }
        fillInAddressCountry(placeCountry);
    });

    function fillInAddress(place) {  // optional parameter
        const addressNameFormat = {
            'street_number': 'short_name',
            'route': 'long_name',
            'locality': 'long_name',
            'country': 'long_name',

        };
        const getAddressComp = function (type) {
            for (const component of place.address_components) {
                if (component.types[0] === type) {
                    return component[addressNameFormat[type]];
                }
            }
            return '';
        };
        getFormInputElement('location').value = getAddressComp('route');
        for (const component of componentForm) {
            // Location field is handled separately above as it has different logic.
            if (component !== 'location') {
                getFormInputElement(component).value = getAddressComp(component);

            }
        }
    }

    function fillInAddressCountry(placeCountry) {  // optional parameter
        const addressNameFormat = {
            'street_number': 'short_name',
            'locality': 'long_name',
            'country': 'long_name',
            
            
            

        };
        const getAddressComp = function (type) {
            for (const component of placeCountry.address_components) {
                if (component.types[0] === type) {
                    return component[addressNameFormat[type]];
                }
            }
            return '';
        };
        
        getFormInputElement('country').value = getAddressComp('country');
        for (const component of componentForm) {
            // Location field is handled separately above as it has different logic.
            if (component !== 'location' && component !== 'street_number') {
                getFormInputElement(component).value = getAddressComp(component);

            }
        }
        
    }

}
