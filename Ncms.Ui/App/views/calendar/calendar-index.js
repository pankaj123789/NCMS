define([
    'plugins/router',
    'views/shell',
    'modules/enums',
    'services/calendar-data-service'
],
function (router, shell, enums, calendarService) {
    var vm = {
        title: shell.titleWithSmall,
        searchTerm: ko.observable({}),
    };

    var templateData = {
        calendarOptions: {
            minTime: '05:00:00',
            maxTime: '21:00:00',
            contentHeight: 'auto',
            eventStartEditable: false,
            eventDurationEditable: false,
            events: function (start, end, timezone, callback) {
                var request = parsedSearchTerm();
                request.Start = start.format();
                request.End = end.format();
				setTimeout(disableCalendarButtons, 1);

                calendarService.getFluid('events', request).then(function (data) {
                    $('.popover.fade.right.in').remove();
					callback(data);
					enableCalendarButtons();
                });
            },
            eventRender: function (event, element) {
                element.addClass(event.css).find('.fc-title').html(element.find('.fc-title').text());

                if (event.popover) {
                    element.popover({
                        title: event.popover.title,
                        placement: 'right',
                        container: 'body',
                        trigger: 'hover',
                        content: event.popover.content,
                        html: true
                    });
                }
            },
            eventDataTransform: function (e) {
                var title = 'TS{0} -  {1}'.format(
                    e.Id,
                    e.TestSessionName
                );

                var description = '{0}<br />{1} of {2}<br /> {3}'.format(
                    e.VenueName,
                    e.Attendees,
                    e.Capacity,
                    e.RejectedCount + " Rejected"
                );

                var css = '';

                if (e.Completed) {
                    css = 'bg-success';
                }
                else if (e.Attendees > 0 && e.Attendees == e.Accepted) {
                    css = 'bg-info';
                }
                else {
                    css = 'bg-dark-yellow';
                }

                return {
                    id: e.Id,
                    title: title + '<br />' + description,
                    popover: {
                        title: title,
                        content: description
                    },
                    start: e.TestDate,
                    end: moment.utc(e.TestDate).add({ minutes: e.Duration }),
                    css: css
                };
            },
            eventClick: function (calEvent, jsEvent, view) {
                router.navigate('test-session/' + calEvent.id);
            },
            init: function (element) {
				templateData.calendarOptions.element = element;
            }
        }
    };

    vm.searchComponentOptions = {
        name: 'custom-search',
        params: {
            title: vm.title,
            filters: [
                { id: 'TestLocation' },
                { id: 'CredentialTestSession' },
                { id: 'IsActive'}
            ],
            searchType: enums.SearchTypes.Calendar,
            searchTerm: vm.searchTerm,
            searchCallback: function () {
                $(templateData.calendarOptions.element).fullCalendar('refetchEvents');
            },
            resultTemplate: {
                name: 'calendarTemplate',
                data: templateData
            },
            additionalButtons: [{
                'class': 'btn btn-success',
                click: createTestSession,
                icon: 'glyphicon glyphicon-plus',
                resourceName: 'Naati.Resources.TestSession.resources.NewTestSession',
                enableWithPermission: 'TestSession.Create'
            }]
        }
    };

    vm.activate = function () {
        if (templateData.calendarOptions.element) {
            var $element = $(templateData.calendarOptions.element);
            if ($element.length && $element.data('fullCalendar')) {
                $element.fullCalendar('refetchEvents');
            }
        }
	};

	function enableCalendarButtons() {
		$(templateData.calendarOptions.element)
			.find('button')
			.removeClass('fc-state-disabled')
			.each(function () {
				var $this = $(this);
				$this.prop('disabled', $this.data('old-disabled'));

				if ($this.prop('disabled')) {
					$this.addClass('fc-state-disabled');
				}
			});
	}

	function disableCalendarButtons() {
		var $element = $(templateData.calendarOptions.element);
		$element.find('button')
			.each(function () {
				var $this = $(this);
				$this.data('old-disabled', $this.prop('disabled'));
			})
			.addClass('fc-state-disabled')
			.prop('disabled', 'disabled');
	}

    function parsedSearchTerm() {
        var searchTerm = vm.searchTerm();
        var parsed = {};

        addFilterOption(parsed, (searchTerm.CredentialTestSession || {}).Data, 'Credential');
        addFilterOption(parsed, (searchTerm.CredentialTestSession || {}).Data, 'CredentialSkill');
        addFilterOption(parsed, (searchTerm.TestLocation || {}).Data, 'PreferredTestLocation');
        addFilterOption(parsed, (searchTerm.TestLocation || {}).Data, 'TestVenue');
        addFilterOption(parsed, (searchTerm.IsActive || {}).Data, 'IsActive');

        return parsed;
    }

    function addFilterOption(parsed, searchTerm, filterId) {
        if (searchTerm && searchTerm[filterId]) {
            parsed[filterId] = searchTerm[filterId].Data.selectedOptions;
        }
        //if (searchTerm && (searchTerm.checked || !searchTerm.checked)) {
        //    parsed[filterId] = searchTerm.checked;
        //}
        if (searchTerm && typeof(searchTerm.checked) !== 'undefined') {
            parsed[filterId] = searchTerm.checked;
        }
    }

    function createTestSession() {
        router.navigate('test-session-wizard');
    }

    return vm;
});
