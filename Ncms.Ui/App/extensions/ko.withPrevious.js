ko.extenders.withPrevious = function (target) {
    // Define new properties for previous value and whether it's changed
    target.previous = ko.observable();
    target.changed = ko.computed(function () {
    	var p = target.previous() || null,
			t = target() || null;

    	return p !== t;
    });

    // Subscribe to observable to update previous, before change.
    target.subscribe(function (v) {
        target.previous(v);
    }, null, 'beforeChange');

    // Return modified observable
    return target;
}