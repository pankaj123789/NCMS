var mapUrl = 'https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom=16&size=556x280&maptype=roadmap&markers=color:red|{0},{1}&key=AIzaSyCwRmBZUnlm7MMoIwCVT1uOxx611lfXdSY';

var odAddressVisibilityType = {
	DoNotShow: 1,
	StateOnly: 2,
	StateAndSuburb: 3,
	FullAddress: 4
};

$(function () {
	function setupTable(tableType, setupOptions) {
		var $table = $('#' + tableType.toLowerCase() + 'Table');

		var columns = [];
		for (var i = 0; i < setupOptions.columns.length; i++) {

			var columnName = setupOptions.columns[i].columnName;
			var columnId = tableType.toLowerCase() + columnName + 'Column';
			if ($('#' + columnId).length > 0) {
				columns.push(setupOptions.columns[i]);
			}
		}

		setupOptions.columns = columns;

		var dataTableOptions = $.extend({
			ajax: {
				url: 'PersonalDetails/' + tableType + 'Search',
				type: 'POST'
			},
			columns: [],
			info: false,
			language: {
				zeroRecords: 'No ' + tableType + ' Added'
			},
			ordering: false,
			paging: false,
			searching: false,
			serverSide: true
		}, setupOptions);

		dataTableOptions.columns.push({
			data: null,
			defaultContent:
				'<div class="btn-group pull-right">' +
				'<button class="btn btn-primary" type="button" data-toggle="modal" data-target="#detailModal" data-modal-type="Edit" data-modal-detail="' + tableType + '">Edit</button>' +
				'<button class="btn btn-danger" type="button" data-toggle="modal" data-target="#detailModal" data-modal-type="Delete" data-modal-detail="' + tableType + '">Delete</button>' +
				'</div>',
			render: function (data) {
				var disableDeleteButton = data && data.IsPreferred;
				if (disableDeleteButton) {
					return '<div class="btn-group pull-right">' +
						'<button class="btn btn-primary" type="button" data-toggle="modal" data-target="#detailModal" data-modal-type="Edit" data-modal-detail="' + tableType + '">Edit</button>' +
						'<button disabled="true" class="btn btn-danger" type="button" data-toggle="modal" data-target="#detailModal" data-modal-type="Delete" data-modal-detail="' + tableType + '">Delete</button>' +
						'</div>';
				}

				return this.defaultContent;
			}
		});

		$table.on('xhr.dt', function (event, options, data, xhr) {
			if (xhr.status === 200) {

				$('button[data-modal-type="Add"][data-modal-detail="' + tableType + '"]').prop('disabled', data.disableAddButton);
			}
		});

		var dataTable = $table.DataTable(dataTableOptions);

		$table.find('tbody').on('click', '.btn', function () {
			$('#detailModal').data(tableType.toLowerCase() + 'Index', dataTable.row($(this).parents('tr')).index());
		});


		return $table;
	}

	setupTable('Address',
		{
			columns:
				[
					{
						columnName: '',
						data: function (row, type) {

							var preferred = row.IsPreferred
								? ' <span class="badge" style="background-color: #008c7e;" title="Preferred Contact">Preferred</span>'
								: '';
							var examiner = row.ExaminerCorrespondence && row.IsExaminer
								? ' <span class="badge" style="background-color: #008c7e;" title="Preferred contact for examination tasks">Examiner</span>'
								: '';

							return preferred + examiner;
						},
						className: 'dt-center'
					},
					{
						columnName: 'Address',
						data: function (row, type) {
							var addressHtml = row.StreetDetails + (type === 'display' ? '<br/>' : ' ') + (row.Country === 'Australia' ? row.Suburb : row.Country);
							addressHtml = addressHtml.replace(/,/g, '');
							return addressHtml;
						},
						className: 'dt-center'
					},
					{
						columnName: 'OnlineDirectory',
						data: function (row, type) {
							var selected = row.OdAddressVisibilityTypeName;
							var rowId = row.Id;
							return "<div class='form-group'><div class='dropdown'>" +
								"<select class='form-control' type = 'AddressOnlineDirectoryOptions' rowId = " + rowId + " onchange='OnOdAddressChange(" + rowId + ")'>" +
								"<option value='1'" + (selected == "Do Not Show" ? "selected" : "") + ">Do Not Show</option >" +
								"<option value='2'" + (selected == "State Only" ? "selected" : "") + ">State Only</option >" +
								"<option value='3'" + (selected == "State and Suburb" ? "selected" : "") + ">State and Suburb</option >" +
								"<option value='4'" + (selected == "Full Address" ? "selected" : "") + ">Full Address</option >" +
								"</select></div></div>";
						},
						className: 'dt-center'
					}
				]
		}
	);

	setupTable('Phone',
		{
			columns:
				[
					{
						columnName: '',
						data: function (row, type) {

							var preferred = row.IsPreferred
								? ' <span class="badge" style="background-color: #008c7e;" title="Preferred Contact">Preferred</span>'
								: '';
							var examiner = row.ExaminerCorrespondence && row.IsExaminer
								? ' <span class="badge" style="background-color: #008c7e;" title="Preferred contact for examination tasks">Examiner</span>'
								: '';

							return preferred + examiner;
						},
						className: 'dt-center'
					},
					{
						columnName: 'Phone',
						data: 'PhoneNumber',
						className: 'dt-center'
					},
					{
						columnName: 'OnlineDirectory',
						data: function (row, type) {
							var rowId = row.Id;
							return (row.IsCurrentlyListed ? "<div class='toggle btn btn-xs btn-primary' type='PhoneOnlineDirectoryOptions' data-toggle='toggle' rowid='" + rowId + "' style='width: 40px; height: 22px;'>" :
								"<div class='toggle btn btn-xs btn-default off' type='PhoneOnlineDirectoryOptions' data-toggle='toggle' rowid='" + rowId + "' style='width: 40px; height: 22px;'>") +
								"<input class='toggle-event' type='checkbox' data-id='532' checked='' data-toggle='toggle' data-size='mini' onchange = 'OnOdPhoneChange(" + rowId + ")'>" +
								"<div class='toggle-group' > "+
								"<label class='btn btn-primary btn-xs" + (row.isCurrentlyListedactive ? " active" : "") + " toggle-on' >On</label>" +
								"<label class='btn btn-default btn-xs" + (!row.isCurrentlyListedactive ? " active" : "") + " toggle-off' > Off</label > " +
								"<span class='toggle-handle btn btn-default btn-xs'></span></div ></div > ";
						},
						className: 'dt-center'
					}
				]
		}
	);

	setupTable('Email',
		{
			columns:
				[
					{
						columnName: '',
						data: function (row, type) {
							var preferred = row.IsPreferred
								? ' <span class="badge" style="background-color: #008c7e;" title="The preferred email is used to log into myNAATI, as well as for correspodence">Preferred</span>'
								: '';
							var examiner = row.ExaminerCorrespondence && row.IsExaminer
								? ' <span class="badge" style="background-color: #008c7e;" title="Preferred contact for examination tasks">Examiner</span>'
								: '';

							return preferred + examiner;
						},
						className: 'dt-center'
					},
					{
						columnName: 'Email',
						data: 'Email',
						className: 'dt-center'
					},
					{
						columnName: 'OnlineDirectory',
						data: function (row, type) {
							var rowId = row.Id;
							return (row.IsCurrentlyListed ? "<div class='toggle btn btn-xs btn-primary' type='EmailOnlineDirectoryOptions' data-toggle='toggle' rowid='" + rowId + "' style='width: 40px; height: 22px;'>" :
								"<div class='toggle btn btn-xs btn-default off' type='EmailOnlineDirectoryOptions' data-toggle='toggle' rowid='" + rowId + "' style='width: 40px; height: 22px;'>") +
								"<input class='toggle-event' type='checkbox' data-id='532' checked='' data-toggle='toggle' data-size='mini' onchange = 'OnOdEmailChange(" + rowId + ")'>" +
								"<div class='toggle-group' > " +
								"<label class='btn btn-primary btn-xs" + (row.isCurrentlyListedactive ? " active" : "") + " toggle-on' >On</label>" +
								"<label class='btn btn-default btn-xs" + (!row.isCurrentlyListedactive ? " active" : "") + " toggle-off' > Off</label > " +
								"<span class='toggle-handle btn btn-default btn-xs'></span></div ></div > ";
						},
						className: 'dt-center'
					}
				]
		}
	);

	setupTable('Website',
		{
			columns:
				[
					{
						columnName: '',
						data: function (row, type) {
							return ' <span class="badge" style="background-color: #008c7e;" title="Show in the Online Directory">Directory</span>';
						},
						className: 'dt-center'
					},
					{
						columnName: 'Website',
						data: 'Url',
						className: 'dt-center'
					}
				]
		}
	);

	var $modal = $('#detailModal');
	var $okButton = $modal.find('.modal-footer .btn-primary');
	var $cancelButton = $modal.find('.modal-footer .btn-danger');

	function onModalEvent(eventType, modalType, modalDetail, event) {
		var showFunction = eventType + modalType + modalDetail;

		try {
			new Function('event', showFunction + '(event)')(event);
		} catch (e) {

		}
	}

	function setupModal(event) {
		var $target = $(event.relatedTarget);
		var modalType = $target.data('modalType');
		var modalDetail = $target.data('modalDetail');

		var id = modalType.toLowerCase() + modalDetail;

		$modal.find('.modal-title').text(modalType + ' ' + modalDetail);
		$modal.find('.modal-body').html($('#' + id).html());

		$okButton.attr('onclick', id + '();');
		$okButton.prop('disabled', false);
		$okButton.show();

		$cancelButton.prop('disabled', false);
		$cancelButton.show();

		$modal.data('modalType', modalType);
		$modal.data('modalDetail', modalDetail);

		// Only setup modal event for show, if the modal was opened by clicking a button,
		// Every other call to show the modal is made after an ajax call from another show event
		// So leave this here, and don't call it twice!
		onModalEvent('show', modalType, modalDetail, event);
	}

	$modal.on('show.bs.modal', function (event) {
		$('.validation-summary-errors').empty();

		// Edit modals have the show event called twice (once when clicked, and again after the ajax call to the server to get the edit data)
		// relatedTarget is only set when the modal was opened from clicking a button
		if (event.relatedTarget) {
			setupModal(event);
		}
	});

	$modal.on('shown.bs.modal', function (event) {
		onModalEvent('shown', $modal.data('modalType'), $modal.data('modalDetail'), event);

		$('form[data-ajax=true]').on('submit', function (evt) {
			evt.preventDefault();

			var $form = $(this);
			var validationInfo = $form.data('unobtrusiveValidation');

			if (!(!validationInfo || !validationInfo.validate || validationInfo.validate())) {
				return;
			}

			$form.find('input[type="checkbox"]').each(function () {
				var $checkbox = $(this);
				$checkbox.val($checkbox.prop('checked'));
			});

			var actionButtons = $('#detailModal .modal-footer button');
			actionButtons.attr('disabled', true);
			actionButtons.filter('.btn-primary').text('Processing...');

			function reactivateButtons() {
				actionButtons.attr('disabled', false);
				actionButtons.filter('.btn-primary').text('OK');
			}

			$.ajax({
				url: $form.attr('action'),
				type: $form.attr('method') || 'GET',
				data: $form.serializeArray(),
				success: function (data, status, xhr) {
					reactivateButtons();
					eval($form.data('ajaxSuccess')).call($form, data, status, xhr);
				},
				error: function (xhr, status, error) {
					reactivateButtons();
					eval($form.data('ajaxSuccess')).call($form, xhr, status, error);
				}
			});
		});
	});

	$modal.on('hide.bs.modal', function (event) {
		onModalEvent('hide', $modal.data('modalType'), $modal.data('modalDetail'), event);
	});

	$modal.on('hidden.bs.modal', function (event) {
		onModalEvent('hidden', $modal.data('modalType'), $modal.data('modalDetail'), event);
	});
});

function showAddAddress() {
	var $modal = $('#detailModal');
	// If there are no preferred addresses in the address table then we want to force the next address added to be the preferred address.
	// We do this by checking the IsPreferred check box to true, setting the Value to true, and hiding it from the user
	// (we could not use readonly and disabled because this prevented the PersonDetails controller being sent the IsPreferred flag so instead we use a dummy checkbox for the UI).
	// After hiding the real checkbox from the user we unhide the dummy checkbox which is always disabled and checked in the html to show the user that the address will be
	// marked as preferred.
	// We did not use a Html Helper checkbox in the cshtml to do this as it sends IsPreferred to the controller twice if the check box is ticked due to a hidden field always being created with false upon rendering the checkbox.
	// This was decided to be too messy and unclear for future developers if they checked the network trace and saw two of the same flag being sent to the controller with the same value.
	if (!$('#addressTable').DataTable().rows(function (index, data) {
		return data.IsPreferred;
	}).any()) {
		// set real checkbox values and hide it
		$('#AddressForCreateGoogle_IsPreferred').prop('checked', true);
		$('#AddressForCreateGoogle_IsPreferred').attr("value", "True");
		$('#AddressForCreateGoogle_IsPreferred').prop('hidden', true);
		// unhide the dummy check box for the UI
		$('#hiddenUnlessNoPrefferredAddress').prop('hidden', false);
	}

	$modal.find('.modal-footer .btn-primary').prop('disabled', true);

	$('#contactNaatiAdd').removeClass('alert-info');

	setupGoogleAddressSearch('Add');
}

function shownAddAddress() {
	setGoogleMapsLocation('NAATI National Office', -35.321984, 149.093084);
}

function addAddress() {
	$('#addAddressForm').submit();
}

var currentAddressEdit;

function setAddressFields(address) {
	var country = address.Country;

	$('[id*="_ValidateInExternalTool"]').val(address.ValidateInExternalTool);

	if (country === 'Australia') {
		$('[id*="_IsFromAustralia"]').val('true');
		$('[id*="_StreetDetails"]').val(address.Address);
		$('[id*="_SuburbName"]').val(address.Suburb);
		$('[id*="_Postcode"]').val(address.PostCode);
		$('[id*="_State"]').val(address.State);

	} else {

		var overseasAddress = $('#ValidateByGoogle').val() !== 'true' ? address.Address : address.FullAddress;

		$('[id*="_StreetDetails"]').val(overseasAddress);
		$('[id*="_IsFromAustralia"]').val('false');
		$('[id*="_SuburbName"]').val('');
		$('[id*="_Postcode"]').val('');
		$('[id*="_State"]').val('');
	}

	$('[id*="_CountryName"]').val(country);
}

function OnOdEmailChange(rowId) {
	var includeFlag = false;
	var selectedElement;

	var includeFlag = false;
	var selectedElement;
	var divs = document.querySelectorAll('[type="EmailOnlineDirectoryOptions"]')
	for (i = 0; i < divs.length; ++i) {
		if (divs[i].getAttribute("rowid") == rowId) {
			selectedElement = divs[i];
			includeFlag = divs[i].classList.contains('btn-primary');
		}
	}

	//flip the selection
	includeFlag = !includeFlag;

	$.ajax({
		url: 'PersonalDetails/SetOdEmailPreference',
		type: 'post',
		data: {
			id: rowId,
			include: includeFlag
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}
			if (includeFlag) {
				selectedElement.classList.add('btn-primary');
				selectedElement.classList.remove('btn-default');
				selectedElement.classList.remove('off');
			}
			else {
				selectedElement.classList.add('btn-default');
				selectedElement.classList.add('off');
				selectedElement.classList.remove('btn-primary');
			}
			toastr.success('Preferences have been saved.');
		},
		error: requestError
	});
}

function OnOdPhoneChange(rowId) {
	var includeFlag = false;
	var selectedElement;
	var divs = document.querySelectorAll('[type="PhoneOnlineDirectoryOptions"]')
	for (i = 0; i < divs.length; ++i) {
		if (divs[i].getAttribute("rowid") == rowId) {
			selectedElement = divs[i];
			includeFlag = divs[i].classList.contains('btn-primary');
		}
	}

	//flip the selection
	includeFlag = !includeFlag;

	$.ajax({
		url: 'PersonalDetails/SetOdPhonePreference',
		type: 'post',
		data: {
			id: rowId,
			include: includeFlag
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}
			if (includeFlag) {
				selectedElement.classList.add('btn-primary');
				selectedElement.classList.remove('btn-default');
				selectedElement.classList.remove('off');
			}
			else {
				selectedElement.classList.add('btn-default');
				selectedElement.classList.add('off');
				selectedElement.classList.remove('btn-primary');
			}
			toastr.success('Preferences have been saved.');
		},
		error: requestError
	});
}

function OnOdAddressChange(rowId) {
	var addressDropDowns = $('select[type=AddressOnlineDirectoryOptions]');
	var selectedElement;
	for (i = 0; i < addressDropDowns.length; ++i) {
		if (addressDropDowns[i].getAttribute("rowid") == rowId) {
			selectedElement = addressDropDowns[i];
		}
	}

	var selectedIndex = selectedElement.value;

	//if selecting is not 'do not show' then set all other to 'do not show'
	if (selectedIndex >= 1) {
		for (i = 0; i < addressDropDowns.length; ++i) {
			if (addressDropDowns[i].getAttribute("rowid") != rowId) {
				addressDropDowns[i].value = 1;
			}
		}
	}

	$.ajax({
		url: 'PersonalDetails/SetOdAddressPreference',
		type: 'post',
		data: {
			id: rowId,
			selectedIndex: selectedIndex
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}
			toastr.success('Preferences have been saved.');
		},
		error: requestError
	});
}

function showEditAddress(event) {
	event.preventDefault();

	var $modal = $('#detailModal');
	var $okButton = $modal.find('.modal-footer .btn-primary');
	var $form = $('#editAddressForm');

	$okButton.prop('disabled', true);
	$form.hide();

	$('#contactNaatiEdit').removeClass('alert-info');

	$.ajax({
		type: 'GET',
		cache: false,
		url: 'PersonalDetails/AddressEditJson/' + getTableRowData($('#addressTable'), $modal.data('addressIndex'))['Id'],
		success: function (data) {
			if (!data.Success) {
				showErrorModal('<p>' + concatStringArray(data.Errors) + '</p>');

				return;
			}

			setAddressFields({
				Country: data.CountryName,
				Address: data.StreetDetails,
				Suburb: data.SuburbName,
				PostCode: data.Postcode,
				State: data.State,
				FullAddress: data.FullAddress,
				ValidateInExternalTool: data.ValidateInExternalTool
			});


			$('#AddressForEditGoogle_Id').val(data.Id);
			$('#googleAddressSearchEdit').val(data.FullAddress);
			$('#AddressForEditGoogle_IsPreferred').prop('checked', data.IsPreferred);
			$('#AddressForEditGoogle_OdAddressVisibilityTypeId').val(data.OdAddressVisibilityTypeId);
			$('#AddressForEditGoogle_ExaminerCorrespondence').prop('checked', data.ExaminerCorrespondence);

			$('#AddressForEditGoogle_IsPreferred,#AddressForEditGoogle_ExaminerCorrespondence,#AddressForEditGoogle_OdAddressVisibilityTypeId').change(function () {
				$okButton.prop('disabled', false);
			});

			//for testing Ticket 196330
			//data.Latitude = null;
			//data.Longitude = null;

			if (data.Latitude == null && data.Longitude == null)
			{
				//csharp call to Google GeoCode didnt work. Call it again
				var newLatLong = getLatAndLongFromAddress(data.FullAddress).then(function (result) {
					data.Latitude = result.Latitude;
					data.Longitude = result.Longitude;

					currentAddressEdit = {
						FullAddress: data.FullAddress,
						Latitude: data.Latitude,
						Longitude: data.Longitude
					};

					if (data.OdAddressVisibilityTypeId !== odAddressVisibilityType.DoNotShow) {
						$('#addressIsCurrentlyListed').show();
					}

					$form.show();
					$modal.modal('show');
				});
				return;
            };

			currentAddressEdit = {
				FullAddress: data.FullAddress,
				Latitude: data.Latitude,
				Longitude: data.Longitude
			};

			if (data.OdAddressVisibilityTypeId !== odAddressVisibilityType.DoNotShow) {
				$('#addressIsCurrentlyListed').show();
			}

			$form.show();
			$modal.modal('show');
		},
		error: requestError
	});

	setupGoogleAddressSearch('Edit');
}

function getLatAndLongFromAddress(fullAddress) {
	var baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=" + fullAddress;
	var url = baseUrl + "&key=AIzaSyCwRmBZUnlm7MMoIwCVT1uOxx611lfXdSY";
	return new Promise(function (resolve, reject) {
		$.ajax({
			url: url,
			type: 'get',
			success: function (result) {
				if (result.ErrorMessage) {
					showErrorModal('<p>' + result.ErrorMessage + '</p>');
					reject;
				}

				resolve({ Latitude: result.results[0].geometry.location.lat, Longitude: result.results[0].geometry.location.lng });

			},
			error: requestError
		});
	});
}

function shownEditAddress() {
	setTimeout(function () {
		setGoogleMapsLocation(currentAddressEdit.FullAddress, currentAddressEdit.Latitude, currentAddressEdit.Longitude);
	}, 500);
}

function editAddress() {

	$('#editAddressForm').submit();
}

function showDeleteAddress() {
	var $modal = $('#detailModal');
	var data = getTableRowData($('#addressTable'), $modal.data('addressIndex'));

	if (data['IsLastContactInPD']) {
		$modal.find('p[data-message="lastInPD"]').show();
	} else if (data['OdAddressVisibilityTypeId'] !== odAddressVisibilityType.DoNotShow) {
		$modal.find('p[data-message="inPD"]').show();
	} else {
		$modal.find('p[data-message="generic"]').show();
	}
}

function deleteAddress() {
	var $modal = $('#detailModal');
	var $addressTable = $('#addressTable');

	$.ajax({
		url: 'PersonalDetails/DeleteAddress',
		type: 'post',
		data: {
			id: getTableRowData($addressTable, $modal.data('addressIndex'))['Id']
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}

			loadTableData($addressTable);
			$modal.modal('hide');
		},
		error: requestError
	});
}

function showAddPhone() {
	if (!$('#phoneTable').DataTable().rows(function (index, data) {
		return data.IsPreferred;
	}).any()) {
		$('#PhoneForCreate_IsPreferred').prop('checked', true);
	}

	$('#detailModal .modal-dialog').addClass('modal-sm');
}

function hiddenAddPhone() {
	$('#detailModal .modal-dialog').removeClass('modal-sm');
}

function addPhone() {
	$('#addPhoneForm').submit();
}

function showEditPhone(event) {
	event.preventDefault();

	var $modal = $('#detailModal');
	var $form = $('#editPhoneForm');
	var $okButton = $modal.find('.modal-footer .btn-primary');
	$okButton.prop('disabled', true);
	$modal.find('.modal-dialog').addClass('modal-sm');

	$form.hide();

	$.ajax({
		type: 'GET',
		cache: false,
		url: 'PersonalDetails/PhoneEditJson/' + getTableRowData($('#phoneTable'), $modal.data('phoneIndex'))['Id'],
		success: function (data) {
			if (!data.Success) {
				showErrorModal('<p>' + concatStringArray(data.Errors) + '</p>');

				return;
			}

			$('#PhoneForEdit_Id').val(data.Id);
			$('#PhoneForEdit_CountryCode').val(data.CountryCode);
			$('#PhoneForEdit_AreaCode').val(data.AreaCode);
			$('#PhoneForEdit_Number').val(data.Number);
			$('#PhoneForEdit_IsPreferred').prop('checked', data.IsPreferred);
			$('#PhoneForEdit_IsCurrentlyListed').prop('checked', data.IsCurrentlyListed);
			$('#PhoneForEdit_ExaminerCorrespondence').prop('checked', data.ExaminerCorrespondence);

			$('#PhoneForEdit_Number').keydown(function () {
				$okButton.prop('disabled', false);
			});

			$('#PhoneForEdit_ExaminerCorrespondence,#PhoneForEdit_IsPreferred,#PhoneForEdit_IsCurrentlyListed').change(function () {
				$okButton.prop('disabled', false);
			});

			if (data.IsCurrentlyListed) {
				$('#phoneIsCurrentlyListed').show();
			}

			$form.show();
			$modal.modal('show');
		},
		error: requestError
	});
}

function hiddenEditPhone() {
	$('#detailModal .modal-dialog').removeClass('modal-sm');
}

function editPhone() {
	$('#editPhoneForm').submit();
}

function showDeletePhone() {
	var $modal = $('#detailModal');
	var data = getTableRowData($('#phoneTable'), $modal.data('phoneIndex'));

	if (data['IsLastContactInPD']) {
		$modal.find('p[data-message="lastInPD"]').show();
	} else if (data['IsCurrentlyListed']) {
		$modal.find('p[data-message="inPD"]').show();
	} else {
		$modal.find('p[data-message="generic"]').show();
	}
}

function deletePhone() {
	var $modal = $('#detailModal');
	var $phoneTable = $('#phoneTable');

	$.ajax({
		url: 'PersonalDetails/DeletePhone',
		type: 'post',
		data: {
			id: getTableRowData($phoneTable, $modal.data('phoneIndex'))['Id']
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}

			loadTableData($phoneTable);
			$modal.modal('hide');
		},
		error: requestError
	});
}

function showAddEmail() {

	var primaryEmail = '';
	if (!$('#emailTable').DataTable().rows(function (index, data) {
		if (data.IsPreferred) {
			primaryEmail = data.Email;
		}
		return data.IsPreferred;
	}).any()) {
		$('#EmailForCreate_IsPreferred').prop('checked', true);
	}

	var emailChangeMessageSpan = $('#EmailForCreate_ChangeEmailMessage');
	var text = emailChangeMessageSpan.text();
	text = text.replace('[[primaryEmail]]', primaryEmail);
	emailChangeMessageSpan.text(text);
	emailChangeMessageSpan.hide();
	$('#EmailForCreate_IsPreferred').change(function () {

		if (this.checked) {
			emailChangeMessageSpan.show();
		} else {
			emailChangeMessageSpan.hide();
		}
	});

	$('#detailModal').find('.modal-dialog').addClass('modal-sm');
}

function hiddenAddEmail() {
	$('#detailModal .modal-dialog').removeClass('modal-sm');
}

function addEmail() {
	$('#addEmailForm').submit();
}

function showEditEmail(event) {
	event.preventDefault();

	var $modal = $('#detailModal');
	var $form = $('#editPhoneForm');

	$modal.find('.modal-dialog').addClass('modal-sm');

	$form.hide();

	$.ajax({
		type: 'GET',
		cache: false,
		url: 'PersonalDetails/EmailEditJson/' + getTableRowData($('#emailTable'), $modal.data('emailIndex'))['Id'],
		success: function (data) {
			if (!data.Success) {
				showErrorModal('<p>' + concatStringArray(data.Errors) + '</p>');

				return;
			}
			var emailChangeMessageSpan = $('#EmailForEdit_ChangeEmailMessage');
			var text = emailChangeMessageSpan.text();
			text = text.replace('[[primaryEmail]]', data.PrimaryEmail);
			emailChangeMessageSpan.text(text);

			if (!data.IsPreferred) {
				emailChangeMessageSpan.hide();
			}

			var initialEmail = data.Email.toLowerCase();
			var initialPreferredValue = data.IsPreferred;
			var intialListing = data.IsCurrentlyListed;
			var initialExaminerCorrespondence = data.ExaminerCorrespondence;
			var $okButton = $modal.find('.modal-footer .btn-primary');
			$okButton.prop('disabled', true);
			$('#EmailForEdit_Id').val(data.Id);
			$('#EmailForEdit_Email').val(data.Email);
			$('#EmailForEdit_Email').change(function () {
				validateEditModal(initialEmail, initialPreferredValue, intialListing);
			});

			$('#EmailForEdit_IsPreferred').change(function () {
				validateEditModal(initialEmail, initialPreferredValue, intialListing, initialExaminerCorrespondence);
				if (this.checked) {
					emailChangeMessageSpan.show();
				} else {
					emailChangeMessageSpan.hide();
				}
			});
			$('#EmailForEdit_IsPreferred').prop('checked', data.IsPreferred);
			$('#EmailForEdit_IsCurrentlyListed').prop('checked', data.IsCurrentlyListed);
			$('#EmailForEdit_ExaminerCorrespondence').prop('checked', data.ExaminerCorrespondence);

			if (data.IsCurrentlyListed) {
				$('#emailIsCurrentlyListed').show();
			}

			$('#EmailForEdit_IsCurrentlyListed,#EmailForEdit_ExaminerCorrespondence').change(function () {
				validateEditModal(initialEmail, initialPreferredValue, intialListing, initialExaminerCorrespondence);
			});

			$form.show();
			$modal.modal('show');
		},
		error: requestError
	});

	function validateEditModal(initialEmail, initialPreferredFlag, intialListing, initialExaminerCorrespondence) {
		var $okButton = $modal.find('.modal-footer .btn-primary');
		var currentPreferredValue = $('#EmailForEdit_IsPreferred').prop('checked');
		var currentListing = $('#EmailForEdit_IsCurrentlyListed').prop('checked');
		var currentExaminerCorrespondence = $('#EmailForEdit_ExaminerCorrespondence').prop('checked');
		var currentEmail = $('#EmailForEdit_Email').val();
		var disableOkButton = initialPreferredFlag === currentPreferredValue &&
			(!currentEmail || currentEmail.toLowerCase() === initialEmail)
			&& intialListing === currentListing && currentExaminerCorrespondence === initialExaminerCorrespondence;
		$okButton.prop('disabled', disableOkButton);
	}
}

function hiddenEditEmail() {
	$('#detailModal .modal-dialog').removeClass('modal-sm');
}

function editEmail() {
	$('#editEmailForm').submit();
}

function showDeleteEmail() {
	var $modal = $('#detailModal');
	var data = getTableRowData($('#emailTable'), $modal.data('emailIndex'));

	if (data['IsLastContactInPD']) {
		$modal.find('p[data-message="lastInPD"]').show();
	} else if (data['IsCurrentlyListed']) {
		$modal.find('p[data-message="inPD"]').show();
	} else {
		$modal.find('p[data-message="generic"]').show();
	}
}

function deleteEmail() {
	var $modal = $('#detailModal');
	var $emailTable = $('#emailTable');

	$.ajax({
		url: 'PersonalDetails/DeleteEmail',
		type: 'post',
		data: {
			id: getTableRowData($emailTable, $modal.data('emailIndex'))['Id']
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}

			loadTableData($emailTable);
			$modal.modal('hide');
		},
		error: requestError
	});
}

function showAddWebsite() {
	$('#detailModal').find('.modal-dialog').addClass('modal-sm');
}

function hiddenAddWebsite() {
	$('#detailModal .modal-dialog').removeClass('modal-sm');
}

function addWebsite() {
	$('#addWebsiteForm').submit();
}

function showEditWebsite() {
	var $modal = $('#detailModal');

	$modal.find('.modal-dialog').addClass('modal-sm');

	$('#WebsiteForEdit_Url').val(getTableRowData($('#websiteTable'), $modal.data('websiteIndex'))['Url']);
}

function hiddenEditWebsite() {
	$('#detailModal .modal-dialog').removeClass('modal-sm');
}

function editWebsite() {
	$('#editWebsiteForm').submit();
}

function deleteWebsite() {
	$.ajax({
		url: 'PersonalDetails/WebsiteEditJson',
		type: 'post',
		data: {
			Url: ''
		},
		success: function (result) {
			if (result.ErrorMessage) {
				showErrorModal('<p>' + result.ErrorMessage + '</p>');
				return;
			}

			loadTableData($('#websiteTable'));
			$('#detailModal').modal('hide');
		},
		error: requestError
	});
}

function loadTableData($tableSelector) {
	$tableSelector.DataTable().ajax.reload();
}

function getTableRowData($tableSelector, rowIndex) {
	return $tableSelector.DataTable().row(rowIndex).data();
}

function ajaxSuccess(ajaxResponse) {
	var $modal = $('#detailModal');
	var $errors = $('.validation-summary-errors');

	$errors.empty();

	if (ajaxResponse.Success) {
		$errors.hide();
		$modal.modal('hide');

		loadTableData($('#' + $modal.data('modalDetail').toLowerCase() + 'Table'));
	} else {
		$errors.show();
		$errors.append('<ul></ul>');

		$.each(ajaxResponse.Errors, function (index, value) {
			$errors.find('ul').append('<li>' + value + '</li>');
		});
	}
}

function ajaxFailure() {
	var $errors = $('.validation-summary-errors');

	$errors.empty();
	$errors.append('<li>A communications error was encountered. Please try again.</li>');
}

function concatStringArray(stringValues) {
	var returnValue = '';

	$.each(stringValues, function (index, value) {
		returnValue = returnValue + (index > 0 ? '\n' : '') + value;
	});

	return returnValue;
}

function showErrorModal(error) {
	var $modal = $('#detailModal');

	$modal.find('.modal-body').html(error);
	$modal.find('.modal-footer .btn-primary').hide();
	$modal.modal('show');
}

function requestError(error) {
    if (error.responseText.indexOf('User is not authorised to access this item.') !== -1) {
        toastr.error('User is not authorised to access this item.');
    } else {
        toastr.error("An error occurred while processing your request.");
    }
}

function setGoogleMapsLocation(selectedAddress, latitude, longitude) {
	var url = mapUrl.replace(/\{0\}/g, latitude).replace(/\{1\}/g, longitude);
	$('#map').attr('src', url).attr('title', selectedAddress);
}

function setupGoogleAddressSearch(type) {
	var $modal = $('#detailModal');
	var $okButton = $modal.find('.modal-footer .btn-primary');
	var modalType = $modal.data('modalType');
	var $noAddressResultsFound = $('#noAddressResultsFound' + type);
	var $searchInput = $('#googleAddressSearch' + type);

	$searchInput.on('input', function () {
		$okButton.prop('disabled', true);
	});

	var defaultOptions = {
		bounds: [],
		types: ['geocode']
	};

	defaultOptions.bounds = new google.maps.LatLngBounds({ lat: -54.83376579999999, lng: 110.95103389999997 },
		{ lat: -9.187026399999999, lng: 159.28722229999994 });

	if ($searchInput.attr('address-search-minlength')) {
		setupCustom();
	}
	else {
		setupDefault();
	}

	function setupDefault() {
		var autocomplete = new google.maps.places
			.Autocomplete($searchInput[0], defaultOptions);

		autocomplete.addListener('place_changed', function () {
             onAddressSelected(autocomplete.getPlace());
		});
	}



    function onAddressSelected(selectedAddress) {
        var message = $('#contactNaati' + modalType);
        if (!selectedAddress) {
            $okButton.prop('disabled', true);
            message.addClass('alert-info');
        }

        return parseAddress(selectedAddress).then(function (address) {
            var parsedAddress = {
                FormattedAddress: selectedAddress.formatted_address,
                Latitude: selectedAddress.geometry.location.lat(),
                Longitude: selectedAddress.geometry.location.lng()
            };
            parsedAddress.StreetDetails = address.StreetDetails;
            parsedAddress.Suburb = address.Suburb;
            parsedAddress.PostCode = address.Postcode;
            parsedAddress.Country = address.Country;
            parsedAddress.Address = address.StreetDetails;
            parsedAddress.FullAddress = address.StreetDetails;
            parsedAddress.State = address.State;
            parsedAddress.StreetNumber = address.StreetNumber;
            parsedAddress.ValidateInExternalTool = true;
            $('#ValidateByGoogle').val(true);
            setGoogleMapsLocation(parsedAddress.FullAddress, parsedAddress.Latitude, parsedAddress.Longitude);

            setAddressFields(parsedAddress);

            $okButton.prop('disabled', false);

            message.removeClass('alert-info');
        });
	}
	function setupCustom() {
		$searchInput.autocomplete({
			minLength: $searchInput.attr('address-search-minlength'),
			source: function (request, response) {
				search(request.term).then(function (data) {
					response(data);
				});
			},
			select: function (event, ui) {
				getPredictionDetails(ui.item.place_id).then(function (data) {
                    onAddressSelected(data);				
				});
			}
		});
	}

	function search(query) {
		//var restrictions = { country: 'uk' };
		var service = new google.maps.places.AutocompleteService();

		return new Promise(function (resolve, reject) {
			function displaySuggestions(predictions, status) {
				if (status != google.maps.places.PlacesServiceStatus.OK) {
					return resolve([]);
				}

				$.each(predictions, function (i, p) {
					p.label = p.description;
					p.value = p.description;
				});

				return resolve(predictions);
			};

			service.getPlacePredictions($.extend({ input: query }, defaultOptions), displaySuggestions);
		});
    }

    function parseAddress(address) {
        var request = getParseAddressRequest(address);
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'POST',
                data: request,
                dataType: 'json',
                url: 'PersonalDetails/ParseAddress',
                success: function (data) {
                    resolve(data);
                },
                error: function (error) {
                    reject(error);
                }
            });
        });
    }

    function getParseAddressRequest(address) {

        return {
            Geometry: {
                Location: {
                    Lat: address.geometry.location.lat(),
                    Lng: address.geometry.location.lng()
                }
            },
            Formatted_Address: address.formatted_address,
            Types: address.types,
            Address_Components: address.address_components.map(getAddressComponent)
        };
    }

    function getAddressComponent(component) {
        return {
            Long_Name: component.long_name,
            Short_Name: component.short_name,
            Types: component.types
        };
    }

	function getPredictionDetails(placeId) {
		var placeService = new google.maps.places.PlacesService(document.getElementById("map"));
		return new Promise(function (resolve, reject) {
			placeService.getDetails({
				placeId: placeId
			}, function (result, status) {
				if (status === google.maps.places.PlacesServiceStatus.OK) {
					resolve(result);
				} else {
					reject(status);
				}
			});
		});
	}
}