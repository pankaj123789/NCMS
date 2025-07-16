(function () {
    "use strict";

    function Grid(selector, form, url) {
        var grid = null;
        var gridSelector = selector;
        var formSelector = form;
        var searchUrl = url;
        var gridLoaded = false;

        var defaultOptions = {
            datatype: loadGridData,
            colModel: [],
            viewrecords: true,
            pager: '#pager',
            rowList: [10, 20, 30],
            rowNum: 10,
            emptyrecords: "No matching search results were found.",
            height: 235,
            autowidth: true,
            shrinkToFit: false
        };

        this.init = InitGrid;

        function InitGrid(options) {
            if (!gridLoaded) {
                gridLoaded = true;
                defaultOptions = $.extend(defaultOptions, options);
                grid = $(gridSelector).jqGrid(defaultOptions);
            }
            else {
                //  Reset grid to page 1 and reload.
                $(gridSelector).setGridParam({ page: 1 });
                $(gridSelector).trigger('reloadGrid');
            }

            return grid;
        }

        function loadGridData(postData) {
            getResultsSuccessEvent(postData, formSelector, gridSelector, searchUrl, addCell, null, beforeSuccess);
        }

        function addCell(rows, item) {
            var cell = [];

            for (var i = 0; i < defaultOptions.colModel.length; i++) {
                var colModel = defaultOptions.colModel[i];
                var name = colModel.index;
                var itemValue = null;
                if (name in item) {
                    itemValue = item[name];
                }
                if (defaultOptions.colModel[i].customFormatter) {
                    itemValue = defaultOptions.colModel[i].customFormatter(itemValue, item);
                }

                cell.push(itemValue);
            }

            rows.push({ cell: cell });
        }

        function endReq() {
            $("#lui_searchGrid").hide();
            $("#load_searchGrid").hide();
        }

        function performAction(url) {
            postAndReload(url);
        }

        function postAndReload(url) {
            beginReq();
            $.post(url, {}, function () {
                $(gridSelector).trigger('reloadGrid');
            });
        }

        function beforeSuccess(jsonWrapper) {
            if (jsonWrapper.IsError) {
                alert(jsonWrapper.Message);
            }
            else {
                $(gridSelector)[0].addJSONData(formatResults(jsonWrapper.Data, addCell));
            }
            endReq();
        }
    };

    window.beginReq = function () {
        $("#lui_searchGrid").show();
        $("#load_searchGrid").show();
    };

    if (!window.Grid) {
        window.Grid = Grid;
    }
})();