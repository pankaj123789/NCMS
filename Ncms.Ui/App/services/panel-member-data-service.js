define(['services/util-data-service'], function (utilDataService) {
	var data = utilDataService('api/panelMembership');

	data.getMembershipMapper = function (credentialTypes) {
		return function (membership) {
			return data.mapMembership(membership, credentialTypes);
		};
	};

	data.mapMembership = function (membership, credentialTypes) {
		membership.StartDateFormatted = moment(membership.StartDate).format(CONST.settings.shortDateDisplayFormat);
		membership.EndDateFormatted = moment(membership.EndDate).format(CONST.settings.shortDateDisplayFormat);

		var reducer = function (accumulator, currentCredentialTypeId, index, array) {
			var credentialType = credentialTypes.find(function (ct) {
				return ct.Id === currentCredentialTypeId;
			});

			if (credentialType) {
				if (index == array.length - 1) {
					return accumulator.concat(credentialType.DisplayName);
				}
				else {
					return accumulator.concat(credentialType.DisplayName + ", ");
                }
			}

			return accumulator;
		};

		membership.CredentialTypes = membership.CredentialTypeIds.reduce(reducer, '');
		membership.MaterialCredentialTypes = membership.MaterialCredentialTypeIds.reduce(reducer, '');
		membership.CoordinatorCredentialTypes = membership.CoordinatorCredentialTypeIds.reduce(reducer, '');

		//var text = "";
		//text = text || '<li>No credential types assigned</li>';
        //var $div = $('<div style="float:left;">' + text + '</div>').appendTo('body');
		//var minPopoverWidth = 230;
		//var width = $div.width() + 40;
		//width = width < minPopoverWidth ? minPopoverWidth : width;
		//$div.remove();
		//text = '<ul class="w-xxl small text-left  m-l-n-md">' + text + '</ul>';
		//var popOverTitle = ko.Localization('Naati.Resources.Panel.resources.CredentialTypes');

		//if (width < 100) {
		//	width = 140;
		//}

		//membership.Popover = {
		//	html: true,
		//	animation: false,
		//	trigger: 'hover',
		//	container: 'body',
		//	title: popOverTitle,
		//	content: text,
		//	placement: 'top',
		//	template: '<div class="popover" role="tooltip" style="width: ' + width + 'px; max-width:' + width + 'px;"><div class="arrow"></div><h3 style="background-color:#0081A3;" class="popover-title bg-info"></h3><div class="popover-content"></div></div>'

		//};


        var markingrequestPopOverTitle = ko.Localization('Naati.Resources.Panel.resources.OutstandingMarkingRequestTitle');
        membership.displaynone = (membership.InProgress || membership.Overdue) ? true : false;
        var markingrequestText = '<li>In Progress: ' + membership.InProgress + '</li>';
	    markingrequestText += '<li>Overdue: ' + membership.Overdue + '</li>';
        markingrequestText = '<ul class="w-xxl small text-left  m-l-n-md">' + markingrequestText + '</ul>';
	    var widthSummary = 220;
        membership.markingrequestPopover = {
	        html: true,
	        animation: false,
	        trigger: 'hover',
	        container: 'body',
            title: markingrequestPopOverTitle,
            content: markingrequestText,
	        placement: 'top',
            template: '<div class="popover" role="tooltip" style="width: ' + widthSummary + 'px; max-width:' + widthSummary + 'px;"><div class="arrow"></div><h3 style="background-color:#0081A3;" class="popover-title bg-info"></h3><div class="popover-content"></div></div>'

        };

        var examinerunavailablePopOverTitle = ko.Localization('Naati.Resources.Panel.resources.ExaminerUnavailableTitle');
	    var examinerunavailableText = '';
        if (membership.UnAvailableExaminers.length > 0) {
            for (var i = 0; i < membership.UnAvailableExaminers.length; i++) {
                if (i === 4) {
                    examinerunavailableText += '<li>......</li>';
                    break;
                } else {
                    var startDate = moment(membership.UnAvailableExaminers[i].StartDate).format(CONST.settings.shortDateDisplayFormat);
                    var endDate = moment(membership.UnAvailableExaminers[i].EndDate).format(CONST.settings.shortDateDisplayFormat);

                    examinerunavailableText += '<li>' + startDate + ' to ' + endDate + '</li>';   
                }
            }
        }
        membership.examinerunavailablePopover = {
	        html: true,
	        animation: false,
	        trigger: 'hover',
	        container: 'body',
            title: examinerunavailablePopOverTitle,
            content: examinerunavailableText,
	        placement: 'top',
            template: '<div class="popover" role="tooltip" style="width: ' + widthSummary + 'px; max-width:' + widthSummary + 'px;"><div class="arrow"></div><h3 style="background-color:#0081A3;" class="popover-title bg-info"></h3><div class="popover-content"></div></div>'

	    };

        membership.ShowPopover = function (model, event) {
            var $element = $(event.currentTarget);
            if (!$element.data("bs.popover") || !$element.attr('data-popoverAttached')) {

                if ($element.data("popover") === 'markingrequest') {
                    $element.popover('destroy').popover(membership.markingrequestPopover);
                } else if ($element.data("popover") === 'examinerunavailable') {
                    $element.popover('destroy').popover(membership.examinerunavailablePopover);
                } else {
                    $element.popover('destroy').popover(membership.Popover);
                }
                $element.attr('data-popoverAttached', true);
            }
            $element.popover('show');
        };

		return membership;
	};
	return data;
});
