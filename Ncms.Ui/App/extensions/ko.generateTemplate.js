// http://stackoverflow.com/questions/20473070/jquery-knockout-render-template-in-memory
ko.generateTemplate = function (name, data, tag) {
    var hasTag = tag;
    tag = tag || "<div>";

    // create temporary container for rendered html
    var temp = $(tag);

    // apply "template" binding to div with specified data
    ko.applyBindingsToNode(temp[0], { template: { name: name, data: data } });

    // save inner html of temporary div
    if (hasTag) {
        return temp[0];
    }

    var html = temp.html();

    // cleanup temporary node and return the result
    temp.remove();

    return html;
};