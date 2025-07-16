ko.bindingHandlers.map = {
	init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
		var mapObj = ko.utils.unwrapObservable(valueAccessor());
		var latLng = new google.maps.LatLng(
            ko.utils.unwrapObservable(mapObj.lat),
            ko.utils.unwrapObservable(mapObj.lng));

		var mapOptions = {
			center: latLng,
			zoom: 12,
			scaleControl: true,
			zoomControl: true,
			panControl: true,
			mapTypeId: google.maps.MapTypeId.ROADMAP
		};

		google.maps.visualRefresh = true;

		mapObj.googleMap = new google.maps.Map(element, mapOptions);
	}
};