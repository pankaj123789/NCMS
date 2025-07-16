define([
    'services/screen/message-service',
    'services/person-data-service',
    'services/file-data-service',
    'services/application-data-service',
    'services/test-material-data-service'
], function (message, personService, fileService, applicationService, testMaterialService) {
    var defaultFileName = null;

    fileService.getFluid('defaultassetfilename').then(function (data) {
        defaultFileName = data;
    });

    var link = document.createElement('a');

    var relToAbs = function (el) {
        var clone = $(el).clone()[0];
        var linkHost;

        if (clone.nodeName.toLowerCase() === 'link') {
            link.href = clone.href;
            linkHost = link.host;

            if (linkHost.indexOf('/') === -1 && link.pathname.indexOf('/') !== 0) {
                linkHost += '/';
            }

            clone.href = link.protocol + '//' + linkHost + link.pathname + link.search;
        }

        return clone.outerHTML;
    };

    function getOption(value, option, resource) {
        return {
            value: value,
            text: ko.Localization('Naati.Resources.' + resource + '.resources.' + option.split(' ').join(''))
        };
    }

    function recursiveFlag(options) {
        var preventBubbling = false;
        var parents = [];

        function setChildrenFlag(item, parent) {
            preventBubbling = true;
            setChildren(item);
            setParent(parent);
            preventBubbling = false;
        }

        function setChildren(item) {
            var localItem = getItem(item, parents);

            if (!localItem) {
                return;
            }

            var shouldSetParent = false;
            var processChildren = [];
            ko.utils.arrayForEach(localItem.children, function (i) {
                if (!(options.flagName in i.item)) {
                    return;
                }
                shouldSetParent = true;
                i.item[options.flagName](item[options.flagName]());
                processChildren.push(i);
            });

            ko.utils.arrayForEach(processChildren, function (i) {
                setChildren(i.item);
            });

            if (shouldSetParent) {
                setParent(localItem);
            }
        }

        function setParent(parent) {
            if (!parent) {
                return;
            }

            var indeterminate = ko.utils.arrayFilter(parent.children, function (pi) {
                return pi.item[options.flagName]() == null;
            });

            var checked = ko.utils.arrayFilter(parent.children, function (pi) {
                return pi.item[options.flagName]();
            });

            if (indeterminate.length) {
                parent.item[options.flagName](null);
            }
            else if (!checked.length) {
                parent.item[options.flagName](false);
            }
            else if (checked.length == parent.children.length) {
                parent.item[options.flagName](true);
            }
            else {
                parent.item[options.flagName](null);
            }

            setParent(parent.parent);
        }

        function getItem(item, source) {
            if (!item) {
                return null;
            }

            for (var i = 0; i < source.length; i++) {
                if (item === source[i].item) {
                    return source[i];
                }
            }

            for (var i = 0; i < source.length; i++) {
                var localItem = getItem(item, source[i].children);
                if (localItem) {
                    return localItem;
                }
            }

            return null;
        }

        function resetFlag(source) {
            for (var i = 0; i < source.length; i++) {
                source[i].item[options.flagName](false);
                resetFlag(source[i].children);
            }
        }

        return {
            addFlag: function (item, parent) {
                var localItem = { parent: getItem(parent, parents), item: item, children: [] };
                if (!parent) {
                    parents.push(localItem);
                }
                else {
                    var localParent = getItem(parent, parents);
                    if (localParent) {
                        localParent.children.push(localItem);
                    }
                }

                item[options.flagName] = ko.observable(false);
                item[options.flagName].subscribe(function () {
                    if (preventBubbling) {
                        return;
                    }
                    setChildrenFlag(localItem.item, localItem.parent);
                });
            },
            reset: function () {
                resetFlag(parents);
            }
        };
    }

    return {
        topics: function () {
            return {
                splitJob: function (testId) {
                    return 'split-job-' + testId;
                }
            };
        },
        functions: function () {
            return {
                recursiveFlag: recursiveFlag,
                naatiNumberSearch: function (naatiNumber, callback) {
                    personService.get({ term: naatiNumber }).then(function (data) {
                        $.each(data, function (i, d) {
                            d.BirthDateFormatted = moment(d.BirthDate).format(CONST.settings.shortDateDisplayFormat);
                            d.NaatiNumberAndName = '{0} - {1}'.format(d.NaatiNumber, d.Name);
                        });

                        callback(data);
                    });
                },
                getTest: function (test) {
                    var direction = '';

                    if (test.Direction === 'E')
                        direction = ko.Localization('Naati.Resources.Test.resources.ToEnglish');
                    else if (test.Direction === 'O')
                        direction = ko.Localization('Naati.Resources.Test.resources.FromEnglish');
                    else if (test.Direction === 'B')
                        direction = ko.Localization('Naati.Resources.Test.resources.Both');

                    return {
                        abbreviation: ko.observable(test.Abbreviation),
                        testResultId: ko.observable(test.TestResultID),
                        testId: ko.observable(test.TestID),
                        lastReviewJobID: ko.observable(test.LastReviewJobID),
                        dueDateReview: ko.observable(),
                        jobId: ko.observable(test.JobID),
                        testDate: ko.observable(moment(test.TestDate).format(CONST.settings.shortDateDisplayFormat)).extend({ required: true }),
                        dueDate: ko.observable(moment(test.DueDate).format(CONST.settings.shortDateDisplayFormat)).extend({
                            required: {
                                onlyIf: function () {
                                    return test.JobId;
                                }
                            }
                        }),
                        naatiNumber: ko.observable(test.NAATINumber),
                        entityId: ko.observable(test.EntityId),
                        personName: ko.observable(test.PersonName),
                        testDescription: ko.observable(test.Description),
                        testLanguage: ko.observable(test.Language),
                        testLanguageId: ko.observable(test.LanguageId),
                        testIsSat: ko.observable(test.HasSat),
                        testHasAssets: ko.observable(test.HasAssets),
                        testHasExaminers: ko.observable(test.HasExaminers),
                        testEventId: ko.observable(test.EventId),
                        testStatus: ko.observable(test.Status),
                        testStatusText: ko.observable(ko.Localization('Naati.Resources.TestStatus.resources.' + test.Status)),
                        testEventVenueId: ko.observable(test.EventVenueId).extend({ required: true }),
                        testOffice: ko.observable(test.Office),
                        testResultStatus: ko.observable(!test.ResultStatus
                                    ? ko.Localization('Naati.Resources.Shared.resources.NA')
                                    : test.ResultStatus),
                        testMaterialId: ko.observable(test.TestMaterialId).extend({ required: true }),
                        isEportalActive: ko.observable(test.IsEportalActive),
                        personBirthDate: ko.observable(test.PersonBirthDate),
                        personHasPhoto: ko.observable(test.PersonHasPhoto),
                        photoDate: ko.observable(test.PhotoDate),
                        eportalRegistrationDate: ko.observable(test.EportalRegistrationDate),
                        level: ko.observable(test.Level),
                        levelId: ko.observable(test.LevelId),
                        category: ko.observable(test.Category),
                        direction: ko.observable(direction),
                        supplementary: ko.observable(test.Supplementary)
                    };
                },
                testAssetFileNameProcessor: function (options) {
                    var defaultOptions = {
                        FileName: null,
                        DocumentTypeName: null,
                        Test: null
                    };

                    $.extend(defaultOptions, options);

                    var ext = defaultOptions.FileName.split('.');
                    var test = $.extend({}, ko.toJS(defaultOptions.Test), { DocumentType: defaultOptions.DocumentTypeName });

                    var newName = defaultFileName.replace(/\[(.*?)\]/g, function (match, content) {
                        if (content === 'Date') {
                            return moment().format('DDMMYYYY');
                        }
                        if (content in test) {
                            return test[content];
                        }
                        else {
                            return match;
                        }
                    });

                    return newName + '.' + ext[ext.length - 1];
                },
                getTablePrintHtml: function (tableId, options) {
                    var config = $.extend({
                        header: true,
                        footer: false,
                        exportOptions: {}
                    }, options);

                    var dt = $('#' + tableId).DataTable();
                    var data = dt.buttons.exportData(config.exportOptions);
                    var addRow = function (d, tag, attr) {
                        var row = '<tr>';

                        for (var i = 0; i < d.length; i++) {
                            row += '<' + tag + (attr ? ' ' + attr : '') + '>' + d[i] + '</' + tag + '>';
                        }

                        return row + '</tr>';
                    };

                    var html = '<table class="' + dt.table().node().className + '">';

                    if (config.header) {
                        html += '<thead>' + addRow(data.header, 'th') + '</thead>';
                    }

                    html += '<tbody>';

                    if (data.body.length) {
                        for (var i = 0; i < data.body.length; i++) {
                            html += addRow(data.body[i], 'td');
                        }
                    } else {
                        html += addRow(['No data available in table'], 'td', 'colspan="{0}"'.format(data.header.length));
                    }

                    html += '</tbody>';

                    if (config.footer) {
                        html += '<tfoot>' + addRow(data.footer, 'th') + '</tfoot>';
                    }

                    html += '</table>';

                    return html;
                },
                print: function (html, options) {
                    var config = $.extend({
                        title: '*',
                        autoPrint: true
                    }, options);

                    var win = window.open('', '');
                    var title = config.title;

                    if (typeof title === 'function') {
                        title = title();
                    }

                    if (title.indexOf('*') !== -1) {
                        title = title.replace('*', $('title').text());
                    }

                    var head = '<title>' + title + '</title>';

                    $('style, link').each(function () {
                        head += relToAbs(this);
                    });

                    $(win.document.head).html(head);
                    $(win.document.body).html(html);

                    if (config.customize) {
                        config.customize(win);
                    }

                    if (config.autoPrint) {
                        setTimeout(function () {
                            win.print();
                            win.close();
                        }, 250);
                    }

                    win.document.close();
                },
                performOperation: function (operationRequest) {
                    var defer = Q.defer();

                    function checkFail(data) {
                        if (!data.Error) {
                            defer.resolve(data);

                            return;
                        }

                        message.confirm({
                            title: ko.Localization('Naati.Resources.Shared.resources.Warning'),
                            content: operationRequest.errorMessage
                        }).then(function (answer) {
                            if (answer === 'yes') {
                                operationRequest.service.post({ OperationId: data.OperationId }, operationRequest.retryAction).then(checkFail);
                            }
                            else {
                                operationRequest.service.post({ OperationId: data.OperationId }, operationRequest.cancelAction);

                                defer.reject('Cancelled');
                            }
                        });
                    }

                    operationRequest.service.post(operationRequest.data, operationRequest.action).then(checkFail);

                    return defer.promise;
                },
                optionsValueFactory: function (optionsList, resource) {
                    return $.map(optionsList,
                        function (option) {
                            return getOption(option, option, resource);
                        });
                },
                optionsNameFactory: function (optionsList, resource, useValue) {
                    useValue = useValue === true;

                    var options = [];

                    for (var option in optionsList) {
                        if (option !== 'list' && optionsList.hasOwnProperty(option)) {
                            var value = useValue ? optionsList[option] : option;
                            options.push(getOption(value, option, resource));
                        }
                    }

                    return options;
                },
                humanizeDate: function (date) {
                    var delta = Math.round((+new Date - date) / 1000);
                    var datePart = new Date();
                    datePart.setHours(0,0,0,0)//take out hours, minutes and seconds
                    var datePartDelta = Math.round((datePart - date) / 1000);


                    var minute = 60,
                        hour = minute * 60,
                        day = hour * 24,
                        week = day * 7;

                    var fuzzy;

                    if (delta < 30) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.JustThen');
                    } else if (delta < minute) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.SecondsAgo').format(delta);
                    } else if (delta < 2 * minute) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.AMinuteAgo');
                    } else if (delta < hour) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.MinutesAgo').format(Math.floor(delta / minute));
                    } else if (Math.floor(delta / hour) == 1) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.AHourAgo');
                    } else if (delta < day && datePartDelta < 0) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.HoursAgo').format(Math.floor(delta / hour));
                    } else if (datePartDelta < day) {
                        fuzzy = ko.Localization('Naati.Resources.Shared.resources.Yesterday');
                    }
                    else {
                        fuzzy = moment(date).format("L");
                    }

                    return fuzzy;
                },
                getLookup: function (lookupName) {
                    return applicationService.getFluid('LookupType/' + lookupName);
                },
                getVenues: function (testLocationIds) {
                    return applicationService.getFluid('venue', { TestLocation: testLocationIds });
                },
                getVenuesShowingInactive: function (testLocationIds) {
                    return applicationService.getFluid('venuesShowingInactive', { TestLocation: testLocationIds });
                },
                getCredentialTypeSkills: function (credentialTypeIds) {
                    return applicationService.getFluid('credentialTypeSkills', { CredentialTypeIds: credentialTypeIds });
                },
                getCredentialTypeDomains: function (credentialTypeIds) {
                    return applicationService.getFluid('credentialTypeDomains', { CredentialTypeIds: credentialTypeIds });
                },
                getCredentialTypeTaskTypes: function (credentialTypeIds) {
                    return testMaterialService.getFluid('taskTypes', { CredentialTypeIds: credentialTypeIds });
                },
                getCredentialTypeSkillsTestSession: function (credentialTypeIds) {
                    return applicationService.getFluid('credentialTypeSkillsTestSession', { CredentialTypeIds: credentialTypeIds });
                },
                formatPhone: function (phoneNumber) {
                    if (!phoneNumber) {
                        return phoneNumber;
                    }

                    var localNumber = phoneNumber.replace(/[^a-zA-Z0-9-+]/g, "");
                    var symbolCount = localNumber.match(/[-+]/g) !== null ? (localNumber.match(/[-+]/g)).length : 0;

                    if (symbolCount < 2) {
                        if (localNumber.length - symbolCount == 8) {
                            //Where phone number is eight digits long format as xxxx xxxx
                            return localNumber.substr(0, 4 + symbolCount) + ' ' + localNumber.substr(4 + symbolCount);
                        }
                        else if (localNumber.replace(/[-+]/g, "").startsWith("04")) {
                            //Where phone number is ten digits long format and starts with 04, format as xxxx xxx xxx 
                            return localNumber.substr(0, 4 + symbolCount) + ' ' + localNumber.substr(4 + symbolCount, 3 + symbolCount) + ' ' + localNumber.substr(7 + symbolCount);
                        }
                        else if (localNumber.length - symbolCount == 10) {
                            //Where phone number is ten digits long format and starts with 0A, format as xx xxxx xxxx 
                            return localNumber.substr(0, 2 + symbolCount) + ' ' + localNumber.substr(2 + symbolCount, 4 + symbolCount) + ' ' + localNumber.substr(6 + symbolCount);
                        }
                        else if (localNumber.length > 0) {
                            //Where phone number is longer than ten characters display with a space every four characters
                            var number = '';
                            while (localNumber.length > 4) {
                                number += localNumber.substr(0, 4 + symbolCount) + ' ';
                                localNumber = localNumber.substr(4 + symbolCount);
                            }

                            number += localNumber;
                            number = $.trim(number);

                            return number;
                        }

                        return phoneNumber;
                    }

                    return localNumber;
                },
                htmlEncode: function (value) {
                    return $('<div/>').text(value).html();
                },
                htmlDecode: function (value) {
                    return $('<div/>').html(value).text();
                }
            };
        }
    };
});
