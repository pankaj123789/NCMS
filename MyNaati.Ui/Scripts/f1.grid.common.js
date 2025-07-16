function beginReq() {
    $("#lui_resultsgrid").show();
    $("#load_resultsgrid").show();
}

function endReq() {
    $("#lui_resultsgrid").hide();
    $("#load_resultsgrid").hide();
}

function mergeFormDataWithGridData(formData, gridData) {
    for (var propertyName in gridData)
        formData.push({ name: propertyName, value: gridData[propertyName] });
}

function getResults(postData, searchFormSelector, resultsGridSelector, searchUrl, push, onResultsReceived) {
    $(searchFormSelector).ajaxSubmit({
        beforeSubmit: function (formValues) { mergeFormDataWithGridData(formValues, postData); },
        type: 'post',
        dataType: 'json',
        url: searchUrl,
        success: function (data) {
            $(resultsGridSelector)[0].addJSONData(formatResults(data, push));
            if (onResultsReceived != null)
                onResultsReceived(data);
            endReq();
        },
        error: endReq,
        beforeSend: beginReq
    });
}

function getResultsSuccessEvent(postData, searchFormSelector, resultsGridSelector, searchUrl, push, successFunction, beforeSuccessFunction) {
    if (beforeSuccessFunction == null) {
        beforeSuccessFunction = function (data) {
            var results = formatResults(data, push);
            $(resultsGridSelector)[0].addJSONData(results);
            successFunction();
            endReq();
        };
    }

    var options = {
        beforeSubmit: function (formValues) { mergeFormDataWithGridData(formValues, postData); },
        type: 'post',
        dataType: 'json',
        url: searchUrl,
        success: beforeSuccessFunction,
        error: endReq,
        beforeSend: beginReq
    };

    if (searchFormSelector) {
        $(searchFormSelector).ajaxSubmit(options);
    }
    else {
        $.ajax(options);
    }
}

function handleZeroRecordsMessage(grid, message) {
    var spanId = grid.id + '_missingRecordsMessage';
    var span = $('#' + spanId);
    if (span.length == 0) {
        var newSpan = document.createElement('span');
        newSpan.id = spanId;
        newSpan.className = 'subtleAdvisoryMessage'
        newSpan.innerText = message;
        grid.parentNode.insertBefore(newSpan)

        //this is so hide/show methods exist
        span = $('#' + spanId);
    }

    if (grid.rows.length > 0)
        span.hide();
    else
        span.show();
}

// Convert the returned json into the format the grid expects
function formatResults(searchResults, push) {
    var rows = Array();
    if (searchResults.Results != null) {
        for (i = 0; i < searchResults.Results.length; i++) {
            var item = searchResults.Results[i];
            push(rows, item);
        }
    }

    return { total: searchResults.TotalPageCount, records: searchResults.TotalResultsCount, page: searchResults.PageNumber, rows: rows };
};

function swapGridRows(row1Id, row2Id) {
    var rowData1 = $(gridSelector).getRowData(row1Id);
    var rowData2 = $(gridSelector).getRowData(row2Id);
    $(gridSelector).setRowData(row2Id, rowData1);
    $(gridSelector).setRowData(row1Id, rowData2);
}
