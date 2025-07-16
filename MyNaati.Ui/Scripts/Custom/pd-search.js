var pdSearch = function () {
    var pdSearchUrl;
    var countUrl;
    var contactUrl;
    var exportSearchUrl;
    var englishLanguageId;
    var australiaCountryId = 13;
    var recaptchaScreenUrl;
    var fromLangugeId;
    var toLanguageId;
    var countryList;
    var stateList;
    var certificationList;

    var formSelector = '#searchForm';
    var countryIdSelector = '#CountryId';
    var australianStateIdSelector = '#StateId';
    var suburbSelector = '#Suburb';
    var postcodeSelector = '#Postcode';
    var resultLastUpdatedSelector = '#resultLastUpdated';
    var showingDialog = false;
    var oldCounts;
    var noOfPages;

    var previousFormData;
    var previousFormDataBroadened;

    return {
        initialise: function (pdSearchUrlParam, countUrlParam, contactUrlParam, urlForSearchAgainButtonParam, exportUrlParam, australiaCountryIdParam, englishLanguageIdParam, recaptchaScreenUrlParam, firstLanguageId, secondLanguageId, certifications, countries, states, skills) {
            pdSearchUrl = pdSearchUrlParam;
            countUrl = countUrlParam;
            contactUrl = contactUrlParam;
            exportSearchUrl = exportUrlParam;
            australiaCountryId = australiaCountryIdParam;
            englishLanguageId = englishLanguageIdParam;
            recaptchaScreenUrl = recaptchaScreenUrlParam;
            fromLangugeId = firstLanguageId;
            toLanguageId = secondLanguageId;
            previousFormData = $(formSelector).serializeArray();
            countryList = countries;
            certificationList = certifications;
            stateList = states;
            skillList = skills;

            pdSearch.updateResultsLastUpdated('');
            pdSearch.hideMessageArea();

            $('#PDSearchMenu').addClass('active');

            $('#AccreditationCategoryId option').each(function () {
                this.text = this.text + " (0)";
            });

            $('#AccreditationLevelId option').each(function () {
                this.text = this.text + " (0)";
            });

            $('#FirstLanguageId option').each(function () {
                this.text = this.text + " (0)";
            });

            $('#SecondLanguageId option').each(function () {
                this.text = this.text + " (0)";
            });

            $('#StateId option').each(function () {
                this.text = this.text + " (0)";
            });

            $('#CountryId option').each(function () {
                this.text = this.text + " (0)";
            });

            $('#sorting').change(function () {
                pdSearch.performSearch();
            });

            $(formSelector).submit(function () {
                pdSearch.showLoadingOverlay();
                pdSearch.disableSearchButtons();

                previousFormData = $(formSelector).serializeArray();

                $.ajax({
                    type: 'post',
                    data: $( this ).serialize(),
                    dataType: 'json',
                    url: pdSearchUrl,
                    success: function (data) {
                        pdSearch.searchSuccess(data);
                    },
                    error: function () {
                        pdSearch.showMessageArea("Sorry. An error occurred processing your request", true);
                        pdSearch.hideLoadingOverlay();
                        pdSearch.enableSearchButtons();
                    }
                });

                return false;
            });

            $('#clearAllButton').click(function () {
                $('#resultsContainer').html("");
                previousFormData = null;
                pdSearch.hideMessageArea();

                // $('#AccreditationCategoryId').val('');
                $('#AccreditationLevelId').val('0');
                $('#StateId').val('0');

                $("#CountryId option").each(function() {
                    if ($(this).val() === australiaCountryId.toString()) {
                        $('#CountryId').val(australiaCountryId);
                        return false;
                    } else {
                        $('#CountryId').val(0);
                    }
                });

                $('#Postcode').val('');
                $('#Suburb').val('');
                $('#Surname').val('');

                $('#pagingContainer').empty();
                $('#sortingContainer').hide();

                pdSearch.updateAustralianStateEnabled();

                pdSearch.updateCounts();
            });

            $('#searchAgainButton').click(function () {
                pdSearch.HideResults();
                var randomNumber = createRandomNumber(0, 25000);
                assignRandomSeed(randomNumber);
            });

            $('.collapsible-header').click(function () {
                var collapsibleContainer = $(this).next();
                var collapsibleButton = $(this).find('.collapsible-button');

                collapsibleContainer.slideToggle(250, function () {
                    collapsibleButton.toggleClass('collapsed');
                });
            });

            $('#searchButton').click(function() {
                if (previousFormData != $(formSelector).serializeArray()) {
                    $('#pagingCurrentPage').val(1);
                    var randomNumber = pdSearch.createRandomNumber(0, 25000);
                    pdSearch.assignRandomSeed(randomNumber);
                }
            });

            $(countryIdSelector).change(function () { pdSearch.updateAustralianStateEnabled(); });

            $('.search select').change(function () {
                pdSearch.updateCounts();
            });

            pdSearch.performSearch();
            pdSearch.updateCounts();
            pdSearch.updateAustralianStateEnabled();
        },

        changePage: function (changeToNext) {
            if (changeToNext == true) {
                if ($('#pagingCurrentPage').val() != noOfPages) {
                    $('#pagingCurrentPage').val((Number($('#pagingCurrentPage').val()) + 1).toString());
                }
            } else {
                $('#pagingCurrentPage').val((Number($('#pagingCurrentPage').val()) - 1).toString());
            }
            $("html, body").animate({ scrollTop: 0 }, "slow");
        },

        performSearch: function () {
            $(formSelector).submit();
        },

        //getSelector: function (v) {
        //    switch (v) {
        //        case 1:
        //            return "#FirstLanguageId";
        //        case 2:
        //            return "#SecondLanguageId";
        //        case 3:
        //            return "#AccreditationCategoryId";
        //        case 4:
        //            return "#AccreditationLevelId";
        //        case 5:
        //            return "#CountryId";
        //        case 6:
        //            return "#StateId";
        //        default:
        //            return "";
        //    }
        //},

        getSelector: function (v) {
            switch (v) {
            case 0:
                return "#AccreditationLevelId";
            case 1:
                return "#CountryId";
            case 2:
                return "#StateId";
            default:
                return "";
            }
        },

        submitForExport: function (anchor) {

            if (previousFormData == null || previousFormData.length === 0) {
                return false;
            }

            var query = "";

            for (var i = 0; i < previousFormData.length; i++) {

                if (i === 0) {
                    query += "?"; // First time add this

                } else if (i > 0) {
                    query += "&"; // All other times add this
                }

                query += (previousFormData[i].name + "=" + previousFormData[i].value);
            }
            query = query.replace("[0]", "first").replace("[1]", "second");
            query = exportSearchUrl + query;

            $(anchor).attr('href', query);
        },

        updateResultsLastUpdated: function (text) {
            if (text == '') {
                $(resultLastUpdatedSelector).parent().css('visibility', 'hidden');
            } else {
                $(resultLastUpdatedSelector).parent().css('visibility', 'visible');
            }

            $(resultLastUpdatedSelector)[0].innerHTML = text;
        },


        getDropDownList: function (v) {
            switch (v) {
            case "#AccreditationLevelId":
                return certificationList;
            case "#CountryId":
                return countryList;
            case "#StateId":
                return stateList;
            default:
                return [];
            }
        },

        populateSearchValues: function (results, oldResults, uniqueVals, uniqueOldVals, selector) {
            
            var selectedId = 0;
            $(selector + ' option').each(function() {
                if (this.selected) {
                    selectedId = this.value;
                }
            });

            $(selector).empty();

            var dropDownList = pdSearch.getDropDownList(selector);
            dropDownList.forEach(function (item, index) {
                var foundResult = undefined;

                for (var i = 0; i < results.length ; i++) {
                    var result = results[i];
                    if (result.Val.toString() === item.value) {
                        foundResult = result;
                        break;
                    }
                }

                if (foundResult) {
                    var selected = selectedId === item.value ? 'selected =\"selected\"' : "";
                    $(selector).append("<option value='" +item.value + "' " + selected+ ">" +
                        item.text +
                        " (" +
                        foundResult.Count +
                        ")" +
                        "</option>");
                }

            });
        },


        updateSearch: function (data) {
            $(data.Data.Results).each(function (v) {
                var results = data.Data.Results[v];
                var uniqueVals = new Array(results.length);

                results.forEach(function (item, index) {
                    uniqueVals[index] = data.Data.Results[v][index].Val;
                });

                var oldResults;
                var uniqueOldVals;

                if (oldCounts != null) {

                    oldResults = oldCounts.Data.Results[v];
                    uniqueOldVals = new Array(oldResults.length);
                    oldResults.forEach(function (item, index) {
                        uniqueOldVals[index] = oldCounts.Data.Results[v][index].Val;
                    });
                }

                var selector = pdSearch.getSelector(v);
                if (selector != "") {
                    pdSearch.populateSearchValues(results, oldResults, uniqueVals, uniqueOldVals, selector);
                }
            });
        },

        enableSearchButtons: function () {
            $('#searchButton').removeAttr('disabled');
            $('#clearAllButton').removeAttr('disabled');
        },

        disableSearchButtons: function () {
            $('#searchButton').attr('disabled', 'disabled');
            $('#clearAllButton').attr('disabled', 'disabled');
        },

        showLoadingOverlay: function () {
            loadingOverlay.show();
        },

        hideLoadingOverlay: function () {
            loadingOverlay.hide();
        },

        updateCounts: function () {
            // find all the form data
            var accLevel = $('#AccreditationLevelId').val();
            var state = $('#StateId').val();
            var country = $('#CountryId').val();
            
            // post the form data to the server via ajax
            $.post(countUrl,
                {
                    //  'AccreditationCategoryId': accCategory,
                    'FirstLanguageId': fromLangugeId,
                    'SecondLanguageId': toLanguageId,
                    'AccreditationLevelId': accLevel,
                    'StateId': state,
                    'CountryId': country,
                    'Skills': skillList
                },
                function(data) {
                    
                    pdSearch.updateSearch(data);
                    oldCounts = data;
                }
            );
        },

        assignRandomSeed: function (randomNumber) {
            $('#RandomSearchSeed').val(randomNumber);
        },

        // Random number generation starts
        nextRandomNumber: function () {
            var hi = this.seed / this.Q;
            var lo = this.seed % this.Q;
            var test = this.A * lo - this.R * hi;
            if (test > 0) {
                this.seed = test;
            } else {
                this.seed = test + this.M;
            }
            return (this.seed * this.oneOverM);
        },

        RandomNumberGenerator: function () {
            var d = new Date();
            this.seed = 2345678901 + (d.getSeconds() * 0xFFFFFF) + (d.getMinutes() * 0xFFFF);
            this.A = 48271;
            this.M = 2147483647;
            this.Q = this.M / this.A;
            this.R = this.M % this.A;
            this.oneOverM = 1.0 / this.M;
            this.next = pdSearch.nextRandomNumber;
            return this;
        },

        createRandomNumber: function (min, max) {
            var rand = new pdSearch.RandomNumberGenerator();
            return Math.round((max - min) * rand.next() + min);
        },
        // Random number generation ends

        updateAustralianStateEnabled: function () {
            if ($(countryIdSelector).val() == australiaCountryId) {
                $(australianStateIdSelector).removeAttr('disabled');
                $(suburbSelector).removeAttr('disabled');
                $(postcodeSelector).removeAttr('disabled');
            } else {
                $(australianStateIdSelector).attr('disabled', 'disabled');
                $(suburbSelector).attr('disabled', 'disabled');
                $(postcodeSelector).attr('disabled', 'disabled');

                $(australianStateIdSelector).val('0');
                $(suburbSelector).val('');
                $(postcodeSelector).val('');
            }
        },

        ShowResults: function () {
            $('#resultsOnlyLoadingMessageDiv').hide();
            $('#sortingContainer').show();
            $('#resultsDiv').show();
            $('#gbox_resultsgrid').hide();
        },

        HideResults: function () {
            $('.contactDialog').each(function () { $(this).dialog('close'); });
            $(formSelector).show();
            $('#resultsDiv').hide();
        },

        searchSuccess: function (jsonWrapper) {
            $('#searchButton').text('Search');
            $('#searchButton').removeAttr('disabled');
            $('#clearAllButton').removeAttr('disabled');

            pdSearch.hideLoadingOverlay();
            pdSearch.hideMessageArea();

            if (jsonWrapper.Message !== "") {
                pdSearch.showMessageArea(jsonWrapper.Message, jsonWrapper.IsError);
            }

            if (jsonWrapper.Code === "1") {
                previousFormDataBroadened = true;
                for (var i = 0; i < previousFormData.length; i++) {
                    if (previousFormData[i].name === "AccreditationLevelId" || previousFormData[i].name === "StateId") {
                        previousFormData[i].value = "0";
                    }

                    if (previousFormData[i].name === "Suburb" || previousFormData[i].name === "Postcode") {
                        previousFormData[i].value = "";
                    }
                }
            } else {
                previousFormDataBroadened = false;
            }

            if (jsonWrapper.RecaptchaRequired) {
                window.location = recaptchaScreenUrl;
            } else if (!jsonWrapper.IsError){

                if (jsonWrapper.Data.TotalResultsCount > 0) {

                    pdSearch.ShowResults();

                    var source = $("#results-template").html();
                    var template = Handlebars.compile(source);
                    $('#resultsContainer').html("");

                    for (var i = 0; i < jsonWrapper.Data.Results.length; i++) {

                        var context = {
                            name: jsonWrapper.Data.Results[i].Title + " " + jsonWrapper.Data.Results[i].GivenName + " " + jsonWrapper.Data.Results[i].Surname,
                            skills: jsonWrapper.Data.Results[i].Skills,
                            id: jsonWrapper.Data.Results[i].Id,
                            location: jsonWrapper.Data.Results[i].Location,
                            hash: jsonWrapper.Data.Results[i].Hash
                        };

                        var html = template(context);
                        $('#resultsContainer').html($('#resultsContainer').html() + html);
                    }

                    $('#searchCriteria').innerHTML = jsonWrapper.Criteria.replace(/\r\n/g, '<br />');
                    pdSearch.updateResultsLastUpdated(jsonWrapper.Data.ResultsLastUpdated);

                    source = $("#paging-template").html();
                    template = Handlebars.compile(source);
                    var context = {
                        totalPages: jsonWrapper.Data.TotalPageCount,
                        pageSize: jsonWrapper.Data.PageSize
                    }
                    noOfPages = jsonWrapper.Data.TotalPageCount;
                    $('#pagingContainer').html(template(context));

                    var pageDropDown = $('#pagingCurrentPage');
                    pageDropDown.empty();
                    for (var j = 1; j <= jsonWrapper.Data.TotalPageCount; j++) {
                        var selected = j == jsonWrapper.Data.PageNumber ? 'selected' : '';
                        pageDropDown.append('<option id="' + j + '" ' + selected + '>' + j + '</option>');
                    }
                    $('#print-link').removeAttr("disabled");
                    $('#bypassrecaptcha').val('true');
                } else {
                    $('#resultsContainer').html("");
                    $('#print-link').attr("disabled", "disabled");
                    pdSearch.showMessageArea('No matching practitioners were found. To broaden your search, remove some of your search criteria.', false);
                    $('#pagingContainer').html("");
                }
            }
        },

        hideMessageArea: function () {
            var messageArea = $('#messageArea');
            messageArea.removeClass('alert-info');
            messageArea.removeClass('alert-danger');
            messageArea.removeClass('alert-warning');
            messageArea.find('span').text('');
            messageArea.hide();
        },

        showMessageArea: function (text, isError) {
            var cssClass = 'alert-info';
            var messageArea = $('#messageArea');
            messageArea.find('span').text(text);

            if (isError != null) {
                cssClass = isError ? 'alert-danger' : 'alert-warning';
            }

            messageArea.removeClass('alert-info');
            messageArea.removeClass('alert-danger');
            messageArea.removeClass('alert-warning');
            messageArea.addClass(cssClass);

            messageArea.show();
        },

        OpenContactDialog: function (identifier, hash) {
            var url = contactUrl;
            url = url.replace(/PLACEHOLDER/, identifier);
            url = url.replace(/HASHHOLDER/, hash);
            url = url.replace(/SEEDHOLDER/, $('#RandomSearchSeed').val());
            url = url.replace(/&amp;/g, '&');

            // load remote content 
            $('#contactDetailsModal .modal-content').load(
                url,
                function (responseText, textStatus, XMLHttpRequest) {
                    $('#contactDetailsModal').modal('show');
                }
            );

            //prevent the browser to follow the link 
            return false;
        }
    }
} ();
